namespace WebApi.Services;

/// <summary>
/// Should be raised after trying to insert a duplicate row in the database.
/// </summary>
public class DbDuplicateException : Exception
{
    public DbDuplicateException(string message) : base(message) {}
}

/// <summary>
/// Should be raised after a user tries the access an entity without to correct permission.
/// </summary>
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) {}
}
