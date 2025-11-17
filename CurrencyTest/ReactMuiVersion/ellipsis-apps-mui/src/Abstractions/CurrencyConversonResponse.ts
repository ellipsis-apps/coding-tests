import type CurrencyConversionItem from "./CurrencyConversionItem";

export default interface CurrencyConversionResponse {
    data: CurrencyConversionItem[];
    meta: object; // don't care what data is here
    links: object; // don't care what data is here
}
