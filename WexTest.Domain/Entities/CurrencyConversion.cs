namespace WexTest.Domain.Entities
{
    public class CurrencyConversion
    {
        public DateOnly RecordDate { get; set; }
        //public string Country { get; set; } = string.Empty;
        //public string Currency { get; set; } = string.Empty;
        public string CountryCurrencyDesc { get; set; } = string.Empty;
        public decimal ExchangeRate { get; set; }
        public DateOnly EffectiveDate { get; set; }
        //public int SrcLineNbr { get; set; }
        //public int RecordFiscalYear { get; set; }
        //public int RecordFiscalQuarter { get; set; }
        //public int RecordCalendarQuarter { get; set; }
        //public int RecordCalendarMonth { get; set; }
        //public int RecordCalendarDay { get; set; }
    }
}
