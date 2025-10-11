using APITemplate.Models;

namespace APITemplate.Business.DTOs.Barrios
{
    public class LocalidadesDTO
    {
        public string Ciudad { get; set; } = string.Empty;
        public List<BARRIOS> Barrios { get; set; } = new();
    }
}
