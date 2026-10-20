using Microsoft.EntityFrameworkCore;
using TallerMecanico.Data;
using TallerMecanico.Models;

namespace TallerMecanico.Repositories
{
    public class MecanicoRepository
    {
        private readonly TallerMecanicoContext _context;

        public MecanicoRepository(TallerMecanicoContext context) => _context = context;

        public async Task<List<Mecanico>> ObtenerTodosAsync() => await _context.Mecanicos.ToListAsync();

        public async Task<Mecanico?> ObtenerPorIdAsync(int id) => await _context.Mecanicos.FirstOrDefaultAsync(m => m.Id == id);

        public async Task<int> CrearAsync(Mecanico mecanico)
        {
            _context.Mecanicos.Add(mecanico);
            await _context.SaveChangesAsync();
            return mecanico.Id;
        }

        public async Task<bool> ActualizarAsync(Mecanico mecanico)
        {
            if (!await _context.Mecanicos.AnyAsync(m => m.Id == mecanico.Id)) return false;
            _context.Mecanicos.Update(mecanico);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var mecanico = await _context.Mecanicos.FindAsync(id);
            if (mecanico == null) return false;
            _context.Mecanicos.Remove(mecanico);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
