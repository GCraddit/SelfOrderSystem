using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Giles_Chen_test_1;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;

namespace Giles_Chen_test_1
{
    public class CafeContext : DbContext
    {
        public CafeContext(DbContextOptions<CafeContext> options) : base(options)
        {
        }

        public CafeContext()
        {
        }

        public DbSet<Foodandbev> Foodandbevs { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Staff> Staffs {  get; set; } 

        public DbSet<Merch> Merches { get; set; }

        public DbSet<Member>Members { get; set; }


        public void InitializeStaffs()
        {
            if (!Staffs.Any()) // Check if the Staffs table is empty
            {
                // Create a list of initial Staff data
                var initialStaffs = new List<Staff>
        {
            new Staff { staffID = 1, password = "password1" },
            new Staff { staffID = 2, password = "password2" },
            new Staff { staffID = 3, password = "password3" }
        };

                // Add to the database
                Staffs.AddRange(initialStaffs);
                SaveChanges(); // Save changes to the database
            }
        }
        public void InitializeMembers()
        {
            // Check if any members already exist in the Members table.
            if (Members.Any())
            {
                // If there are already members in the table, do nothing.
                return;
            }

            // Define a list of initial members to add to the database.
            var initialMembers = new List<Member>
    {
        new Member
        {
            MemberId = new Random().Next(10000, 99999),  // The unique identifier for the member.
            Phone = "1234567890",   // Phone number of the member.
            MbPassword = "password123",  // Password for the member.
            Name = "ABC",              // Name of the member.
            Point = 50                 // Initial points for the member.
        }
    };
            try
            {
                // Add the initial members to the Members table.
                Members.AddRange(initialMembers);

                // Commit changes to the database.
                SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while initializing members: " + ex.Message);
                throw; // Rethrow the exception to see full stack trace
            }

        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Foodandbev>()
                .HasKey(f => f.FoodandbevID); 

            modelBuilder.Entity<Foodandbev>()
                .Property(f => f.foodandbevName)
                .IsRequired()
                .HasMaxLength(50); 

            modelBuilder.Entity<Foodandbev>()
                .Property(f => f.foodandbevPrice)
                .HasColumnType("decimal(18, 2)"); 

            modelBuilder.Entity<Foodandbev>()
                .Property(f => f.Type)
                .HasConversion<string>();

            modelBuilder.Entity<Order>()
                .HasKey(o => o.OrderID);

            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => oi.OrderItemID);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderID);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Foodandbev)
                .WithMany()
                .HasForeignKey(oi => oi.FoodandbevID);

            modelBuilder.Entity<Staff>()
                .HasKey(s => s.staffID);

            modelBuilder.Entity<Staff>()
                .Property(s => s.password)
                .IsRequired();

            modelBuilder.Entity<Merch>(entity =>
            {
                entity.ToTable("Merches"); // Ensures the table name matches the database

                entity.HasKey(e => e.MerchID); // Set the primary key

                // Configure properties
                entity.Property(e => e.MerchID).HasColumnName("merchID");
                entity.Property(e => e.MerchImagePath).HasColumnName("merchImagePath").HasMaxLength(255);
                entity.Property(e => e.MerchName).HasColumnName("merchName").IsRequired().HasMaxLength(100);
                entity.Property(e => e.MerchDescription).HasColumnName("merchDescription").HasMaxLength(500);
                entity.Property(e => e.MerchPoints).HasColumnName("merchPoints").IsRequired();
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.ToTable("Members"); 

                entity.HasKey(e => e.MemberId); 

                entity.Property(e => e.MemberId)
                    .HasColumnName("MemberId")
                    .IsRequired();

                entity.Property(e => e.Phone)
                    .HasColumnName("Phone")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.MbPassword)
                    .HasColumnName("MbPassword")
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(e => e.Name)
                    .HasColumnName("Name")
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(e => e.Point)
                    .HasColumnName("Point")
                    .HasDefaultValue(0);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                
                string connectionString = ConfigurationManager.ConnectionStrings["CafeDatabase"].ConnectionString;

                
                optionsBuilder.UseSqlServer(connectionString);
            }
        }



    }
}
