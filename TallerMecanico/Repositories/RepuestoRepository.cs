using Microsoft.EntityFrameworkCore;
using TallerMecanico.Data;
using TallerMecanico.Models;

namespace TallerMecanico.Repositories
{
    public class RepuestoRepository
    {
        private readonly TallerMecanicoContext _context;

        public RepuestoRepository(TallerMecanicoContext context) => _context = context;

        public async Task<List<Repuesto>> ObtenerTodosAsync() => await _context.Repuestos.ToListAsync();

        public async Task<Repuesto?> ObtenerPorIdAsync(int id) => await _context.Repuestos.FirstOrDefaultAsync(r => r.Id == id);

        public async Task<int> CrearAsync(Repuesto repuesto)
        {
            _context.Repuestos.Add(repuesto);
            await _context.SaveChangesAsync();
            return repuesto.Id;
        }

        public async Task<bool> ActualizarAsync(Repuesto repuesto)
        {
            if (!await _context.Repuestos.AnyAsync(r => r.Id == repuesto.Id)) return false;
            _context.Repuestos.Update(repuesto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var repuesto = await _context.Repuestos.FindAsync(id);
            if (repuesto == null) return false;
            _context.Repuestos.Remove(repuesto);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
