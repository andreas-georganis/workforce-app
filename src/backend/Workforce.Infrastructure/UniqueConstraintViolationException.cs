namespace Workforce.Infrastructure
{
    public class UniqueConstraintViolationException : Exception
    {
        public UniqueConstraintViolationException(string propertyName, string message) : base(message)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; }
    }
}