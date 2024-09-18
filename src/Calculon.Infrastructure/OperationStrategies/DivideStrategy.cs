using Calculon.Core.Interfaces;

namespace Calculon.Infrastructure.OperationStrategies
{
    public class DivideStrategy : IOperationStrategy
    {
        public decimal Execute(IReadOnlyList<decimal> numbers) =>
            numbers.Count > 1 && numbers[1] != 0
                ? numbers.Select(n => (decimal)n).Aggregate((x, y) => x / y)
                : 0;

        public string GetFormula(IReadOnlyList<decimal> numbers) => string.Join("/", numbers);
    }
}