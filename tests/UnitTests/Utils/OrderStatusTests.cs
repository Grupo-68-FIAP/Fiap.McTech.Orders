using System.ComponentModel;
using Domain.Enums;

namespace Fiap.McTech.UnitTests.Utils
{
    public class OrderStatusTests
    {
        [Theory]
        [InlineData(OrderStatus.None, "Nenhum")]
        [InlineData(OrderStatus.WaitPayment, "Aguardando Pagamento")]
        [InlineData(OrderStatus.Received, "Recebido")]
        [InlineData(OrderStatus.InPreparation, "Em Preparo")]
        [InlineData(OrderStatus.Ready, "Pronto")]
        [InlineData(OrderStatus.Finished, "Finalizado")]
        [InlineData(OrderStatus.Canceled, "Cancelado")]
        public void OrderStatus_ShouldHaveCorrectDescriptions(OrderStatus status, string expectedDescription)
        {
            // Act
            var description = status.GetDescription();

            // Assert
            Assert.Equal(expectedDescription, description);
        }

        [Theory]
        [InlineData("Nenhum", OrderStatus.None)]
        [InlineData("Aguardando Pagamento", OrderStatus.WaitPayment)]
        [InlineData("Recebido", OrderStatus.Received)]
        [InlineData("Em Preparo", OrderStatus.InPreparation)]
        [InlineData("Pronto", OrderStatus.Ready)]
        [InlineData("Finalizado", OrderStatus.Finished)]
        [InlineData("Cancelado", OrderStatus.Canceled)]
        public void GetFromDescription_ShouldReturnCorrectEnumValue(string description, OrderStatus expectedStatus)
        {
            // Act
            var status = EnumExtensions.GetFromDescription(description, typeof(OrderStatus));

            // Assert
            Assert.Equal((int)expectedStatus, status);
        }

        [Fact]
        public void GetFromDescription_ShouldReturnNegativeOneForInvalidDescription()
        {
            // Arrange
            var description = "Invalid Status";

            // Act
            var status = EnumExtensions.GetFromDescription(description, typeof(OrderStatus));

            // Assert
            Assert.Equal(-1, status);
        }
    }

    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attribute = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false)
                                      .FirstOrDefault() as DescriptionAttribute;
            return attribute?.Description ?? value.ToString();
        }

        public static int GetFromDescription(string description, Type enumType)
        {
            foreach (var field in enumType.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null && attribute.Description.Equals(description, StringComparison.OrdinalIgnoreCase))
                {
                    return (int)field.GetValue(null);
                }
            }
            return -1;
        }
    }
}
