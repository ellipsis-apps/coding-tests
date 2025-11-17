import axios, { type AxiosResponse } from "axios";
import log from "loglevel";
import type CountryCurrencyResponse from "../Abstractions/CountryCurrencyResponse";
import type CurrencyConversionItem from "../Abstractions/CurrencyConversionItem";
import type CurrencyConversionResponse from "../Abstractions/CurrencyConversonResponse";
import type CurrencyDescItem from "../Abstractions/CurrencyDescItem";
import * as Constants from "../constants";

log.setLevel(log.levels.INFO);

export async function getTreasuryCurrenciesAsync(): Promise<
    Array<CurrencyDescItem>
> {
    const apiEndpoint = `${Constants.API_BASE_URL}?fields=country_currency_desc&page[number]=1&page[size]=25000`;
    const response: AxiosResponse<CountryCurrencyResponse> =
        await axios.get<CountryCurrencyResponse>(apiEndpoint);
    return response.data.data;
}

export async function getTreasuryConversionsAsync(
    currency_conv_desc: string
): Promise<Array<CurrencyConversionItem>> {
    // log.debug(`getTreasuryConversionsAsync::entering currency conversions`);
    // log.debug(`getTreasuryConversionsAsync::currency_conv_desc: ${currency_conv_desc}`);
    if (currency_conv_desc && currency_conv_desc.trim() !== "") {
        const apiEndpoint = `${Constants.API_BASE_URL}?filter=country_currency_desc:in:(${currency_conv_desc})&fields=exchange_rate,effective_date&page[number]=1&page[size]=25000`;
        log.info(`getTreasuryConversionsAsync::cFetching currency conversions from ${apiEndpoint}`);
        const response: AxiosResponse<CurrencyConversionResponse> = await axios.get<CurrencyConversionResponse>(apiEndpoint);
        log.info(`getTreasuryConversionsAsync::found currency conversions: ${JSON.stringify(response.data)}`);
        return response.data.data;
    }
    return [];
}
