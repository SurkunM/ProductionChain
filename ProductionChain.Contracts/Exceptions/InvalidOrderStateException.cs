namespace ProductionChain.Contracts.Exceptions;

public class InvalidOrderStateException : Exception
{
    public InvalidOrderStateException(string message) : base(message) { }

    public InvalidOrderStateException(string message, Exception exception) : base(message, exception) { }
}
