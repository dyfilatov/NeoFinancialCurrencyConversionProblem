using NeoFinancialCurrencyConversionProblem.Models;

namespace NeoFinancialCurrencyConversionProblem.Infrastructure;

/// <summary>
/// Searcher for conversion rates 
/// </summary>
public class ConversionRatesSearcher
{
    public Dictionary<string, ConversionPathBuilder> SearchBestConversionPathsFromOneToOthers(string currencyCode,
        ConversionRate[] rates)
    {
        //Validate
        var bestCurrencyRates = new Dictionary<string, ConversionPathBuilder>();
        if (currencyCode == null || rates == null) return bestCurrencyRates;
        rates = rates.Where(r => r.FromCurrencyCode != null && r.ToCurrencyCode != null).ToArray();

        //Fill dictionary and currencies hashset for a faster and convenient search 
        var currencies = new HashSet<Currency>();
        var ratesDict = new Dictionary<string, List<ConversionRate>>();
        foreach (var rate in rates)
        {
            if (ratesDict.ContainsKey(rate.FromCurrencyCode))
            {
                ratesDict[rate.FromCurrencyCode].Add(rate);
            }
            else
            {
                ratesDict.Add(rate.FromCurrencyCode, new List<ConversionRate>() { rate });
            }

            currencies.Add(new Currency(rate.FromCurrencyCode)
            {
                Name = rate.FromCurrencyName
            });
            currencies.Add(new Currency(rate.ToCurrencyCode)
            {
                Name = rate.ToCurrencyCode
            });
        }

        /*
         * For every rate (edge in our graph) from a given currency start BFS.
         * BFS from one vertex to another is O(V+E), we need to find paths from one vertex to all others.
         * So this algorithm complexity is O((V+E)*V).
         * For complexity optimization purposes, we can add memorization, but it would make a little sense in practice.
         */
        
        //Add initial rates (edges)
        var queue = new Queue<ConversionPathBuilder>();
        var fromCurrencyRates = ratesDict.ContainsKey(currencyCode) 
            ? ratesDict[currencyCode] 
            : new List<ConversionRate>();
        foreach (var rate in fromCurrencyRates)
        {
            var pathBuilder = new ConversionPathBuilder().Add(rate);
            queue.Enqueue(pathBuilder);
        }
        
        //Go through all other edges
        while (queue.Count != 0)
        {
            var queueLength = queue.Count;
            for (var i = 0; i < queueLength; i++)
            {
                var pathBuilder = queue.Dequeue();
                var currentRate = pathBuilder.Last;
                var currentCurrencyCode = currentRate.ToCurrencyCode;

                if (!bestCurrencyRates.TryAdd(currentCurrencyCode, pathBuilder) &&
                    pathBuilder.FinalRate() > bestCurrencyRates[currentCurrencyCode].FinalRate())
                {
                    Console.WriteLine(
                        $"Found better rate {bestCurrencyRates[currentCurrencyCode].FinalRate()} < {pathBuilder.FinalRate()}{Environment.NewLine}" +
                        $"Old: {bestCurrencyRates[currentCurrencyCode].ToPipeString()} -> New: {pathBuilder.ToPipeString()}");
                    bestCurrencyRates[currentCurrencyCode] = pathBuilder;
                }

                if (!ratesDict.ContainsKey(currentCurrencyCode)) continue;
                //Add a copy of paths to the next iteration if path has not been traversed yet.
                foreach (var rate in ratesDict[currentCurrencyCode].Except(pathBuilder.Path))
                {
                    queue.Enqueue(pathBuilder.Copy().Add(rate));
                }
            }
        }

        //If there is no path from given currency to another currency, then add this currency with rate 0.
        //Do not count given currency. There is no information about that in the task, but IMHO we need to handle such cases. 
        var notPresentCurrencyCodes = currencies.Select(c => c.Code).Except(bestCurrencyRates.Keys).Where(c => c != currencyCode).ToList();
        foreach (var notPresentCurrencyCode in notPresentCurrencyCodes)
        {
            currencies.TryGetValue(new Currency(notPresentCurrencyCode), out var currency);
            bestCurrencyRates.Add(notPresentCurrencyCode, new ConversionPathBuilder());
            var selfConversionRate = new ConversionRate()
            {
                ExchangeRate = 0,
                FromCurrencyCode = notPresentCurrencyCode,
                ToCurrencyCode = notPresentCurrencyCode,
                FromCurrencyName = currency.Country,
                ToCurrencyName = currency.Country
            };
            bestCurrencyRates[notPresentCurrencyCode].Add(selfConversionRate);
        }

        return bestCurrencyRates;
    }
}