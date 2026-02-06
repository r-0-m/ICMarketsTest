namespace ICMarketsTest.Core.Exceptions;

/// <summary>
/// Raised when an external dependency fails.
/// </summary>
public sealed class ExternalServiceException : Exception
{
    public ExternalServiceException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
