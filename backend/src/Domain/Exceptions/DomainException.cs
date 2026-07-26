namespace Domain.Exceptions;

public abstract class DomainException(string message) : Exception(message);

public sealed class NotFoundException(string name, object key)
    : DomainException($"{name} with key '{key}' was not found.");

public sealed class ValidationException(string message) : DomainException(message);
