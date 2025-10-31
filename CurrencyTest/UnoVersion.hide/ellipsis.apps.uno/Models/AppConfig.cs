namespace ellipsis.apps.uno.Models;

public record AppConfig
{
    public string? Environment { get; init; }
    public string ApiBaseUrl { get; init; } = "https://api.fiscaldata.treasury.gov/services/api/fiscal_service/v1/accounting/od/rates_of_exchange";
}
