using Calculon.Core.Interfaces;

namespace Calculon.Infrastructure.OperationStrategies
{
    public class SubtractStrategy : IOperationStrategy
    {
        public decimal Execute(IReadOnlyList<decimal> numbers) =>
            numbers.Select(n => (decimal)n).Aggregate((x, y) => x - y);

        public string GetFormula(IReadOnlyList<decimal> numbers) => string.Join("-", numbers);
    }
}