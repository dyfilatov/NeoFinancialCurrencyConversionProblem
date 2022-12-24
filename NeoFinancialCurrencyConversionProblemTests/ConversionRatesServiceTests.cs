using System.Text.Json;
using System.Text.Json.Serialization;
using NeoFinancialCurrencyConversionProblem.Infrastructure;
using NeoFinancialCurrencyConversionProblem.Models;

namespace NeoFinancialCurrencyConversionProblemTests;

public class ConversionRatesServiceTests
{
    private readonly ConversionRatesSearcher _conversionRateSearcher;

    public ConversionRatesServiceTests()
    {
        _conversionRateSearcher = new ConversionRatesSearcher();
    }

    [Fact]
    public void Should_ReturnEmpty_WhenInputNull()
    {
        var result = _conversionRateSearcher.SearchBestConversionPathsFromOneToOthers(null, null);
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void Should_ReturnEmpty_WhenInputEmpty()
    {
        var result =
            _conversionRateSearcher.SearchBestConversionPathsFromOneToOthers("", Array.Empty<ConversionRate>());
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void Should_ReturnOptimal_WhenRealLifeCase()
    {
        var conversionRatesJson = File.ReadAllText("RealLifeConversionRates.txt");
        var conversionRates = JsonSerializer.Deserialize<ConversionRate[]>(conversionRatesJson, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        var paths = _conversionRateSearcher.SearchBestConversionPathsFromOneToOthers("CAD", conversionRates);
        Assert.Equal(110.16848320388691390977370540m, paths["DZD"].FinalRate());
        Assert.Equal(6.2179212902670686150354461161m, paths["HKD"].FinalRate());
        Assert.Equal(1.0478536280940038307514957331m, paths["SGD"].FinalRate());
        Assert.Equal(0.670744909787547700m, paths["EUR"].FinalRate());
        Assert.Equal(0m, paths["KYD"].FinalRate());
    }

    [Fact]
    public void Should_ReturnZeros_WhenInputNotIncludesCurrency()
    {
        var conversionRates = new ConversionRate[]
        {
            new()
            {
                ExchangeRate = 12,
                FromCurrencyCode = "AAA",
                ToCurrencyCode = "BBB",
                FromCurrencyName = "A A",
                ToCurrencyName = "B B"
            },
            new()
            {
                ExchangeRate = 13,
                FromCurrencyCode = "BBB",
                ToCurrencyCode = "CCC",
                FromCurrencyName = "B B",
                ToCurrencyName = "C C"
            },
        };
        var result = _conversionRateSearcher.SearchBestConversionPathsFromOneToOthers("DDD", conversionRates);
        Assert.Contains("AAA", result.Keys);
        Assert.Contains("BBB", result.Keys);
        Assert.Contains("CCC", result.Keys);
        Assert.Equal(0m, result["AAA"].FinalRate());
        Assert.Equal(0m, result["BBB"].FinalRate());
        Assert.Equal(0m, result["CCC"].FinalRate());
    }

    [Fact]
    public void Should_ReturnOptimal_WhenInputCorrect()
    {
        var conversionRates = new ConversionRate[]
        {
            new()
            {
                ExchangeRate = 1,
                FromCurrencyCode = "AAA",
                ToCurrencyCode = "BBB",
                FromCurrencyName = "A A",
                ToCurrencyName = "B B"
            },
            new()
            {
                ExchangeRate = 0.91m,
                FromCurrencyCode = "BBB",
                ToCurrencyCode = "CCC",
                FromCurrencyName = "B B",
                ToCurrencyName = "C C"
            },
            new()
            {
                ExchangeRate = 0.95m,
                FromCurrencyCode = "CCC",
                ToCurrencyCode = "DDD",
                FromCurrencyName = "C C",
                ToCurrencyName = "D D"
            }
        };
        var result = _conversionRateSearcher.SearchBestConversionPathsFromOneToOthers("AAA", conversionRates);
        Assert.Contains("BBB", result.Keys);
        Assert.Contains("CCC", result.Keys);
        Assert.Contains("DDD", result.Keys);
        Assert.Equal(1m, result["BBB"].FinalRate());
        Assert.Equal(0.91m, result["CCC"].FinalRate());
        Assert.Equal(0.8645m, result["DDD"].FinalRate());
    }

    [Fact]
    public void Should_ReturnOptimal_WhenHaveDifferentPaths()
    {
        var conversionRates = new ConversionRate[]
        {
            new()
            {
                ExchangeRate = 0.905m,
                FromCurrencyCode = "AAA",
                ToCurrencyCode = "CCC",
                FromCurrencyName = "A A",
                ToCurrencyName = "C C"
            },
            new()
            {
                ExchangeRate = 1,
                FromCurrencyCode = "AAA",
                ToCurrencyCode = "BBB",
                FromCurrencyName = "A A",
                ToCurrencyName = "B B"
            },
            new()
            {
                ExchangeRate = 0.91m,
                FromCurrencyCode = "BBB",
                ToCurrencyCode = "CCC",
                FromCurrencyName = "B B",
                ToCurrencyName = "C C"
            },
            new()
            {
                ExchangeRate = 0.95m,
                FromCurrencyCode = "CCC",
                ToCurrencyCode = "DDD",
                FromCurrencyName = "C C",
                ToCurrencyName = "D D"
            }
        };
        var result = _conversionRateSearcher.SearchBestConversionPathsFromOneToOthers("AAA", conversionRates);
        Assert.Contains("BBB", result.Keys);
        Assert.Contains("CCC", result.Keys);
        Assert.Contains("DDD", result.Keys);
        Assert.Equal(1m, result["BBB"].FinalRate());
        Assert.Equal(0.91m, result["CCC"].FinalRate());
        Assert.Equal(0.8645m, result["DDD"].FinalRate());
    }

    [Fact]
    public void Should_ReturnOptimal_WhenInputIncludesNegativeCycle()
    {
        var conversionRates = new ConversionRate[]
        {
            new()
            {
                ExchangeRate = 1,
                FromCurrencyCode = "AAA",
                ToCurrencyCode = "BBB",
                FromCurrencyName = "A A",
                ToCurrencyName = "B B"
            },
            new()
            {
                ExchangeRate = 0.91m,
                FromCurrencyCode = "BBB",
                ToCurrencyCode = "CCC",
                FromCurrencyName = "B B",
                ToCurrencyName = "C C"
            },
            new()
            {
                ExchangeRate = 0.95m,
                FromCurrencyCode = "CCC",
                ToCurrencyCode = "DDD",
                FromCurrencyName = "C C",
                ToCurrencyName = "D D"
            },
            new()
            {
                ExchangeRate = 0.99m,
                FromCurrencyCode = "DDD",
                ToCurrencyCode = "BBB",
                FromCurrencyName = "D D",
                ToCurrencyName = "B B"
            },
        };
        var result = _conversionRateSearcher.SearchBestConversionPathsFromOneToOthers("AAA", conversionRates);
        Assert.Contains("BBB", result.Keys);
        Assert.Contains("CCC", result.Keys);
        Assert.Contains("DDD", result.Keys);
    }

    /// <summary>
    /// Impossible case in practice. This test is to fix the behaviour.
    /// </summary>
    [Fact]
    public void Should_ReturnOptimal_WhenInputIncludesPositiveCycle()
    {
        var conversionRates = new ConversionRate[]
        {
            new()
            {
                ExchangeRate = 1,
                FromCurrencyCode = "AAA",
                ToCurrencyCode = "BBB",
                FromCurrencyName = "A A",
                ToCurrencyName = "B B"
            },
            new()
            {
                ExchangeRate = 1.2m,
                FromCurrencyCode = "BBB",
                ToCurrencyCode = "CCC",
                FromCurrencyName = "B B",
                ToCurrencyName = "C C"
            },
            new()
            {
                ExchangeRate = 1.1m,
                FromCurrencyCode = "CCC",
                ToCurrencyCode = "DDD",
                FromCurrencyName = "C C",
                ToCurrencyName = "D D"
            },
            new()
            {
                ExchangeRate = 1.1m,
                FromCurrencyCode = "DDD",
                ToCurrencyCode = "BBB",
                FromCurrencyName = "D D",
                ToCurrencyName = "B B"
            },
        };
        var result = _conversionRateSearcher.SearchBestConversionPathsFromOneToOthers("AAA", conversionRates);
        Assert.Contains("BBB", result.Keys);
        Assert.Contains("CCC", result.Keys);
        Assert.Contains("DDD", result.Keys);
        Assert.Equal(1.452m, result["BBB"].FinalRate()); //Went cycle 1 time
        Assert.Equal(1.2m, result["CCC"].FinalRate());
        Assert.Equal(1.32m, result["DDD"].FinalRate());
    }
}