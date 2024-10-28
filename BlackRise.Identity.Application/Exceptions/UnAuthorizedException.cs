namespace BlackRise.Identity.Application.Exceptions;

public class UnAuthorizedException : ApplicationException
{
    public UnAuthorizedException(string message):base(message)
    {
        
    }
}
