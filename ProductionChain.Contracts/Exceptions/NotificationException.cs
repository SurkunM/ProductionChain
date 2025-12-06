namespace ProductionChain.Contracts.Exceptions;

public class NotificationException : Exception
{
    public NotificationException(string message) : base(message) { }

    public NotificationException(string message, Exception exception) : base(message, exception) { }
}
