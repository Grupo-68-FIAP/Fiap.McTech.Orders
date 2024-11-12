namespace Domain.Exceptions
{
    public class EntityNotFoundException : McTechException
    {
        public EntityNotFoundException(string message)
            : base(message) { }

        public EntityNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
