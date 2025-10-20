namespace APITemplate.Business.DTOs.Auth
{
    /// <summary>
    /// DTO para exponer el perfil de un usuario sin información sensible
    /// </summary>
    public class UserProfileDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int IdRol { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
