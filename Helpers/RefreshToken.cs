namespace APITemplate.Helpers
{
    public class RefreshToken
    {
        public string Token { get; set; }           // El string aleatorio del refresh token
        public DateTime Expires { get; set; }       // Fecha y hora de expiración
        public DateTime Created { get; set; }       // Cuándo fue creado
        public string CreatedByIp { get; set; }     // IP opcional (para trazabilidad)
        public bool IsExpired => DateTime.UtcNow >= Expires; // Propiedad calculada opcional
    }
}
