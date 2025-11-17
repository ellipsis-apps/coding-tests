import Guid from './Guid';

export default interface PurchaseTransaction {
    id: Guid;
    description: string;
    transaction_date: Date;
    purchase_amount: number;
    exchange_rate: number;
    converted_amount?: number;
}
