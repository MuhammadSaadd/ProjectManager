namespace Domain.Primitives;

public sealed record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static Error NotFound(string name, object key) =>
        new("NotFound", $"{name} with key '{key}' was not found.");

    public static Error Validation(string message) =>
        new("Validation", message);

    public static Error Validation(IDictionary<string, string[]> errors) =>
        new("Validation", "One or more validation errors occurred.") { ValidationErrors = errors };

    public IDictionary<string, string[]>? ValidationErrors { get; init; }
}
