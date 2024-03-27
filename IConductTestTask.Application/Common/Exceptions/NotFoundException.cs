namespace IConductTestTask.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message)
        : base(message)
    {
    }

    public NotFoundException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }

    public NotFoundException(
        Guid key,
        string objectName)
        : this(key.ToString(), objectName)
    {
    }

    public NotFoundException(
        int key,
        string objectName)
        : this(key.ToString(), objectName)
    {
    }

    public NotFoundException(
        short key,
        string objectName)
        : this(key.ToString(), objectName)
    {
    }

    public NotFoundException(
        string key,
        string objectName)
        : base($"Queried object {objectName} was not found, Key: {key}")
    {
    }

    public NotFoundException(
        object filter,
        string objectName)
        : base($"Queried object {objectName} was not found, Filter: {filter}")
    {
    }
}

