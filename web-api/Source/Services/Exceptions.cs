namespace WebApi.Services;

/// <summary>
/// Should be raised after trying to insert a duplicate row in the database.
/// </summary>
public class DbDuplicateException : Exception
{
    public DbDuplicateException(string message) : base(message) {}
}
