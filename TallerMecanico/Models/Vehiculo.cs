namespace TallerMecanico.Models
{
    public class Vehiculo
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }          // FK al cliente dueño
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int Anio { get; set; }
        public string Placa { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
    }
}
