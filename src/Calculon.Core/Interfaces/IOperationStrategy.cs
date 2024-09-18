namespace Calculon.Core.Interfaces
{
    public interface IOperationStrategy
    {
        decimal Execute(IReadOnlyList<decimal> numbers);
        string GetFormula(IReadOnlyList<decimal> numbers);
    }
}