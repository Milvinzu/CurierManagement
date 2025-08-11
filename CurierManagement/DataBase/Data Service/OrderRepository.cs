using CurierManagement.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurierManagement.DataBase.Data_Service
{
    public class OrderRepository : BaseRepository<Order>
    {
        public OrderRepository(DbContext context) : base(context)
        {
        }

        public async Task<List<Order>> GetOrdersAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(o => o.DeliveryPackage)
                .Include(o => o.Courier)
                .ToListAsync(cancellationToken);
        }

        public async Task AddOrder(Order order, CancellationToken cancellationToken = default)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));
            await _dbSet.AddAsync(order, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateOrder(Order order, CancellationToken cancellationToken = default)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));
            _dbSet.Update(order);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteOrder(int id, CancellationToken cancellationToken = default)
        {
            var order = await GetByIdAsync(id, cancellationToken);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {id} not found.");
            _dbSet.Remove(order);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
