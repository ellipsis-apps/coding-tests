import {
    Autocomplete,
    Box,
    Button,
    Container,
    FormControl,
    FormLabel,
    TextField,
    Typography,
} from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import dayjs from "dayjs";
import { type FormEvent, useEffect, useState } from "react";
import { NumericFormat } from "react-number-format";
import { useSessionStorage } from "usehooks-ts";
import type PurchaseTransaction from "../Abstractions/PurchaseTransaction";
import * as Constants from "../constants";
import log from "loglevel";
import { getTreasuryCurrenciesAsync } from "../services/TreasuryApiService";
import type CurrencyDescItem from "../Abstractions/CurrencyDescItem";
import { LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import TransactionTable from "../components/transactionTable";

log.setLevel(log.levels.DEBUG);

export default function PurchasePage() {
    const today = new Date();
    today.setHours(0, 0, 0, 0); // Optional: reset time
    const [transactionDate, setTransactionDate] = useState<Date>(today);
    const [purchases, setPurchases] = useSessionStorage<Array<PurchaseTransaction> | null>(Constants.PURCHASES_KEY_NAME, null);
    const [selectedCurrency, setSelectedCurrency] = useState<string>("");
    const [currencies, setCurrencies] = useSessionStorage<Array<string> | null>(Constants.CURRENCIES_KEY_NAME, null);
    const [renderTableToggle, setRenderTableToggle] = useState<boolean>(false);

    const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        const data = new FormData(event.currentTarget);
        console.log({
            name: data.get("description"),
            transaction_date: transactionDate,
            purchase_amount: data.get("purchase_amount"),
        });
        const purchaseTransaction: PurchaseTransaction = {
            id: crypto.randomUUID(),
            description: data.get("description") as string,
            transaction_date: transactionDate, //data.get("transaction_date") ? Date.parse(data.get("transaction_date")) : today,
            purchase_amount: parseFloat(data.get("purchase_amount") as string),
            exchange_rate: 1.0, // Placeholder value
        };
        const currPurchases = purchases ? [...purchases] : [];
        currPurchases.push(purchaseTransaction);
        sessionStorage.setItem(Constants.PURCHASES_KEY_NAME, JSON.stringify(currPurchases));
        setPurchases(currPurchases);
        setTransactionDate(today);
        setRenderTableToggle(!renderTableToggle); // trigger re-render
    };

    const handleCurrencyChange = (event: any, newValue: string | null) => {
        setSelectedCurrency(newValue || "");
        setRenderTableToggle(!renderTableToggle); // trigger re-render
    };

    useEffect(() => {
        const getCurrencies = async () => {
            try {
                const response = await getTreasuryCurrenciesAsync(); // Axios response
                const data: CurrencyDescItem[] = response; // Extract data
                if (!Array.isArray(data)) {
                    throw new Error('API did not return an array');
                }
                const currencyList = data.map(
                    (item) => item.country_currency_desc
                );
                // log.info("Fetched currencies:", currencyList);
                setCurrencies(currencyList);
            } catch (err) {
                log.error("Failed to fetch currencies", err);
            }
        };
        getCurrencies();
        setRenderTableToggle(!renderTableToggle); // trigger re-render
    }, []);

    return (
        <Container maxWidth="lg" sx={{
            mt: 0,
            ml: 0, // remove auto-centering
        }}>
            <Box sx={{ maxWidth: { xs: '100%', sm: 300 }, display: "flex", flexDirection: "column", gap: 2, mb: 4 }}
                component="form"
                onSubmit={handleSubmit}
            >
                <Typography variant="h4" gutterBottom>
                    Enter Transaction
                </Typography>
                {/* form fields */}
                <TextField
                    label="Description"
                    name="description"
                    variant="outlined"
                    required
                    fullWidth
                />
                <LocalizationProvider dateAdapter={AdapterDayjs}>
                    <DatePicker
                        label="Transaction Date"
                        value={dayjs(transactionDate)}           // Convert Date → Dayjs
                        onChange={(date) => {
                            setTransactionDate(date?.toDate() ?? today); // fallback to today
                        }}
                        format="YYYY-MM-DD"                      // Show ISO format
                        slotProps={{
                            textField: { fullWidth: true, required: true },
                        }}
                    />
                </LocalizationProvider>
                <NumericFormat
                    customInput={TextField}
                    name="purchase_amount"
                    label="Purchase Amount"
                    decimalScale={2}   // exactly 2 decimals
                    fixedDecimalScale
                    allowNegative={false}
                />
                <FormControl>
                    <FormLabel>Purchases</FormLabel>
                </FormControl>
                <Button type="submit" variant="contained" color="primary">
                    Submit
                </Button>
            </Box>
            {/* autocomplete */}
            <Box component="div" sx={{ maxWidth: { xs: '100%', sm: 600 }, display: "flex", flexDirection: "column", gap: 2, mb: 4 }}>
                <Autocomplete
                    options={currencies ? currencies : []}
                    value={selectedCurrency}
                    onChange={(event, newValue) => handleCurrencyChange(event, newValue || "")}
                    renderInput={(params) => <TextField {...params} label="Currency" />}
                    sx={{ width: "60%", mb: 0 }}
                />
            </Box>
            {/* transaction table */}
            <Box component="div" sx={{ maxWidth: { xs: '100%', sm: 1600 }, display: "flex", flexDirection: "column", gap: 2, mb: 4 }}>
                <TransactionTable selectedCurrency={selectedCurrency} key={renderTableToggle.toString()}/>
            </Box>
        </Container>
    );
}

