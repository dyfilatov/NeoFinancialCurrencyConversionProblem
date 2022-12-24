using NeoFinancialCurrencyConversionProblem.Infrastructure;
using NeoFinancialCurrencyConversionProblem.Models;

namespace NeoFinancialCurrencyConversionProblemTests;

public class ExtensionsTests
{
    [Theory]
    [MemberData(nameof(ConversionExchangeRatesData))]
    public void Should_CalculateFinalConversionRate(ConversionPathBuilder path, decimal expectedRate)
    {
        Assert.Equal(expectedRate, path.FinalRate());
    }

    [Theory]
    [MemberData(nameof(ConversionRateCodesData))]
    public void Should_ConvertPathToString(ConversionPathBuilder path, string expected)
    {
        Assert.Equal(expected, path.ToPipeString());
    }

    public static IEnumerable<object[]> ConversionRateCodesData()
    {
        yield return new object[]
        {
            new ConversionPathBuilder(),
            ""
        };
        yield return new object[]
        {
            new ConversionPathBuilder()
                .Add(new ConversionRate { FromCurrencyCode = "AAA", ToCurrencyCode = "BBB" }),
            "AAA | BBB"
        };
        yield return new object[]
        {
            new ConversionPathBuilder()
                .Add(new ConversionRate { FromCurrencyCode = "AAA", ToCurrencyCode = "AAB" })
                .Add(new ConversionRate { FromCurrencyCode = "AAB", ToCurrencyCode = "AAC" })
                .Add(new ConversionRate { FromCurrencyCode = "AAC", ToCurrencyCode = "ABA" })
                .Add(new ConversionRate { FromCurrencyCode = "ABA", ToCurrencyCode = "ABB" })
                .Add(new ConversionRate { FromCurrencyCode = "ABB", ToCurrencyCode = "ABC" }),
            "AAA | AAB | AAC | ABA | ABB | ABC"
        };
    }

    public static IEnumerable<object[]> ConversionExchangeRatesData()
    {
        yield return new object[]
        {
            new ConversionPathBuilder(),
            0
        };
        yield return new object[]
        {
            new ConversionPathBuilder()
                .Add(new ConversionRate() { ExchangeRate = 10m }),
            10m
        };
        yield return new object[]
        {
            new ConversionPathBuilder()
                .Add(new ConversionRate() { ExchangeRate = 10m })
                .Add(new ConversionRate() { ExchangeRate = 10m }),
            100m
        };
        yield return new object[]
        {
            new ConversionPathBuilder()
                .Add(new ConversionRate() { ExchangeRate = 1.3m })
                .Add(new ConversionRate() { ExchangeRate = 5m }),
            6.5m
        };
        yield return new object[]
        {
            new ConversionPathBuilder()
                .Add(new ConversionRate() { ExchangeRate = 1.3m })
                .Add(new ConversionRate() { ExchangeRate = 5m })
                .Add(new ConversionRate() { ExchangeRate = 0.3m }),
            1.95m
        };
        yield return new object[]
        {
            new ConversionPathBuilder()
                .Add(new ConversionRate() { ExchangeRate = 1.3m })
                .Add(new ConversionRate() { ExchangeRate = 0m })
                .Add(new ConversionRate() { ExchangeRate = 0.3m }),
            0m
        };
    }
}