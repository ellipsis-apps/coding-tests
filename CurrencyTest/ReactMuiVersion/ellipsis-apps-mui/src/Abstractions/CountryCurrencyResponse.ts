import type CurrencyDescItem from "./CurrencyDescItem";

export default interface CountryCurrencyResponse {
    data: CurrencyDescItem[];
    meta: object; // don't care what data is here
    links: object; // don't care what data is here
}
