namespace LabOpsDesk.Api.Domain;

public sealed class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}
