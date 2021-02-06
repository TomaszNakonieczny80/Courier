﻿using System;
using System.Collections.Generic;
using System.Text;
using Courier.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace Courier.DataLayer
{
    public interface IParcelsDbContext : IDisposable
    {

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
            optionsBuilder.UseSqlServer(@"Server=.;Database=HW4_20210102_CourierOperations;Trusted_Connection=True");
        }
    }
}

