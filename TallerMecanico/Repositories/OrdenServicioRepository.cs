using Microsoft.EntityFrameworkCore;
using TallerMecanico.Data;
using TallerMecanico.Models;

namespace TallerMecanico.Repositories
{
    public class OrdenServicioRepository
    {
        private readonly TallerMecanicoContext _context;

        public OrdenServicioRepository(TallerMecanicoContext context) => _context = context;

        public async Task<List<OrdenServicio>> ObtenerTodosAsync() => await _context.OrdenesServicio.ToListAsync();

        public async Task<OrdenServicio?> ObtenerPorIdAsync(int id) => await _context.OrdenesServicio.FirstOrDefaultAsync(o => o.Id == id);

        public async Task<int> CrearAsync(OrdenServicio orden)
        {
            _context.OrdenesServicio.Add(orden);
            await _context.SaveChangesAsync();
            return orden.Id;
        }

        public async Task<bool> ActualizarAsync(OrdenServicio orden)
        {
            if (!await _context.OrdenesServicio.AnyAsync(o => o.Id == orden.Id)) return false;
            _context.OrdenesServicio.Update(orden);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var orden = await _context.OrdenesServicio.FindAsync(id);
            if (orden == null) return false;
            _context.OrdenesServicio.Remove(orden);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
