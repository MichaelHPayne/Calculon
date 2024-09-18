namespace Calculon.Core.Interfaces
{
    public interface IDelimiterStrategy
    {
        bool CanHandle(string input);
        IReadOnlyList<string> Split(string input);
    }
}
