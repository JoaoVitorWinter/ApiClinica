namespace ApiClinica.Services.Exceptions;

public class ValidationErrorException : Exception
{
    public ValidationErrorException(string mensagem) : base(mensagem)
    {
    }
}
