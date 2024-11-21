using Domain.Entities.Orders;
using Infra.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityMapper
{
    internal class OrderMap : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable(nameof(DataContext.Orders));

            builder.HasKey(order => order.Id);

            builder.Property(order => order.TotalAmount)
                .HasPrecision(14, 2);

            builder.Property(order => order.Status)
                .HasConversion<int>();

            builder.Navigation(cart => cart.Items).UsePropertyAccessMode(PropertyAccessMode.Property);
        }
    }
}
