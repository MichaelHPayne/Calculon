using Calculon.Core.Interfaces;

namespace Calculon.Infrastructure.DelimiterStrategies
{
    public class DefaultDelimiterStrategy : IDelimiterStrategy
    {
        private readonly string _delimiter;

        public DefaultDelimiterStrategy(string delimiter) => _delimiter = delimiter;

        public bool CanHandle(string input) => true;

        public IReadOnlyList<string> Split(string input) => input.Split(_delimiter);
    }
}
