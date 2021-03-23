﻿// <auto-generated />
using System;
using Courier.DataLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Courier.DataLayer.Migrations
{
    [DbContext(typeof(ParcelsDbContext))]
    partial class ParcelsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.3");

            modelBuilder.Entity("Courier.DataLayer.Models.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HouseNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Latitude")
                        .HasColumnType("float");

                    b.Property<double>("Longitude")
                        .HasColumnType("float");

                    b.Property<string>("Street")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ZipCode")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("Courier.DataLayer.Models.Car", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<bool>("Available")
                        .HasColumnType("bit");

                    b.Property<long>("AverageSpeed")
                        .HasColumnType("bigint");

                    b.Property<string>("Brand")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("Capacity")
                        .HasColumnType("bigint");

                    b.Property<double>("Latitude")
                        .HasColumnType("float");

                    b.Property<double>("Longitude")
                        .HasColumnType("float");

                    b.Property<string>("Model")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RegistrationNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Cars");
                });

            modelBuilder.Entity("Courier.DataLayer.Models.CarParcel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<long>("AvailableCapcity")
                        .HasColumnType("bigint");

                    b.Property<double>("AvailableTime")
                        .HasColumnType("float");

                    b.Property<long>("AverageSpeed")
                        .HasColumnType("bigint");

                    b.Property<int>("CarId")
                        .HasColumnType("int");

                    b.Property<double>("DistanceToParcel")
                        .HasColumnType("float");

                    b.Property<double>("DistanceToRecipient")
                        .HasColumnType("float");

                    b.Property<bool>("Full")
                        .HasColumnType("bit");

                    b.Property<int>("ParcelId")
                        .HasColumnType("int");

                    b.Property<int>("ParcelSize")
                        .HasColumnType("int");

                    b.Property<bool>("Posted")
                        .HasColumnType("bit");

                    b.Property<double>("TotalTravelTime")
                        .HasColumnType("float");

                    b.Property<double>("TravelTimeToParcel")
                        .HasColumnType("float");

                    b.Property<double>("TravelTimeToRecipient")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("CarParcels");
                });

            modelBuilder.Entity("Courier.DataLayer.Models.Parcel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("CarId")
                        .HasColumnType("int");

                    b.Property<bool>("DeliveredAutomatically")
                        .HasColumnType("bit");

                    b.Property<double>("ParcelLatitude")
                        .HasColumnType("float");

                    b.Property<double>("ParcelLongitude")
                        .HasColumnType("float");

                    b.Property<Guid>("ParcelNumber")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ParcelSize")
                        .HasColumnType("int");

                    b.Property<int>("ParcelStatus")
                        .HasColumnType("int");

                    b.Property<int?>("RecipientId")
                        .HasColumnType("int");

                    b.Property<double>("RecipientLatitude")
                        .HasColumnType("float");

                    b.Property<double>("RecipientLongitude")
                        .HasColumnType("float");

                    b.Property<DateTime>("RegisterDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("SenderId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RecipientId");

                    b.HasIndex("SenderId");

                    b.ToTable("Parcels");
                });

            modelBuilder.Entity("Courier.DataLayer.Models.Shipment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("CarId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DeliveredTime")
                        .HasColumnType("datetime2");

                    b.Property<double>("DistanceToParcel")
                        .HasColumnType("float");

                    b.Property<double>("DistanceToRecipient")
                        .HasColumnType("float");

                    b.Property<int>("ParcelId")
                        .HasColumnType("int");

                    b.Property<Guid>("ParcelNumber")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("PickedUpTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("RegisterDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ScheduledDeliveryTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ScheduledPickUpTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Scoring")
                        .HasColumnType("int");

                    b.Property<double>("TravelTimeToParcel")
                        .HasColumnType("float");

                    b.Property<double>("TravelTimeToRecipient")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Shipments");
                });

            modelBuilder.Entity("Courier.DataLayer.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("AddressId")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Courier.DataLayer.Models.Parcel", b =>
                {
                    b.HasOne("Courier.DataLayer.Models.User", "Recipient")
                        .WithMany()
                        .HasForeignKey("RecipientId");

                    b.HasOne("Courier.DataLayer.Models.User", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Recipient");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("Courier.DataLayer.Models.User", b =>
                {
                    b.HasOne("Courier.DataLayer.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId");

                    b.Navigation("Address");
                });
#pragma warning restore 612, 618
        }
    }
}