namespace Webhooks.Engine.Exceptions;

public sealed class InvalidEventException : Exception
{
    public InvalidEventException(string eventJson)
        : base($"Could not deserialize event. Raw data: {eventJson}")
    {
    }
}
