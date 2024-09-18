using Calculon.Core.Common;
using Calculon.Core.Enums;
using Calculon.Core.Interfaces;
using Calculon.Core.Models;

namespace Calculon.ConsoleApp
{
    public class Application
    {
        private readonly IStringCalculator _calculator;
        private readonly IConsoleIO _console;

        public Application(IStringCalculator calculator, IConsoleIO console)
        {
            _calculator = calculator ?? throw new ArgumentNullException(nameof(calculator));
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }

        public bool Run()
        {
            DisplayWelcomeMessage();
            return RunCalculationLoop(OperationType.Add);
        }

        private void DisplayWelcomeMessage()
        {
            _console.WriteLine("Welcome to the String Calculator!");
            _console.WriteLine("Enter numbers to calculate, or 'q' to quit.");
            _console.WriteLine("Use '+', '-', '*', or '/' to change operations.");
        }

        private bool RunCalculationLoop(OperationType initialOperationType)
        {
            var state = new CalculationState(initialOperationType);
            while (true)
            {
                var result = PerformCalculation(state);
                if (result.IsExit)
                {
                    return true; // Successful exit
                }
                state = result.NewState;
            }
        }

        private (bool IsExit, CalculationState NewState) PerformCalculation(CalculationState state)
        {
            var inputResult = GetUserInput(state);
            return inputResult.Match(
                input => input.ToLower() == "q"
                    ? (true, state) // Exit
                    : ProcessUserInput(input, state),
                error =>
                {
                    _console.WriteLine($"Error: {error}");
                    return (false, state);
                }
            );
        }

        private Result<string> GetUserInput(CalculationState state)
        {
            _console.Write($"[{state.OperationType}] > ");
            var input = _console.ReadLine() ?? string.Empty;
            return ValidateInput(input);
        }

        private (bool IsExit, CalculationState NewState) ProcessUserInput(string input, CalculationState state)
        {
            return ProcessInput(input, state).Match(
                result =>
                {
                    _console.WriteLine($"Result: {result.Formula}");
                    return (false, state);
                },
                error =>
                {
                    if (error.StartsWith("Operation changed"))
                    {
                        _console.WriteLine(error);
                        return ParseOperationType(input).Match(
                            op => (false, state with { OperationType = op }),
                            _ => (false, state)
                        );
                    }
                    else
                    {
                        _console.WriteLine($"Error: {error}");
                        return (false, state);
                    }
                }
            );
        }

        private Result<CalculationResult> ProcessInput(string input, CalculationState state) =>
            ParseOperationType(input)
                .Match<Result<CalculationResult>>(
                    op => new Result<CalculationResult>.Error($"Operation changed to {op}"),
                    _ => _calculator.Calculate(new RawInput(input), state.OperationType)
                );

        private Result<string> ValidateInput(string input) =>
            string.IsNullOrWhiteSpace(input)
                ? new Result<string>.Error("Input cannot be empty")
                : new Result<string>.Ok(input);

        private Result<OperationType> ParseOperationType(string input) =>
            input switch
            {
                "+" => new Result<OperationType>.Ok(OperationType.Add),
                "-" => new Result<OperationType>.Ok(OperationType.Subtract),
                "*" => new Result<OperationType>.Ok(OperationType.Multiply),
                "/" => new Result<OperationType>.Ok(OperationType.Divide),
                _ => new Result<OperationType>.Error("Invalid operation")
            };

        private record CalculationState(OperationType OperationType);
    }
}