using CurierManagement.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurierManagement.DataBase.Data_Service
{
    public class CourierRepository : BaseRepository<Courier>
    {
        public CourierRepository(DbContext context) : base(context)
        {
        }

        public async Task<Courier?> GetByIdWithPackagesAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(c => c.Packages)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Courier>> GetAllWithPackagesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(c => c.Packages)
                .ToListAsync(cancellationToken);
        }

        public async Task AddCourier(Courier courier, CancellationToken cancellationToken = default)
        {
            if (courier == null)
                throw new ArgumentNullException(nameof(courier));
            await _dbSet.AddAsync(courier, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateCourier(Courier courier, CancellationToken cancellationToken = default)
        {
            if (courier == null)
                throw new ArgumentNullException(nameof(courier));
            _dbSet.Update(courier);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteCourier(int id, CancellationToken cancellationToken = default)
        {
            var courier = await GetByIdAsync(id, cancellationToken);
            if (courier == null)
                throw new KeyNotFoundException($"Courier with ID {id} not found.");
            _dbSet.Remove(courier);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
