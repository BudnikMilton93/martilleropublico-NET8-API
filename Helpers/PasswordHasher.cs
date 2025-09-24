namespace APITemplate.Helpers
{
    public static class PasswordHasher
    {
        /// <summary>
        /// Toma una contraseña en texto plano.
        /// Devuelve la versión encriptada usando bcrypt.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Compara una contraseña ingresada por el usuario con el hash guardado en la base de datos.
        /// Devuelve true si coinciden.
        /// </summary>
        /// <param name="inputPassword"></param>
        /// <param name="storedHash"></param>
        /// <returns></returns>
        public static bool VerifyPassword(string inputPassword, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(inputPassword, storedHash);
        }
    }
}
