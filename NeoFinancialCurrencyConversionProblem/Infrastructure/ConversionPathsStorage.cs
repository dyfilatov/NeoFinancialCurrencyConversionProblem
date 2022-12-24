using System.Text;
using NeoFinancialCurrencyConversionProblem.Models;

namespace NeoFinancialCurrencyConversionProblem.Infrastructure;

/// <summary>
/// Storage for conversion paths 
/// </summary>
public class ConversionPathsStorage
{
    public void SaveToFile(Dictionary<string, ConversionPathBuilder> conversionPaths, decimal amount)
    {
        var csvStringBuilder = new StringBuilder();
        csvStringBuilder.Append($"CurrencyCode;Country;Amount;Path\r\n");
        foreach (var conversionRate in conversionPaths)
        {
            var currency = new Currency(conversionRate.Key)
            {
                Name = conversionRate.Value.Last.ToCurrencyName
            };

            var pathString = "";
            var currencyAmount = conversionRate.Value.FinalRate() * amount;
            if (currencyAmount != 0)
            {
                pathString = conversionRate.Value.ToPipeString();
            }

            csvStringBuilder.Append(
                $"{currency.Code};{currency.Country};{currencyAmount};{pathString}\r\n");
        }

        var fileName = "CurrencyRates.csv";
        File.WriteAllText(fileName, csvStringBuilder.ToString(), Encoding.UTF8);
        Console.WriteLine("Full path to the generated file: " + Path.GetFullPath(fileName));
    }
}