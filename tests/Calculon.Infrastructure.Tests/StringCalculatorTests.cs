using Calculon.Core.Common;
using Calculon.Core.Enums;
using Calculon.Core.Interfaces;
using Calculon.Core.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Calculon.Infrastructure.Tests
{
    public class StringCalculatorTests
    {
        private readonly IStringCalculator _calculator;

        public StringCalculatorTests()
        {
            var services = new ServiceCollection();
            services.AddStringCalculator(new CalculatorOptions());
            var serviceProvider = services.BuildServiceProvider();

            _calculator = serviceProvider.GetRequiredService<IStringCalculator>();
        }

        [Theory]
        [InlineData("1,2", OperationType.Add, 3, "1+2 = 3")]
        [InlineData("20", OperationType.Add, 20, "20+0 = 20")]
        [InlineData("5,tytyt", OperationType.Add, 5, "5+0 = 5")]
        [InlineData("5,3", OperationType.Subtract, 2, "5-3 = 2")]
        [InlineData("4,2", OperationType.Multiply, 8, "4*2 = 8")]
        [InlineData("10,2", OperationType.Divide, 5, "10/2 = 5")]
        public void Calculate_ValidInput_ReturnsExpectedResultAndFormula(string input, OperationType operationType, int expectedResult, string expectedFormula)
        {
            // Act
            var result = _calculator.Calculate(new RawInput(input), operationType);

            // Assert
            result.Should().BeOfType<Result<CalculationResult>.Ok>()
                .Which.Value.Should().Match<CalculationResult>(cr =>
                    cr.Result == expectedResult && cr.Formula == expectedFormula)
                .And.Subject.Should().BeEquivalentTo(new CalculationResult(expectedResult, expectedFormula),
                    because: $"Input: '{input}' with operation {operationType} should result in {expectedResult} with formula '{expectedFormula}'");
        }

    }
}