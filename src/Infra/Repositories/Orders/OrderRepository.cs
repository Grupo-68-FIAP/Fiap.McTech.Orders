﻿using Domain.Entities.Orders;
using Domain.Enums;
using Domain.Interfaces.Repositories.Orders;
using Infra.Context;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Infra.Repositories.Orders
{
    [ExcludeFromCodeCoverage]
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(DataContext context) : base(context) { }

        public async Task<Order> GetOrderByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Order>> GetOrderByStatusAsync(OrderStatus status)
        {
            return await _dbSet
                .Include(o => o.Items)
                .Where(o => o.Status == status)
                .ToListAsync();
        }

        public override async Task UpdateAsync(Order obj)
        {
            var original = await _db.Set<Order>()
                .AsNoTracking()
                .Include(o => o.Items)
                .FirstAsync(o => o.Id == obj.Id);

            foreach (var item in obj.Items)
            {
                if (original.Items.Any(oi => oi.Id == item.Id))
                {
                    _db.Entry(item).State = EntityState.Modified;
                }
                else
                {
                    _db.Entry(item).State = EntityState.Added;
                }
            }

            await base.UpdateAsync(obj);
        }

        public async Task<List<Order>> GetCurrrentOrders()
        {
            var showStatus = new List<OrderStatus> { OrderStatus.Received, OrderStatus.InPreparation, OrderStatus.Ready };

            return await _dbSet
                .Include(o => o.Items)
                .Where(o => showStatus.Contains(o.Status))
                .OrderByDescending(o => o.Status)
                .ThenByDescending(o => o.CreatedDate)
                .ToListAsync();
        }
    }
}
