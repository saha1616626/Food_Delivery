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
        public FoodDeliveryContext() { Database.EnsureCreated(); } // проверка наличия БД

        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Dishes> Dishes { get; set; }
        public virtual DbSet<CompositionCart> CompositionCarts { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<OrderStatus> OrderStatus { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<CompositionOrder> CompositionOrders { get; set; }

        // настройка атрибутов БД (Fluent API)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.id).HasName("PK__role");

                entity.ToTable("role");

                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.name).HasMaxLength(50).HasColumnName("name");
            });

            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.id).HasName("PK__account");

                entity.ToTable("account");

                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.roleId).HasColumnName("roleId");
                entity.Property(e => e.name).HasMaxLength(100).HasColumnName("name").IsRequired(false);
                entity.Property(e => e.surname).HasMaxLength(100).HasColumnName("surname").IsRequired(false);
                entity.Property(e => e.patronymic).HasMaxLength(100).HasColumnName("patronymic").IsRequired(false);
                entity.Property(e => e.registrationDate).HasColumnType("datetime").HasColumnName("registrationDate").IsRequired(false);
                entity.Property(e => e.email).HasMaxLength(200).HasColumnName("email").IsRequired(false);
                entity.Property(e => e.numberPhone).HasMaxLength(11).HasColumnName("numberPhone").IsRequired(false);
                entity.Property(e => e.login).HasMaxLength(100).HasColumnName("login");
                entity.Property(e => e.password).HasMaxLength(100).HasColumnName("password");
                entity.Property(e => e.city).HasMaxLength(50).HasColumnName("city").IsRequired(false);
                entity.Property(e => e.street).HasMaxLength(150).HasColumnName("street").IsRequired(false);
                entity.Property(e => e.house).HasMaxLength(10).HasColumnName("house").IsRequired(false);
                entity.Property(e => e.apartment).HasMaxLength(10).HasColumnName("apartament").IsRequired(false);

                entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.id).HasConstraintName("fk_roleId_role").OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.id).HasName("PK__category");

                entity.ToTable("category");

                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.name).HasMaxLength(100).HasColumnName("name");
                entity.Property(e => e.description).HasMaxLength(300).HasColumnName("description").IsRequired(false);
            });

            modelBuilder.Entity<Dishes>(entity =>
            {
                entity.HasKey(e => e.id).HasName("PK_dishes");

                entity.ToTable("dishes");
                
                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.name).HasColumnName("name").HasMaxLength(100);
                entity.Property(e => e.description).HasColumnName("description").HasMaxLength(300).IsRequired(false);
                entity.Property(e => e.categoryId).HasColumnName("categoryId");
                entity.Property(e => e.calories).HasColumnName("calories").IsRequired(false);
                entity.Property(e => e.squirrels).HasColumnName("squirrels").IsRequired(false);
                entity.Property(e => e.fats).HasColumnName("fats").IsRequired(false);
                entity.Property(e => e.carbohydrates).HasColumnName("carbohydrates").IsRequired(false);
                entity.Property(e => e.weight).HasColumnName("weight").IsRequired(false);
                entity.Property(e => e.quantity).HasColumnName("quantity").IsRequired(false);
                entity.Property(e => e.price).HasColumnName("price");
                entity.Property(e => e.image).HasColumnName("image");
                entity.Property(e => e.stopList).HasColumnName("stopList");

                entity.HasOne(d => d.Category).WithMany(p => p.Dishes)
                .HasForeignKey(d => d.id).HasConstraintName("fk_categoryId_category").OnDelete(DeleteBehavior.Cascade); // каскадное удаление категорий, при удалении блюда
            });

            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.HasKey(e => e.id).HasName("PK_shoppingCart");

                entity.ToTable("shoppingCart");

                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.accountId).HasColumnName("id").IsRequired(false);
                entity.Property(e => e.costPrice).HasColumnName("costPrice").IsRequired(false);

                entity.HasOne(d => d.Account).WithMany(p => p.ShoppingCarts)
                .HasForeignKey(d => d.id).HasConstraintName("fk_accountId_account").OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CompositionCart>(entity =>
            {
                entity.HasKey(e => e.id).HasName("PK_compositionCart");

                entity.ToTable("compositionCart");

                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.shoppingCartId).HasColumnName("shoppingCartId");
                entity.Property(e => e.dishesId).HasColumnName("dishesId");
                entity.Property(e => e.quantity).HasColumnName("quantity");

                entity.HasOne(d => d.ShoppingCart).WithMany(p => p.CompositionCarts)
                .HasForeignKey(d => d.id).HasConstraintName("fk_shoppingCartId_shoppingCart").OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Dishes).WithMany(p => p.CompositionCarts)
                .HasForeignKey(d => d.id).HasConstraintName("fk_dishesId_dishes").OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OrderStatus>(entity =>
            {
                entity.HasKey(e => e.id).HasName("PK_orderStatus");

                entity.ToTable("orderStatus");

                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.name).HasColumnName("name").HasMaxLength(150);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.id).HasName("PK_order");

                entity.ToTable("order");

                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.dateTime).HasColumnType("datetime").HasColumnName("dateTime");
                entity.Property(e => e.startDesiredDeliveryTime).HasColumnName("startDesiredDeliveryTime").HasColumnType("datetime");
                entity.Property(e => e.endDesiredDeliveryTime).HasColumnName("endDesiredDeliveryTime").HasColumnType("datetime");
                entity.Property(e => e.accountId).HasColumnName("accountId").IsRequired(false);
                entity.Property(e => e.shoppingCartId).HasColumnName("shoppingCartId").IsRequired(false);
                entity.Property(e => e.orderStatusId).HasColumnName("orderStatusId").IsRequired(false);
                entity.Property(e => e.name).HasMaxLength(100).HasColumnName("name");
                entity.Property(e => e.surname).HasMaxLength(100).HasColumnName("surname");
                entity.Property(e => e.patronymic).HasMaxLength(100).HasColumnName("patronymic").IsRequired(false);
                entity.Property(e => e.city).HasMaxLength(50).HasColumnName("city");
                entity.Property(e => e.street).HasMaxLength(150).HasColumnName("street");
                entity.Property(e => e.house).HasMaxLength(10).HasColumnName("house");
                entity.Property(e => e.apartment).HasMaxLength(10).HasColumnName("apartament").IsRequired(false);
                entity.Property(e => e.numberPhone).HasMaxLength(11).HasColumnName("numberPhone");
                entity.Property(e => e.email).HasMaxLength(200).HasColumnName("email").IsRequired(false);
                entity.Property(e => e.costPrice).HasColumnName("costPrice");
                entity.Property(e => e.typePayment).HasColumnName("typePayment").HasMaxLength(150);
                entity.Property(e => e.prepareChangeMoney).HasColumnName("prepareChangeMoney").IsRequired(false);

                entity.HasOne(d => d.Account).WithMany(p => p.Orders)
                .HasForeignKey(d => d.id).HasConstraintName("fk_accountId_account").OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.ShoppingCart).WithMany(p => p.Orders)
                .HasForeignKey(d => d.id).HasConstraintName("fk_shoppingCartId_shoppingCart").OnDelete(DeleteBehavior.SetNull); ;

                entity.HasOne(d => d.OrderStatus).WithMany(p => p.Orders)
                .HasForeignKey(d => d.id).HasConstraintName("fk_orderStatusId_orderStatus").OnDelete(DeleteBehavior.SetNull); ;
                /* При удалении внешних ключей связанных с заказом, будут подставляться null */
            });


            modelBuilder.Entity<CompositionOrder>(entity =>
            {
                entity.HasKey(e => e.id).HasName("PK_order");

                entity.ToTable("order");

                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.orderId).HasColumnName("orderId");
                entity.Property(e => e.dishesId).HasColumnName("dishesId");
                entity.Property(e => e.nameDishes).HasColumnName("nameDishes").HasMaxLength(100);
                entity.Property(e => e.descriptionDishes).HasColumnName("descriptionDishes").HasMaxLength(300).IsRequired(false);
                entity.Property(e => e.calories).HasColumnName("calories").IsRequired(false);
                entity.Property(e => e.squirrels).HasColumnName("squirrels").IsRequired(false);
                entity.Property(e => e.fats).HasColumnName("fats").IsRequired(false);
                entity.Property(e => e.carbohydrates).HasColumnName("carbohydrates").IsRequired(false);
                entity.Property(e => e.weight).HasColumnName("weight").IsRequired(false);
                entity.Property(e => e.quantity).HasColumnName("quantity").IsRequired(false);
                entity.Property(e => e.price).HasColumnName("price");
                entity.Property(e => e.image).HasColumnName("image");

                entity.HasOne(c => c.Order).WithMany(d => d.CompositionOrders)
                .HasForeignKey(d => d.id).HasConstraintName("fk_orderId_oder").OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.Dishes).WithMany(d => d.CompositionOrders)
                .HasForeignKey(d => d.id).HasConstraintName("fk_dishesId_dishes").OnDelete(DeleteBehavior.SetNull);
                // удаления блюда никак не будет влиять на состав заказа
            });
        }


        // подключение БД
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=ALEX_BEREZKIN\SQLEXPRESS;Database=Food_Delivery_CourseWorkDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True; encrypt=false");
        }
    }
}
