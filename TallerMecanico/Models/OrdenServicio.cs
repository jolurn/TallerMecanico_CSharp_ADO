namespace TallerMecanico.Models
{
    public class OrdenServicio
    {
        public int Id { get; set; }
        public int VehiculoId { get; set; }
        public int MecanicoId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Costo { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public DateTime FechaIngreso { get; set; } = DateTime.Now;
    }
}
