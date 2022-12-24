using System.Net.Http.Json;
using NeoFinancialCurrencyConversionProblem.Models;

namespace NeoFinancialCurrencyConversionProblem.Infrastructure;

/// <summary>
/// HTTP client for API to get conversion rates.
/// </summary>
public class RatesApiClient
{
    public async Task<ConversionRate[]> GetConversionRatesAsync()
    {
        try
        {
            var apiUrl = "https://api-coding-challenge.neofinancial.com/currency-conversion?seed=63194";
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(apiUrl);
            return await response.Content.ReadFromJsonAsync<ConversionRate[]>() ?? Array.Empty<ConversionRate>();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Array.Empty<ConversionRate>();
        }
    }
}