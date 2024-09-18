namespace Calculon.Core.Models
{
    public record RawInput(string Value);
    public record ProcessedInput(string Value);
    public record NumberStrings(IReadOnlyList<string> Values);
    public record ParsedNumbers(IReadOnlyList<decimal> Values);
    public record CalculationResult(decimal Result, string Formula);

}
