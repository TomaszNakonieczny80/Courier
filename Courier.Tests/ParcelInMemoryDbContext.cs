using System;
using System.Collections.Generic;
using System.Text;
using Courier.DataLayer;
using Courier.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace Courier.Tests
{
    public class ParcelInMemoryDbContext : DbContext, IParcelsDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Parcel> Parcels { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<CarParcel> CarParcels { get; set; }
        public DbSet<Shipment> Shipments { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("Parcels");
        }
    }
}
