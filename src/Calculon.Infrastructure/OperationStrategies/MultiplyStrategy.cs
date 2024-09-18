using Calculon.Core.Interfaces;

namespace Calculon.Infrastructure.OperationStrategies
{
    public class MultiplyStrategy : IOperationStrategy
    {
        public decimal Execute(IReadOnlyList<decimal> numbers) =>
            numbers.Aggregate(1m, (x, y) => x * y);

        public string GetFormula(IReadOnlyList<decimal> numbers) => string.Join("*", numbers);
    }
}