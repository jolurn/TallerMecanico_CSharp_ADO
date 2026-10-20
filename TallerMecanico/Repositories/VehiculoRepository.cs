using Microsoft.EntityFrameworkCore;
using TallerMecanico.Data;
using TallerMecanico.Models;

namespace TallerMecanico.Repositories
{
    public class VehiculoRepository
    {
        // Ya no usamos string _connectionString, ahora inyectamos el DbContext
        private readonly TallerMecanicoContext _context;

        public VehiculoRepository(TallerMecanicoContext context)
        {
            _context = context;
        }

        // GET: api/vehiculos
        public async Task<List<Vehiculo>> ObtenerTodosAsync()
        {
            // El ORM traduce esto a: SELECT * FROM Vehiculos
            return await _context.Vehiculos.ToListAsync();
        }

        // GET: api/vehiculos/cliente/3
        public async Task<List<Vehiculo>> ObtenerPorClienteAsync(int clienteId)
        {
            // El ORM traduce esto a: SELECT * FROM Vehiculos WHERE ClienteId = @clienteId
            return await _context.Vehiculos
                .Where(v => v.ClienteId == clienteId)
                .ToListAsync();
        }

        // GET: api/vehiculos/5
        public async Task<Vehiculo?> ObtenerPorIdAsync(int id)
        {
            // El ORM traduce esto a: SELECT TOP 1 * FROM Vehiculos WHERE Id = @id
            return await _context.Vehiculos
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        // POST: api/vehiculos
        public async Task<int> CrearAsync(Vehiculo vehiculo)
        {
            // 1. Preparamos el INSERT en memoria
            _context.Vehiculos.Add(vehiculo);

            // 2. Ejecutamos el INSERT real en SQL Server
            await _context.SaveChangesAsync();

            // 3. El ORM asigna automáticamente el ID generado a la propiedad vehiculo.Id
            return vehiculo.Id;
        }

        // PUT: api/vehiculos/5
        public async Task<bool> ActualizarAsync(Vehiculo vehiculo)
        {
            // Verificamos si el vehículo realmente existe antes de intentar actualizarlo
            bool existe = await _context.Vehiculos.AnyAsync(v => v.Id == vehiculo.Id);

            if (!existe)
                return false;

            // Le decimos al ORM que este objeto fue modificado (UPDATE)
            _context.Vehiculos.Update(vehiculo);

            // Guardamos los cambios
            await _context.SaveChangesAsync();

            return true;
        }

        // DELETE: api/vehiculos/5
        public async Task<bool> EliminarAsync(int id)
        {
            // Buscamos el vehículo por su ID
            var vehiculo = await _context.Vehiculos.FindAsync(id);

            // Si no existe, retornamos false (el controller responderá 404)
            if (vehiculo == null)
                return false;

            // Le decimos al ORM que lo borre (DELETE)
            _context.Vehiculos.Remove(vehiculo);

            // Ejecutamos el DELETE en la base de datos
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
