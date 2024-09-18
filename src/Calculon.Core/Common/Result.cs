namespace Calculon.Core.Common
{
    // Represents the outcome of an operation, either successful (Ok) or failed (Error).
    public abstract class Result<T>
    {
        // Represents a successful result containing a value.
        public sealed class Ok : Result<T>
        {
            public T Value { get; }

            // Ensures that a successful result always has a value.
            public Ok(T value) => Value = value;
        }

        // Represents a failed result containing an error message.
        public sealed class Error : Result<T>
        {
            public string Message { get; }

            // Ensures that an error result always has a message.
            public Error(string message) => Message = message;
        }

        // Facilitates handling both success and error cases, promoting explicit outcome management.
        public TResult Match<TResult>(Func<T, TResult> onOk, Func<string, TResult> onError) =>
            this switch
            {
                Ok ok => onOk(ok.Value),
                Error error => onError(error.Message),
                _ => throw new InvalidOperationException("Invalid Result type")
            };
    }
}
