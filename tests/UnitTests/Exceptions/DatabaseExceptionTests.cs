using Domain.Exceptions;

namespace Fiap.McTech.UnitTests.Exceptions
{
    public class DatabaseExceptionTests
    {
        [Fact]
        public void DatabaseException_ShouldSetMessageCorrectly()
        {
            // Arrange
            var expectedMessage = "Test exception message";

            // Act
            var exception = new DatabaseException(expectedMessage);

            // Assert
            Assert.Equal(expectedMessage, exception.Message);
        }

        [Fact]
        public void DatabaseException_ShouldSetMessageAndInnerExceptionCorrectly()
        {
            // Arrange
            var expectedMessage = "Test exception message";
            var innerException = new Exception("Inner exception");

            // Act
            var exception = new DatabaseException(expectedMessage, innerException);

            // Assert
            Assert.Equal(expectedMessage, exception.Message);
            Assert.Equal(innerException, exception.InnerException);
        }

        [Fact]
        public void DatabaseException_ShouldInheritFromMcTechException()
        {
            // Arrange
            var exception = new DatabaseException("Test");

            // Act & Assert
            Assert.IsAssignableFrom<McTechException>(exception);
        }
    }
}
