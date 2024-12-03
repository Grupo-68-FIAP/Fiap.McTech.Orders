using Domain.Utils.Attributes;
using System.Reflection;

namespace Fiap.McTech.UnitTests.Utils
{
    public class AlternateAttributeTests
    {
        [Fact]
        public void AlternateValueAttribute_ShouldSetAlternateValueCorrectly()
        {
            // Arrange
            var expectedValue = "Test Value";

            // Act
            var attribute = new AlternateValueAttribute(expectedValue);

            // Assert
            Assert.Equal(expectedValue, attribute.AlternateValue);
        }

        [Fact]
        public void AlternateValueAttribute_ShouldBeApplicableToFieldsOnly()
        {
            // Arrange
            var attributeUsage = typeof(AlternateValueAttribute)
                .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
                .FirstOrDefault() as AttributeUsageAttribute;

            // Assert
            Assert.NotNull(attributeUsage);
            Assert.True(attributeUsage.ValidOn.HasFlag(AttributeTargets.Field));
            Assert.False(attributeUsage.ValidOn.HasFlag(AttributeTargets.Class));
            Assert.False(attributeUsage.ValidOn.HasFlag(AttributeTargets.Method));
        }

        [Theory]
        [InlineData("AltValue1", TestAlternateValue.Value1)]
        [InlineData("AltValue2", TestAlternateValue.Value2)]
        public void AlternateValueAttribute_ShouldRetrieveValueFromEnumField(string expectedValue, TestAlternateValue value)
        {
            // Arrange
            Type Type = value.GetType();
            FieldInfo FieldInfo = Type.GetField(value.ToString());
            
            // Act
            var attribute = FieldInfo.GetCustomAttributes(typeof(AlternateValueAttribute), false)
                .FirstOrDefault() as AlternateValueAttribute;

            // Assert
            Assert.NotNull(attribute);
            Assert.Equal(expectedValue, attribute.AlternateValue);
        }

        // Helper enum for testing
        public enum TestAlternateValue
        {
            [AlternateValue("AltValue1")]
            Value1,

            [AlternateValue("AltValue2")]
            Value2
        }
    }
}
