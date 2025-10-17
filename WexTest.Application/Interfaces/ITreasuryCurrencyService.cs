namespace WexTest.Application.Interfaces
{
    public interface ITreasuryCurrencyService
    {
        Task<IEnumerable<string>> GetCountryCurrencies();
    }
}
