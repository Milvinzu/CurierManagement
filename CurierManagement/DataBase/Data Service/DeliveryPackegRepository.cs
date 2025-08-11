using CurierManagement.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurierManagement.DataBase.Data_Service
{
    public class DeliveryPackegRepository : BaseRepository<DeliveryPackage>
    {
        public DeliveryPackegRepository(DbContext context) : base(context)
        {
        }

        public async Task<List<DeliveryPackage>> GetDeliveryPackagesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(dp => dp.Orders)
                .Include(dp => dp.Courier)
                .Include(dp => dp.Route)
                .ToListAsync(cancellationToken);
        }

        public async Task AddDeliveryPackeg(DeliveryPackage deliveryPackage, CancellationToken cancellationToken = default)
        {
            if (deliveryPackage == null)
                throw new ArgumentNullException(nameof(deliveryPackage));
            await _dbSet.AddAsync(deliveryPackage, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateDeliveryPackeg(DeliveryPackage deliveryPackage, CancellationToken cancellationToken = default)
        {
            if (deliveryPackage == null)
                throw new ArgumentNullException(nameof(deliveryPackage));
            _dbSet.Update(deliveryPackage);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteDeliveryPackeg(int id, CancellationToken cancellationToken = default)
        {
            var deliveryPackage = await GetByIdAsync(id, cancellationToken);
            if (deliveryPackage == null)
                throw new KeyNotFoundException($"DeliveryPackage with ID {id} not found.");
            _dbSet.Remove(deliveryPackage);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
