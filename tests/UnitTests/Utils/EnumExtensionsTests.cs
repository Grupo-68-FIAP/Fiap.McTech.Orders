using Domain.Utils.Attributes;
using Domain.Utils.Extensions;
using System.ComponentModel;

namespace Fiap.McTech.UnitTests.Utils
{
    public class EnumExtensionsTests
    {
        [Fact]
        public void ToDescription_ShouldReturnCorrectDescription()
        {
            // Arrange
            var status = PaymentStatus.Pending;

            // Act
            var description = status.ToDescription();

            // Assert
            Assert.Equal("Pending Payment", description);
        }

        [Fact]
        public void GetAlternateValue_ShouldReturnCorrectValue()
        {
            // Arrange
            var status = PaymentStatus.Completed;

            // Act
            var alternateValue = status.GetAlternateValue();

            // Assert
            Assert.Equal("C", alternateValue);
        }

        [Fact]
        public void GetAttribute_ShouldReturnCorrectAttribute()
        {
            // Arrange
            var status = PaymentStatus.Failed;

            // Act
            var attribute = status.GetAttribute<DescriptionAttribute>();

            // Assert
            Assert.NotNull(attribute);
            Assert.Equal("Failed Payment", attribute.Description);
        }

        [Fact]
        public void GetFromDescription_ShouldReturnCorrectEnumValue()
        {
            // Arrange
            var description = "Completed Payment";

            // Act
            var enumValue = EnumExtensions.GetFromDescription(description, typeof(PaymentStatus));

            // Assert
            Assert.Equal(2, enumValue);
        }

        [Fact]
        public void GetFromAlternateValue_ShouldReturnCorrectEnumValue()
        {
            // Arrange
            var alternateValue = "F";

            // Act
            var enumValue = EnumExtensions.GetFromAlternateValue(alternateValue, typeof(PaymentStatus));

            // Assert
            Assert.Equal(3, enumValue);
        }

        [Fact]
        public void GetFromDescription_ShouldReturnNegativeOneForInvalidDescription()
        {
            // Arrange
            var description = "Nonexistent Description";

            // Act
            var enumValue = EnumExtensions.GetFromDescription(description, typeof(PaymentStatus));

            // Assert
            Assert.Equal(-1, enumValue);
        }

        [Fact]
        public void GetFromAlternateValue_ShouldReturnNegativeOneForInvalidAlternateValue()
        {
            // Arrange
            var alternateValue = "X";

            // Act
            var enumValue = EnumExtensions.GetFromAlternateValue(alternateValue, typeof(PaymentStatus));

            // Assert
            Assert.Equal(-1, enumValue);
        }
    }

    public enum PaymentStatus
    {
        [Description("Pending Payment")]
        [AlternateValue("P")]
        Pending = 1,

        [Description("Completed Payment")]
        [AlternateValue("C")]
        Completed = 2,

        [Description("Failed Payment")]
        [AlternateValue("F")]
        Failed = 3
    }
}
