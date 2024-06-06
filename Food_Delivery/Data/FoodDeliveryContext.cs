using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Food_Delivery.Model;

namespace Food_Delivery.Data
{
    public class FoodDeliveryContext : DbContext
    {
        public FoodDeliveryContext() { Database.EnsureCreated(); }

        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Dishes> Dishes { get; set; }
        public virtual DbSet<CompositionCart> CompositionCarts { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<OrderStatus> OrderStatus { get; set; }
        public virtual DbSet<Order> Orders { get; set; }

        // настройка атрибутов БД (Fluent API)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.id).HasName("PK__role");

                entity.ToTable("role");

                entity.Property(e => e.id).HasColumnName("account");
                entity.Property(e => e.name).HasMaxLength(50).HasColumnName("name");
            });

            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.id).HasName("PK__account");

                entity.ToTable("account");

                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.roleId).HasColumnName("roleId");
                entity.Property(e => e.name).HasMaxLength(100).HasColumnName("name");
                entity.Property(e => e.surname).HasMaxLength(100).HasColumnName("surname");
                entity.Property(e => e.patronymic).HasMaxLength(100).HasColumnName("patronymic");
                entity.Property(e => e.registrationDate).HasColumnType("datetime").HasColumnName("registrationDate");
                entity.Property(e => e.email).HasMaxLength(200).HasColumnName("email");
                entity.Property(e => e.numberPhone).HasMaxLength(11).HasColumnName("numberPhone");
                entity.Property(e => e.login).HasMaxLength(100).HasColumnName("login");
                entity.Property(e => e.password).HasMaxLength(100).HasColumnName("password");
                entity.Property(e => e.city).HasMaxLength(50).HasColumnName("city");
                entity.Property(e => e.street).HasMaxLength(150).HasColumnName("street");
                entity.Property(e => e.house).HasMaxLength(10).HasColumnName("house");
                entity.Property(e => e.apartment).HasMaxLength(10).HasColumnName("apartament");

                entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.id).HasConstraintName("fk_roleId_role");
            });

        }

        // подключение БД
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=ALEX_BEREZKIN\SQLEXPRESS;Database=Food_Delivery_CourseworkDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True; encrypt=false");
        }
    }
}
