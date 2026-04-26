namespace kirfy_api.Shared.Exceptions;

public sealed class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException(string message) : base(message)
    {
    }
}
