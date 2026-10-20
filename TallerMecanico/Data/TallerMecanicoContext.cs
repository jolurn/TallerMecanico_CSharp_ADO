using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;

namespace TallerMecanico.Data 
{
    public class TallerMecanicoContext : DbContext
    {
        public TallerMecanicoContext(DbContextOptions<TallerMecanicoContext> options)
           : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Vehiculo> Vehiculos { get; set; }

        // 👇 NUEVOS MODELOS
        public DbSet<Mecanico> Mecanicos { get; set; }
        public DbSet<Repuesto> Repuestos { get; set; }
        public DbSet<OrdenServicio> OrdenesServicio { get; set; }
    }
}
