using ECommerceAPI.Helpers;
using Xunit;

namespace ECommerceAPI.Tests.Helpers
{
    public class PasswordHasherTests
    {
        /// <summary>
        /// Verifica que se genera un hash no vacío y distinto del texto original.
        /// </summary>
        [Fact]
        public void HashPassword_ShouldReturnHashedValue()
        {
            // Arrange
            string password = "123456";

            // Act
            string hash = PasswordHasher.HashPassword(password);

            // Assert
            Assert.False(string.IsNullOrEmpty(hash));
            Assert.NotEqual(password, hash); // Ensure it's not plain text
        }

        /// <summary>
        /// Valida que una contraseña correcta sea verificada exitosamente.
        /// </summary>
        [Fact]
        public void VerifyPassword_ShouldReturnTrue_WhenPasswordIsCorrect()
        {
            // Arrange
            string password = "mypassword";
            string hash = PasswordHasher.HashPassword(password);

            // Act
            bool result = PasswordHasher.VerifyPassword(password, hash);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Valida que una contraseña incorrecta no sea verificada.
        /// </summary>
        [Fact]
        public void VerifyPassword_ShouldReturnFalse_WhenPasswordIsIncorrect()
        {
            // Arrange
            string correctPassword = "mypassword";
            string incorrectPassword = "wrongpassword";
            string hash = PasswordHasher.HashPassword(correctPassword);

            // Act
            bool result = PasswordHasher.VerifyPassword(incorrectPassword, hash);

            // Assert
            Assert.False(result);
        }
    }
}
