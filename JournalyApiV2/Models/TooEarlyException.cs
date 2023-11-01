namespace JournalyApiV2.Models;

public class TooEarlyException : Exception
{
    public TooEarlyException()
    {
    }

    public TooEarlyException(string message)
        : base(message)
    {
    }

    public TooEarlyException(string message, Exception inner)
        : base(message, inner)
    {
    }
}