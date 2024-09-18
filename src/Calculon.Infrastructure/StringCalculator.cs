using Calculon.Core.Common;
using Calculon.Core.Enums;
using Calculon.Core.Extensions;
using Calculon.Core.Interfaces;
using Calculon.Core.Models;
using System.Collections.Immutable;
using System.Globalization;

namespace Calculon.Infrastructure
{
    public class StringCalculator : IStringCalculator
    {
        private const int MaxDecimalPlaces = 3;
        private readonly IEnumerable<IDelimiterStrategy> _delimiterStrategies;
        private readonly IDictionary<OperationType, IOperationStrategy> _operationStrategies;
        private readonly CalculatorOptions _options;

        // Constructor injects dependencies to adhere to the Dependency Inversion Principle,
        // allowing for easy extension and testing of delimiter and operation strategies.
        public StringCalculator(
            IEnumerable<IDelimiterStrategy> delimiterStrategies,
            IDictionary<OperationType, IOperationStrategy> operationStrategies,
            CalculatorOptions options)
        {
            _delimiterStrategies = delimiterStrategies;
            _operationStrategies = operationStrategies;
            _options = options;
        }

        // The Calculate method orchestrates the calculation process using a functional pipeline,
        // enhancing readability and maintainability by chaining operations that can fail gracefully.
        public Result<CalculationResult> Calculate(RawInput input, OperationType operationType) =>
            ProcessInput(input)
                .Bind(ApplyDelimiterStrategy)
                .Bind(ParseNumbers)
                .Bind(ValidateNumbers)
                .Bind(numbers => ExecuteOperation(numbers, operationType))
                .Map(result => BuildResult(result, operationType));

        // Ensures that empty or whitespace inputs default to "0", preventing potential parsing issues downstream.
        private Result<ProcessedInput> ProcessInput(RawInput input) =>
            string.IsNullOrWhiteSpace(input.Value)
                ? new Result<ProcessedInput>.Ok(new ProcessedInput("0"))
                : new Result<ProcessedInput>.Ok(new ProcessedInput(input.Value.Trim()));

        // Selects the first delimiter strategy that can handle the input,
        // promoting flexibility in supporting various delimiter formats.
        private Result<NumberStrings> ApplyDelimiterStrategy(ProcessedInput input) =>
            _delimiterStrategies
                .FirstOrDefault(s => s.CanHandle(input.Value))
                ?.Split(input.Value)
                .ToNumberStrings()
                ?? new Result<NumberStrings>.Error("No suitable delimiter strategy found");

        // Parses the number strings into decimals, defaulting to 0 for invalid or missing entries.
        // If only one number is provided, pads with a zero to ensure at least two numbers for binary operations.
        private Result<ParsedNumbers> ParseNumbers(NumberStrings numbers)
        {
            var parsedNumbers = numbers.Values
                .Select(number =>
                    string.IsNullOrWhiteSpace(number) ? 0m :
                    decimal.TryParse(number, out var result) ? result : 0m)
                .ToList();

            // If only one number is present, pad with a zero
            if (parsedNumbers.Count == 1)
            {
                parsedNumbers.Add(0m);
            }

            return new Result<ParsedNumbers>.Ok(new ParsedNumbers(parsedNumbers.ToImmutableList()));
        }

        // Validates that there are no negative numbers if the configuration disallows them,
        // enhancing the robustness by enforcing business rules.
        private Result<ParsedNumbers> ValidateNumbers(ParsedNumbers numbers)
        {
            var negatives = numbers.Values.Where(n => n < 0m).ToImmutableList();
            return negatives.Any() && !_options.AllowNegativeNumbers
                ? new Result<ParsedNumbers>.Error($"Negative numbers not allowed: {string.Join(", ", negatives)}")
                : new Result<ParsedNumbers>.Ok(numbers);
        }

        // Executes the specified operation using the appropriate strategy.
        // Specifically handles division by zero to prevent runtime exceptions,
        // and ensures only supported operations are processed.
        private Result<(decimal Result, IReadOnlyList<decimal> Numbers)> ExecuteOperation(ParsedNumbers numbers, OperationType operationType) =>
            _operationStrategies.TryGetValue(operationType, out var strategy)
                ? (operationType == OperationType.Divide && numbers.Values.Skip(1).Contains(0m))
                    ? new Result<(decimal, IReadOnlyList<decimal>)>.Error("Cannot divide by zero")
                    : new Result<(decimal, IReadOnlyList<decimal>)>.Ok((strategy.Execute(numbers.Values), numbers.Values))
                : new Result<(decimal, IReadOnlyList<decimal>)>.Error($"Unsupported operation: {operationType}");

        // Builds the final calculation result, formatting the output for user-friendly display
        // and generating the formula to provide clarity on the performed operation.
        private CalculationResult BuildResult((decimal Result, IReadOnlyList<decimal> Numbers) data, OperationType operationType)
        {
            var strategy = _operationStrategies[operationType];
            var formattedResult = FormatResult(data.Result);
            var formula = $"{strategy.GetFormula(data.Numbers)} = {formattedResult}";
            return new CalculationResult(data.Result, formula);
        }

        // Formats the decimal result to a string, optimizing for integer representation when applicable
        // and limiting decimal places to improve readability without losing significant precision.
        private string FormatResult(decimal result)
        {
            if (result == Math.Floor(result))
            {
                // Avoids unnecessary decimal points for whole numbers
                return ((int)result).ToString(CultureInfo.InvariantCulture);
            }
            // Limits to a maximum number of decimal places and removes trailing zeros for cleaner display
            return result.ToString($"F{MaxDecimalPlaces}", CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.');
        }
    }
}
