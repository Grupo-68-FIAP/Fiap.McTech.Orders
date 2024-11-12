namespace Domain.Exceptions
{
    public class DatabaseException : McTechException
    {
        public DatabaseException(string message)
            : base(message) { }

        public DatabaseException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
