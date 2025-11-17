import { use, useEffect, useMemo, useState } from "react";

import {
    Box,
    Container,
    Typography,
} from "@mui/material";
import { DataGrid, type GridColDef, type GridRenderCellParams } from "@mui/x-data-grid";
import log from "loglevel";
import type CurrencyConversionItem from "../Abstractions/CurrencyConversionItem";
import type PurchaseTransaction from "../Abstractions/PurchaseTransaction";
import * as Constants from "../constants";
import { getTreasuryConversionsAsync } from "../services/TreasuryApiService";
import dayjs from "dayjs";

log.setLevel(log.levels.DEBUG);

type TransactionTableProps = {
    selectedCurrency: string;
};

export default function TransactionTable({ selectedCurrency }: TransactionTableProps) {
    log.info("TransactionTable selectedCurrency:", selectedCurrency);
    const [convertedRows, setConvertedRows] = useState<Array<PurchaseTransaction>>([]);
    const [conversionRates, setConversionRates] = useState<Array<CurrencyConversionItem>>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const columns: GridColDef[] = [
        {
            field: "id",
            headerName: "ID",
            width: 320
        },
        { field: "description", headerName: "Description", width: 300 },
        {
            field: "transaction_date",
            headerName: "Transaction Date",
            width: 140,
            renderHeader: () => (
                <Typography variant="body2" align="center" sx={{ whiteSpace: "normal", lineHeight: 1.1 }}>
                    Transaction<br />Date
                </Typography>
            ),
            renderCell: (params) =>
                params.value ? dayjs(params.value).format('YYYY-MM-DD') : '-'
        },
        {
            field: "purchase_amount",
            headerName: "Purchase Amount", width: 120,
            renderHeader: () => (
                <Typography variant="body2" align="center" sx={{ whiteSpace: "normal", lineHeight: 1.1 }}>
                    Purchase<br />Amount
                </Typography>
            ),
            renderCell: (params) => (
                <Box sx={{ display: 'flex', alignItems: 'center', height: '100%', width: '100%' }}>
                    <Typography sx={{ ml: 'auto' }}>
                        {Number(params.value).toFixed(2)}
                    </Typography>
                </Box>
            )
        },
        {
            field: "exchange_rate",
            headerName: "Exchange Rate",
            width: 120,
            renderHeader: () => (
                <Typography variant="body2" align="center" sx={{ whiteSpace: "normal", lineHeight: 1.1 }}>
                    Exchange<br />Rate
                </Typography>
            ),
            renderCell: (params: GridRenderCellParams<PurchaseTransaction>) => {
                const value = params.value;
                if (value == null || value === 0) {
                    return (
                        <Typography color="error" variant="body2">
                            no conversion<br /> available
                        </Typography>
                    );
                }
                return (
                    <Box sx={{ display: 'flex', alignItems: 'center', height: '100%', width: '100%' }}>
                        <Typography variant="body2" sx={{ ml: 'auto' }}>
                            {Number(value).toFixed(2)}
                        </Typography>
                    </Box>
                );
            },
        },
        {
            field: "converted_amount",
            headerName: "Converted Amount",
            width: 120,
            renderHeader: () => (
                <Typography variant="body2" align="center" sx={{ whiteSpace: "normal", lineHeight: 1.1 }}>
                    Converted<br />Amount
                </Typography>
            ),
            renderCell: (params: GridRenderCellParams<PurchaseTransaction>) => {
                const value = params.value;
                if (value == null) {
                    return (
                        <Typography color="error" variant="body2">
                            no conversion<br /> available
                        </Typography>
                    );
                }
                return (
                    <Box sx={{ display: 'flex', alignItems: 'center', height: '100%', width: '100%' }}>
                        <Typography variant="body2" sx={{ ml: 'auto' }}>
                            {Number(value).toFixed(2)}
                        </Typography>
                    </Box>
                );
            },
        },
    ];

    useEffect(() => {
        const fetchConversions = async () => {
            try {
                const conversionData = await getTreasuryConversionsAsync(selectedCurrency);
                log.debug(`TransactionTable::Fetched currency conversions: ${JSON.stringify(conversionData)}`);
                const sortedConversions = conversionData.sort((a, b) =>
                    b.effective_date.localeCompare(a.effective_date)
                );
                log.debug(`TransactionTable::sorted currency conversions: ${JSON.stringify(sortedConversions)}`);
                setConversionRates(sortedConversions);
            } catch (err) {
                log.error(`Failed to fetch currency conversions: ${err}`);
                setError("Failed to fetch currency conversions");
            }
            finally {
                setLoading(false);
            }
        };
        fetchConversions();
    }, [selectedCurrency]);

    useEffect(() => {
        const raw = sessionStorage.getItem(Constants.PURCHASES_KEY_NAME);
        const origninalTxns = raw ? JSON.parse(raw) : [];
        const transactions = recalculateConversions(origninalTxns);
        setConvertedRows(transactions);
        setLoading(false);
    }, [conversionRates]);

    function recalculateConversions(txns: Array<PurchaseTransaction>): Array<PurchaseTransaction> {
        if (txns && txns.length > 0) {
            const updatedRows = txns.map((txn) => {
                const exchangeRate = getExchangeRate(txn.transaction_date);
                return {
                    ...txn,
                    exchange_rate: exchangeRate,
                    converted_amount: exchangeRate ? txn.purchase_amount * exchangeRate : null,
                };
            });
            return updatedRows;
        }
        return [];
    }

    function getExchangeRate(transaction_date: Date): number | null {
        if (!selectedCurrency || selectedCurrency === "") {
            log.info("TransactionTable::No currency selected, returning 1.0");
            return 1.0; // no currency selected, return 1.0
        };
        const cutoffDate = dayjs(transaction_date).subtract(6, 'month').format("YYYY-MM-DD");
        log.info(`TransactionTable::Getting exchange rate for date ${dayjs(transaction_date).format("YYYY-MM-DD")} with cutoff ${cutoffDate}`);
        const txnDateString = dayjs(transaction_date).format("YYYY-MM-DD");
        const conversion = conversionRates
            .filter(conversion =>
                conversion.effective_date >= cutoffDate &&
                conversion.effective_date <= txnDateString
            )[0] || null;
        return conversion ? conversion.exchange_rate : null;
    }

    if (loading) {
        return (
            <div>
                <Typography variant="h4" gutterBottom>
                    Transactions
                </Typography>
                Loading conversions for {selectedCurrency}...
            </div>
        );
    }

    if (error) {
        return (
            <div>
                <Typography variant="h4" gutterBottom>
                    Transactions
                </Typography>
                Error: {error}.
            </div>
        );
    }

    if (!convertedRows || convertedRows.length === 0) {
        return (
            <div>
                <Typography variant="h4" gutterBottom>
                    Transactions
                </Typography>
                No transactions available.
            </div>
        );
    }

    return (
        <Box component="div">
            <Typography variant="h4" gutterBottom>
                Transactions
            </Typography>
            <Box
                component="div"
                sx={{ height: 400, width: "100%" }}
            >
                <DataGrid
                    rows={convertedRows}
                    columns={columns}
                />
            </Box>
        </Box>
    );
}
