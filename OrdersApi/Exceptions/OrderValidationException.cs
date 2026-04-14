namespace OrdersApi.Exceptions;

/// <summary>
/// Thrown by the service layer when request validation fails (AC2).
/// 
/// Carries a structured dictionary of field-level errors that the controller
/// serializes into a 400 Bad Request response body.
/// Each key is a field path (e.g., "customerId", "items[0].quantity") and
/// each value is an array of human-readable error messages for that field.
/// </summary>
public sealed class OrderValidationException : Exception
{
    /// <summary>
    /// Field-level validation errors.
    /// Key: field path (e.g., "customerId", "items[0].productCode").
    /// Value: one or more error messages for that field.
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public OrderValidationException(string message, Dictionary<string, string[]> errors)
        : base(message)
    {
        Errors = errors;
    }
}
