using Calculon.Core.Interfaces;

public class AddStrategy : IOperationStrategy
{
    public decimal Execute(IReadOnlyList<decimal> numbers) => numbers.Sum(n => (decimal)n);

    public string GetFormula(IReadOnlyList<decimal> numbers) =>
        numbers.Count == 1 ? numbers[0].ToString() : string.Join("+", numbers);
}