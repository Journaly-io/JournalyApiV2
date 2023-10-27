namespace JournalyApiV2.Pipeline;

public class HttpAccessDeniedException : Exception
{
    public HttpAccessDeniedException()
    {
    }

    public HttpAccessDeniedException(string message)
        : base(message)
    {
    }

    public HttpAccessDeniedException(string message, Exception inner)
        : base(message, inner)
    {
    }
}

public class HttpNotFoundExeption : Exception
{
    public HttpNotFoundExeption()
    {
    }

    public HttpNotFoundExeption(string message)
        : base(message)
    {
    }

    public HttpNotFoundExeption(string message, Exception inner)
        : base(message, inner)
    {
    }
}

public class HttpBadRequestException : Exception
{
    public HttpBadRequestException()
    {
    }

    public HttpBadRequestException(string message)
        : base(message)
    {
    }

    public HttpBadRequestException(string message, Exception inner)
        : base(message, inner)
    {
    }
}