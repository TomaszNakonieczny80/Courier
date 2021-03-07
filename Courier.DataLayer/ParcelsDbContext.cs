using System;
using System.Threading;
using System.Threading.Tasks;
using Courier.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace Courier.DataLayer
{
    public interface IParcelsDbContext : IDisposable
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Parcel> Parcels { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<CarParcel> CarParcels { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
    }
    public class ParcelsDbContext : DbContext, IParcelsDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Parcel> Parcels { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<CarParcel> CarParcels { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.;Database=HW6_20210301_CourierOperations;Trusted_Connection=True");
        }
    }
}

