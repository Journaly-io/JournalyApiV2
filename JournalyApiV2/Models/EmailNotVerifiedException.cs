﻿namespace JournalyApiV2.Models;

public class EmailNotVerifiedException : Exception
{
    public EmailNotVerifiedException()
    {
    }

    public EmailNotVerifiedException(string message)
        : base(message)
    {
    }

    public EmailNotVerifiedException(string message, Exception inner)
        : base(message, inner)
    {
    }
}