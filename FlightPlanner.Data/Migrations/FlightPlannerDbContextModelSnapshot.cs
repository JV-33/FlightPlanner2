﻿// <auto-generated />
using FlightPlanner.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FlightPlanner.Data.Migrations
{
    [DbContext(typeof(FlightPlannerDbContext))]
    partial class FlightPlannerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.11");

            modelBuilder.Entity("FlightPlanner.Core.Models.Airport", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AirportCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Airport");
                });

            modelBuilder.Entity("FlightPlanner.Core.Models.Flight", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ArrivalTime")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Carrier")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("DepartureTime")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("FromID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ToID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("FromID");

                    b.HasIndex("ToID");

                    b.ToTable("Flights");
                });

            modelBuilder.Entity("FlightPlanner.Core.Models.Flight", b =>
                {
                    b.HasOne("FlightPlanner.Core.Models.Airport", "From")
                        .WithMany()
                        .HasForeignKey("FromID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FlightPlanner.Core.Models.Airport", "To")
                        .WithMany()
                        .HasForeignKey("ToID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("From");

                    b.Navigation("To");
                });
#pragma warning restore 612, 618
        }
    }
}