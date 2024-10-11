using System;

public class RequestTimeoutException : Exception
{
    public RequestTimeoutException() : base("The request timed out.") { }

    public RequestTimeoutException(string message) : base(message) { }

    public RequestTimeoutException(string message, Exception inner) : base(message, inner) { }
}
