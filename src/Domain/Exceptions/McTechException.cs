namespace Domain.Exceptions
{ 
    public abstract class McTechException : Exception
    {
        protected McTechException(string message)
            : base(message) { }

        protected McTechException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
