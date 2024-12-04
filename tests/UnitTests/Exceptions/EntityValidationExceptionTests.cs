using Domain.Exceptions;

namespace Fiap.McTech.UnitTests.Exceptions
{
    public class EntityValidationExceptionTests
    {
        [Fact]
        public void EntityValidationException_ShouldSetMessageCorrectly()
        {
            // Arrange
            var expectedMessage = "Test exception message";

            // Act
            var exception = new EntityValidationException(expectedMessage);

            // Assert
            Assert.Equal(expectedMessage, exception.Message);
        }

        [Fact]
        public void EntityValidationException_ShouldSetMessageAndInnerExceptionCorrectly()
        {
            // Arrange
            var expectedMessage = "Test exception message";
            var innerException = new Exception("Inner exception");

            // Act
            var exception = new EntityValidationException(expectedMessage, innerException);

            // Assert
            Assert.Equal(expectedMessage, exception.Message);
            Assert.Equal(innerException, exception.InnerException);
        }

        [Fact]
        public void EntityValidationException_ShouldInheritFromMcTechException()
        {
            // Arrange
            var exception = new EntityValidationException("Test");

            // Act & Assert
            Assert.IsAssignableFrom<McTechException>(exception);
        }
    }
}
