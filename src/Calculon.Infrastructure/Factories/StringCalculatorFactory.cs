using Calculon.Core.Enums;
using Calculon.Core.Interfaces;
using Calculon.Core.Models;
using Calculon.Infrastructure.DelimiterStrategies;
using Calculon.Infrastructure.OperationStrategies;

namespace Calculon.Infrastructure.Factories
{
    public static class StringCalculatorFactory
    {
        // Creates and configures a StringCalculator instance with necessary strategies and options.
        public static StringCalculator Create(CalculatorOptions options)
        {
            // Initializes delimiter strategies, allowing for easy extension with additional strategies if needed.
            var delimiterStrategies = new List<IDelimiterStrategy>
            {
                new DefaultDelimiterStrategy(options.AlternateDelimiter),
            };

            // Maps each operation type to its corresponding strategy, facilitating the addition of new operations without modifying the factory.
            var operationStrategies = new Dictionary<OperationType, IOperationStrategy>
            {
                { OperationType.Add, new AddStrategy() },
                { OperationType.Subtract, new SubtractStrategy() },
                { OperationType.Multiply, new MultiplyStrategy() },
                { OperationType.Divide, new DivideStrategy() }
            };

            // Constructs the StringCalculator with the configured delimiter and operation strategies, adhering to the Dependency Injection principle for better testability and flexibility.
            return new StringCalculator(delimiterStrategies, operationStrategies, options);
        }
    }
}
