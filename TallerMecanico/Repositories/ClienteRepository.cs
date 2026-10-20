using Microsoft.EntityFrameworkCore;
using TallerMecanico.Data;
using TallerMecanico.Models;

namespace TallerMecanico.Repositories
{
    public class ClienteRepository
    {
        private readonly TallerMecanicoContext _context;

        // Inyectamos el DbContext en lugar de la cadena de conexión
        public ClienteRepository(TallerMecanicoContext context)
        {
            _context = context;
        }

        // GET: Todos los clientes
        public async Task<List<Cliente>> ObtenerTodosAsync()
        {
            // Equivale a: SELECT * FROM Clientes
            return await _context.Clientes.ToListAsync();
        }

        // GET: Cliente por Id
        public async Task<Cliente?> ObtenerPorIdAsync(int id)
        {
            // Equivale a: SELECT * FROM Clientes WHERE Id = @id
            return await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
        }

        // POST: Crear cliente
        public async Task<int> CrearAsync(Cliente cliente)
        {
            // 1. Lo agregamos al contexto (en memoria)
            _context.Clientes.Add(cliente);

            // 2. Guardamos los cambios en la BD (aquí se ejecuta el INSERT)
            await _context.SaveChangesAsync();

            // 3. EF Core actualiza automáticamente la propiedad Id del objeto cliente
            return cliente.Id;
        }

        // PUT: Actualizar cliente
        public async Task<bool> ActualizarAsync(Cliente cliente)
        {
            // Buscamos si existe en la BD
            var existe = await _context.Clientes.AnyAsync(c => c.Id == cliente.Id);
            if (!existe) return false;

            // Le decimos al ORM que modifique este registro
            _context.Clientes.Update(cliente);

            // Guardamos los cambios
            await _context.SaveChangesAsync();
            return true;
        }

        // DELETE: Eliminar cliente
        public async Task<bool> EliminarAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return false;

            // Le decimos al ORM que lo borre
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
