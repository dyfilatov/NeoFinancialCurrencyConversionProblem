using NeoFinancialCurrencyConversionProblem.Models;

namespace NeoFinancialCurrencyConversionProblem.Infrastructure;

/// <summary>
/// Builder for conversion paths to simplify paths the construction.
/// </summary>
public class ConversionPathBuilder
{
    public IReadOnlyCollection<ConversionRate> Path => _path.AsReadOnly();
    private readonly List<ConversionRate> _path;

    public ConversionPathBuilder()
    {
        _path = new List<ConversionRate>();
    }

    private ConversionPathBuilder(List<ConversionRate> path)
    {
        _path = path;
    }

    public ConversionPathBuilder Add(ConversionRate cr)
    {
        _path.Add(cr);
        return this;
    }

    public ConversionRate Last => _path.Count == 0 ? null : _path[^1];

    public decimal FinalRate()
    {
        if (_path == null || _path.Count == 0) return 0;
        return _path.Select(r => r.ExchangeRate).Aggregate((a, b) => a * b);
    }

    public string ToPipeString()
    {
        if (_path == null || _path.Count == 0) return "";
        
        var currencyCodesPath = _path.Select(r => r.FromCurrencyCode).ToList();
        currencyCodesPath.Add(_path.Last().ToCurrencyCode);
        return string.Join(" | ", currencyCodesPath);
    }
    
    public ConversionPathBuilder Copy()
    {
        return new ConversionPathBuilder(_path.ToArray().ToList());
    }
}