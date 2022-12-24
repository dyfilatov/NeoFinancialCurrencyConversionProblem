using NeoFinancialCurrencyConversionProblem.Infrastructure;

var fromCurrency = "CAD";
var amount = 100m;

var apiClient = new RatesApiClient();
var searcher = new ConversionRatesSearcher();
var storage = new ConversionPathsStorage();

var rates = await apiClient.GetConversionRatesAsync();
foreach (var rate in rates)
{
    Console.WriteLine(rate.ToString());
}


var bestCadRates = searcher.SearchBestConversionPathsFromOneToOthers(fromCurrency, rates);
storage.SaveToFile(bestCadRates, amount);


Console.WriteLine("Finished");
Console.ReadLine();
