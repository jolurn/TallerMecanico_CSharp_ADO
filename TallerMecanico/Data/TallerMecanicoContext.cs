using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;

namespace TallerMecanico.Data 
{
    public class TallerMecanicoContext : DbContext
    {
        // Le pasamos las opciones (como la cadena de conexión) al constructor base
        public TallerMecanicoContext(DbContextOptions<TallerMecanicoContext> options)
            : base(options)
        {
        }

        // DbSet representa una TABLA en la base de datos.
        // "Clientes" será la tabla Clientes y trabajará con la clase Cliente.
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Vehiculo> Vehiculos { get; set; }
    }
}
