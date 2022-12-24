namespace NeoFinancialCurrencyConversionProblem.Models;

public struct Currency
{
    public Currency(string code)
    {
        Code = code;
    }

    public string Code { get; set; }
    public string Name { get; set; }
    public string Country
    {
        get
        {
            if (Name == null) return "";
            return Name.LastIndexOf(' ') > 0 ? string.Concat(Name[..Name.LastIndexOf(' ')]) : Name;
        }
    }

    private bool Equals(Currency other)
    {
        return Code == other.Code;
    }

    public override bool Equals(object? obj)
    {
        return obj is Currency other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Code.GetHashCode();
    }
}