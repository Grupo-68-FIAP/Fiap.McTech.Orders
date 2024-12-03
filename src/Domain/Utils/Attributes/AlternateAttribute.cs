namespace Domain.Utils.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Delegate)]
    public class AlternateValueAttribute : Attribute
    {
        public string AlternateValue { get; protected set; }

        public AlternateValueAttribute(string value)
        {
            this.AlternateValue = value;
        }
    }
}
