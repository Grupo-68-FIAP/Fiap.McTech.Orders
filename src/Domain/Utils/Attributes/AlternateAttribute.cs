namespace Domain.Utils.Attributes
{
    public class AlternateValueAttribute : Attribute
    {
        public string AlternateValue { get; protected set; }

        public AlternateValueAttribute(string value)
        {
            this.AlternateValue = value;
        }
    }
}
