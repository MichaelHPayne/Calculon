using Calculon.Core.Common;
using Calculon.Core.Models;
using System.Collections.Immutable;

namespace Calculon.Core.Extensions
{
    public static class ResultExtensions
    {
        // Implements the Bind (or flatMap) operation to enable chaining of operations that may fail.
        // This promotes a functional programming style, allowing for clean and readable error handling without deeply nested conditionals.
        public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> func) =>
            result is Result<TIn>.Ok ok ? func(ok.Value)
                                       : new Result<TOut>.Error((result as Result<TIn>.Error).Message);

        // Implements the Map operation to transform the successful result while preserving the error state.
        // This allows for the transformation of data within the Result without altering the error flow, facilitating a declarative coding style.
        public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> func) =>
            result is Result<TIn>.Ok ok ? new Result<TOut>.Ok(func(ok.Value))
                                       : new Result<TOut>.Error((result as Result<TIn>.Error).Message);

        // Converts a collection of strings into a Result containing NumberStrings.
        // By encapsulating the conversion logic, it ensures that the operation adheres to the Result-based error handling strategy,
        // and gracefully handles null inputs by defaulting to an empty immutable list, thus avoiding potential null reference issues downstream.
        public static Result<NumberStrings> ToNumberStrings(this IEnumerable<string> strings) =>
            new Result<NumberStrings>.Ok(new NumberStrings(strings?.ToImmutableList() ?? ImmutableList<string>.Empty));
    }
}
