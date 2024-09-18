using Calculon.Core.Enums;
using Calculon.Core.Interfaces;
using Calculon.Core.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Calculon.Infrastructure.Tests.OperationStrategies
{
    public class OperationStrategyTests
    {
        private readonly IDictionary<OperationType, IOperationStrategy> _strategies;

        public OperationStrategyTests()
        {
            var services = new ServiceCollection();
            services.AddStringCalculator(new CalculatorOptions());
            var serviceProvider = services.BuildServiceProvider();

            _strategies = serviceProvider.GetRequiredService<IDictionary<OperationType, IOperationStrategy>>();
        }

        public class ExecuteMethod : OperationStrategyTests
        {
            public static IEnumerable<object[]> ExecuteTestData => new List<object[]>
            {
                new object[] { OperationType.Add, new decimal[] { 1m, 2m }, 3m },
                new object[] { OperationType.Add, new decimal[] { 0m, 0m }, 0m },
                new object[] { OperationType.Add, new decimal[] { -1m, 1m }, 0m },
                new object[] { OperationType.Subtract, new decimal[] { 5m, 3m }, 2m },
                new object[] { OperationType.Subtract, new decimal[] { 0m, 0m }, 0m },
                new object[] { OperationType.Subtract, new decimal[] { -1m, -1m }, 0m },
                new object[] { OperationType.Multiply, new decimal[] { 4m, 2m }, 8m },
                new object[] { OperationType.Multiply, new decimal[] { 0m, 5m }, 0m },
                new object[] { OperationType.Multiply, new decimal[] { -2m, 3m }, -6m },
                new object[] { OperationType.Divide, new decimal[] { 10m, 2m }, 5m },
                new object[] { OperationType.Divide, new decimal[] { 0m, 5m }, 0m },
                new object[] { OperationType.Divide, new decimal[] { 7m, 2m }, 3.5m }, // Decimal division
            };

            [Theory]
            [MemberData(nameof(ExecuteTestData))]
            public void Execute_WithValidInputs_ReturnsExpectedResult(OperationType operationType, decimal[] numbers, decimal expected)
            {
                // Arrange
                var strategy = _strategies[operationType];
                var decimalNumbers = numbers.ToList().AsReadOnly();

                // Act
                var result = strategy.Execute(decimalNumbers);

                // Assert
                result.Should().Be(expected, because: "{0} operation on {1} should result in {2}, but got {3}",
                    operationType, string.Join(", ", numbers), expected, result);
            }

        }

        public class GetFormulaMethod : OperationStrategyTests
        {
            public static IEnumerable<object[]> GetFormulaTestData => new List<object[]>
            {
                new object[] { OperationType.Add, new decimal[] { 1m, 2m }, "1+2" },
                new object[] { OperationType.Subtract, new decimal[] { 5m, 3m }, "5-3" },
                new object[] { OperationType.Multiply, new decimal[] { 4m, 2m }, "4*2" },
                new object[] { OperationType.Divide, new decimal[] { 10m, 2m }, "10/2" },
                new object[] { OperationType.Divide, new decimal[] { 7m, 2m }, "7/2" },
            };

            [Theory]
            [MemberData(nameof(GetFormulaTestData))]
            public void GetFormula_WithValidInputs_ReturnsExpectedFormula(OperationType operationType, decimal[] numbers, string expected)
            {
                // Arrange
                var strategy = _strategies[operationType];
                var decimalNumbers = numbers.ToList().AsReadOnly();

                // Act
                var result = strategy.GetFormula(decimalNumbers);

                // Assert
                result.Should().Be(expected, because: "the formula for {0} operation on {1} should be '{2}', but got '{3}'",
                    operationType, string.Join(", ", numbers), expected, result);
            }

            [Fact]
            public void GetFormula_WithEmptyList_ReturnsEmptyString()
            {
                // Arrange
                var numbers = new List<decimal>().AsReadOnly();

                // Act & Assert
                foreach (var strategy in _strategies.Values)
                {
                    var result = strategy.GetFormula(numbers);
                    result.Should().BeEmpty(because: "the formula for an empty list should be an empty string, but got '{0}'", result);
                }
            }
        }
    }
}
