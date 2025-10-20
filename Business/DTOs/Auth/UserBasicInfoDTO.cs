namespace APITemplate.Business.DTOs.Auth
{
    public class UserBasicInfoDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int IdRol { get; set; }
    }
}
