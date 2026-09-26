namespace GitDashboard.Exceptions;

public class InvalidRepositoryException : Exception
{
    public InvalidRepositoryException(string message) : base(message) {}
}