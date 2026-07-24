namespace Workforce.Domain.Exceptions;


public class WorkforceDomainException : Exception
{
    public WorkforceDomainException()
    { }

    public WorkforceDomainException(string message)
        : base(message)
    { }

    public WorkforceDomainException(string message, Exception innerException)
        : base(message, innerException)
    { }
}