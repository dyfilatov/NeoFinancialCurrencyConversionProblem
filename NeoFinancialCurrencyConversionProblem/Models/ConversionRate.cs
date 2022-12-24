namespace NeoFinancialCurrencyConversionProblem.Models;

/// <summary>
/// Type that we get from API
/// </summary>
public class ConversionRate
{
    public decimal ExchangeRate { get; set; }
    public string FromCurrencyCode { get; set; }
    public string FromCurrencyName { get; set; }
    public string ToCurrencyCode { get; set; }
    public string ToCurrencyName { get; set; }

    public override string ToString()
    {
        return $"From: {FromCurrencyCode} To: {ToCurrencyCode} {ExchangeRate}";
    }


    protected bool Equals(ConversionRate other)
    {
        return FromCurrencyCode == other.FromCurrencyCode && ToCurrencyCode == other.ToCurrencyCode;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ConversionRate)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FromCurrencyCode, ToCurrencyCode);
    }
}