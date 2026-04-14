namespace OrdersApi.Exceptions;

/// <summary>
/// Thrown by the repository layer when a database operation fails (AC3, AC5).
/// 
/// IsConflict = true  → FK or CHECK constraint violation → controller returns 409 Conflict.
/// IsConflict = false → unexpected SQL or infrastructure error → controller returns 500.
/// 
/// Detail is logged internally by the repository and passed here for optional controller
/// logging; it is never included in the outbound HTTP response body.
/// </summary>
public sealed class OrderPersistenceException : Exception
{
    /// <summary>
    /// True when the error is a known DB constraint violation (SqlException error 547).
    /// False for all other unexpected database errors.
    /// </summary>
    public bool IsConflict { get; }

    /// <summary>
    /// Internal diagnostic detail (SQL error number, message, etc.).
    /// Must not be sent to the client — log only.
    /// </summary>
    public string Detail { get; }

    public OrderPersistenceException(string message, bool isConflict, string detail, Exception? innerException = null)
        : base(message, innerException)
    {
        IsConflict = isConflict;
        Detail = detail;
    }
}
