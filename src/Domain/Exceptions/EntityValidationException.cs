namespace Domain.Exceptions
{
    public class EntityValidationException : McTechException
    {
        public EntityValidationException(string message)
            : base(message) { }

        public EntityValidationException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
