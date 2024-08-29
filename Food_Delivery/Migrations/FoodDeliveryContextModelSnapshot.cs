﻿// <auto-generated />
using System;
using Food_Delivery.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Food_Delivery.Migrations
{
    [DbContext(typeof(FoodDeliveryContext))]
    partial class FoodDeliveryContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Food_Delivery.Model.Account", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("apartment")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)")
                        .HasColumnName("apartament");

                    b.Property<string>("city")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("city");

                    b.Property<string>("email")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)")
                        .HasColumnName("email");

                    b.Property<string>("house")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)")
                        .HasColumnName("house");

                    b.Property<string>("login")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("login");

                    b.Property<string>("name")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("name");

                    b.Property<string>("numberPhone")
                        .HasMaxLength(11)
                        .HasColumnType("nvarchar(11)")
                        .HasColumnName("numberPhone");

                    b.Property<string>("password")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("password");

                    b.Property<string>("patronymic")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("patronymic");

                    b.Property<DateTime?>("registrationDate")
                        .HasColumnType("datetime")
                        .HasColumnName("registrationDate");

                    b.Property<int>("roleId")
                        .HasColumnType("int")
                        .HasColumnName("roleId");

                    b.Property<string>("street")
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)")
                        .HasColumnName("street");

                    b.Property<string>("surname")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("surname");

                    b.HasKey("id")
                        .HasName("PK__account");

                    b.HasIndex("roleId");

                    b.ToTable("account", (string)null);
                });

            modelBuilder.Entity("Food_Delivery.Model.Category", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("description")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)")
                        .HasColumnName("description");

                    b.Property<string>("name")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("name");

                    b.HasKey("id")
                        .HasName("PK__category");

                    b.ToTable("category", (string)null);
                });

            modelBuilder.Entity("Food_Delivery.Model.CompositionCart", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<int>("dishesId")
                        .HasColumnType("int")
                        .HasColumnName("dishesId");

                    b.Property<int>("quantity")
                        .HasColumnType("int")
                        .HasColumnName("quantity");

                    b.Property<int>("shoppingCartId")
                        .HasColumnType("int")
                        .HasColumnName("shoppingCartId");

                    b.HasKey("id")
                        .HasName("PK_compositionCart");

                    b.HasIndex("dishesId");

                    b.HasIndex("shoppingCartId");

                    b.ToTable("compositionCart", (string)null);
                });

            modelBuilder.Entity("Food_Delivery.Model.CompositionOrder", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<int?>("calories")
                        .HasColumnType("int")
                        .HasColumnName("calories");

                    b.Property<int?>("carbohydrates")
                        .HasColumnType("int")
                        .HasColumnName("carbohydrates");

                    b.Property<string>("descriptionDishes")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)")
                        .HasColumnName("descriptionDishes");

                    b.Property<int?>("dishesId")
                        .HasColumnType("int")
                        .HasColumnName("dishesId");

                    b.Property<int?>("fats")
                        .HasColumnType("int")
                        .HasColumnName("fats");

                    b.Property<byte[]>("image")
                        .IsRequired()
                        .HasColumnType("varbinary(max)")
                        .HasColumnName("image");

                    b.Property<string>("nameDishes")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("nameDishes");

                    b.Property<int>("orderId")
                        .HasColumnType("int")
                        .HasColumnName("orderId");

                    b.Property<int>("price")
                        .HasColumnType("int")
                        .HasColumnName("price");

                    b.Property<int?>("quantity")
                        .HasColumnType("int")
                        .HasColumnName("quantity");

                    b.Property<int?>("squirrels")
                        .HasColumnType("int")
                        .HasColumnName("squirrels");

                    b.Property<int?>("weight")
                        .HasColumnType("int")
                        .HasColumnName("weight");

                    b.HasKey("id")
                        .HasName("PK_compositionOrder");

                    b.HasIndex("dishesId");

                    b.HasIndex("orderId");

                    b.ToTable("compositionOrder", (string)null);
                });

            modelBuilder.Entity("Food_Delivery.Model.Dishes", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<int?>("calories")
                        .HasColumnType("int")
                        .HasColumnName("calories");

                    b.Property<int?>("carbohydrates")
                        .HasColumnType("int")
                        .HasColumnName("carbohydrates");

                    b.Property<int>("categoryId")
                        .HasColumnType("int")
                        .HasColumnName("categoryId");

                    b.Property<string>("description")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)")
                        .HasColumnName("description");

                    b.Property<int?>("fats")
                        .HasColumnType("int")
                        .HasColumnName("fats");

                    b.Property<byte[]>("image")
                        .IsRequired()
                        .HasColumnType("varbinary(max)")
                        .HasColumnName("image");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("name");

                    b.Property<int>("price")
                        .HasColumnType("int")
                        .HasColumnName("price");

                    b.Property<int?>("quantity")
                        .HasColumnType("int")
                        .HasColumnName("quantity");

                    b.Property<int?>("squirrels")
                        .HasColumnType("int")
                        .HasColumnName("squirrels");

                    b.Property<bool>("stopList")
                        .HasColumnType("bit")
                        .HasColumnName("stopList");

                    b.Property<int?>("weight")
                        .HasColumnType("int")
                        .HasColumnName("weight");

                    b.HasKey("id")
                        .HasName("PK_dishes");

                    b.HasIndex("categoryId");

                    b.ToTable("dishes", (string)null);
                });

            modelBuilder.Entity("Food_Delivery.Model.Order", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<int?>("accountId")
                        .HasColumnType("int")
                        .HasColumnName("accountId");

                    b.Property<string>("apartment")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)")
                        .HasColumnName("apartament");

                    b.Property<string>("city")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("city");

                    b.Property<int>("costPrice")
                        .HasColumnType("int")
                        .HasColumnName("costPrice");

                    b.Property<DateTime?>("dateTime")
                        .HasColumnType("datetime")
                        .HasColumnName("dateTime");

                    b.Property<string>("email")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)")
                        .HasColumnName("email");

                    b.Property<DateTime?>("endDesiredDeliveryTime")
                        .HasColumnType("datetime")
                        .HasColumnName("endDesiredDeliveryTime");

                    b.Property<string>("house")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)")
                        .HasColumnName("house");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("name");

                    b.Property<string>("numberPhone")
                        .HasMaxLength(11)
                        .HasColumnType("nvarchar(11)")
                        .HasColumnName("numberPhone");

                    b.Property<int?>("orderStatusId")
                        .HasColumnType("int")
                        .HasColumnName("orderStatusId");

                    b.Property<string>("patronymic")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("patronymic");

                    b.Property<int?>("prepareChangeMoney")
                        .HasColumnType("int")
                        .HasColumnName("prepareChangeMoney");

                    b.Property<DateTime?>("startDesiredDeliveryTime")
                        .HasColumnType("datetime")
                        .HasColumnName("startDesiredDeliveryTime");

                    b.Property<string>("street")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)")
                        .HasColumnName("street");

                    b.Property<string>("surname")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("surname");

                    b.Property<string>("typePayment")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)")
                        .HasColumnName("typePayment");

                    b.HasKey("id")
                        .HasName("PK_order");

                    b.HasIndex("accountId");

                    b.HasIndex("orderStatusId");

                    b.ToTable("order", (string)null);
                });

            modelBuilder.Entity("Food_Delivery.Model.OrderStatus", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)")
                        .HasColumnName("name");

                    b.HasKey("id")
                        .HasName("PK_orderStatus");

                    b.ToTable("orderStatus", (string)null);
                });

            modelBuilder.Entity("Food_Delivery.Model.Role", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("name");

                    b.HasKey("id")
                        .HasName("PK__role");

                    b.ToTable("role", (string)null);
                });

            modelBuilder.Entity("Food_Delivery.Model.ShoppingCart", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<int?>("accountId")
                        .HasColumnType("int")
                        .HasColumnName("accountId");

                    b.Property<int?>("costPrice")
                        .HasColumnType("int")
                        .HasColumnName("costPrice");

                    b.HasKey("id")
                        .HasName("PK_shoppingCart");

                    b.HasIndex("accountId");

                    b.ToTable("shoppingCart", (string)null);
                });

            modelBuilder.Entity("Food_Delivery.Model.Account", b =>
                {
                    b.HasOne("Food_Delivery.Model.Role", "Role")
                        .WithMany("Accounts")
                        .HasForeignKey("roleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_roleId_account_role");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Food_Delivery.Model.CompositionCart", b =>
                {
                    b.HasOne("Food_Delivery.Model.Dishes", "Dishes")
                        .WithMany("CompositionCarts")
                        .HasForeignKey("dishesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_dishesId_compositionCart_dishes");

                    b.HasOne("Food_Delivery.Model.ShoppingCart", "ShoppingCart")
                        .WithMany("CompositionCarts")
                        .HasForeignKey("shoppingCartId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_shoppingCartId_compositionCart_shoppingCart");

                    b.Navigation("Dishes");

                    b.Navigation("ShoppingCart");
                });

            modelBuilder.Entity("Food_Delivery.Model.CompositionOrder", b =>
                {
                    b.HasOne("Food_Delivery.Model.Dishes", "Dishes")
                        .WithMany("CompositionOrders")
                        .HasForeignKey("dishesId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .HasConstraintName("fk_dishesId_compositionOrder_dishes");

                    b.HasOne("Food_Delivery.Model.Order", "Order")
                        .WithMany("CompositionOrders")
                        .HasForeignKey("orderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_orderId_compositionOrder_oder");

                    b.Navigation("Dishes");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Food_Delivery.Model.Dishes", b =>
                {
                    b.HasOne("Food_Delivery.Model.Category", "Category")
                        .WithMany("Dishes")
                        .HasForeignKey("categoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_categoryId_dishes_category");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Food_Delivery.Model.Order", b =>
                {
                    b.HasOne("Food_Delivery.Model.Account", "Account")
                        .WithMany("Orders")
                        .HasForeignKey("accountId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .HasConstraintName("fk_accountId_order_account");

                    b.HasOne("Food_Delivery.Model.OrderStatus", "OrderStatus")
                        .WithMany("Orders")
                        .HasForeignKey("orderStatusId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .HasConstraintName("fk_orderStatusId_order_orderStatus");

                    b.Navigation("Account");

                    b.Navigation("OrderStatus");
                });

            modelBuilder.Entity("Food_Delivery.Model.ShoppingCart", b =>
                {
                    b.HasOne("Food_Delivery.Model.Account", "Account")
                        .WithMany("ShoppingCarts")
                        .HasForeignKey("accountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("fk_accountId_shoppingCart_account");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Food_Delivery.Model.Account", b =>
                {
                    b.Navigation("Orders");

                    b.Navigation("ShoppingCarts");
                });

            modelBuilder.Entity("Food_Delivery.Model.Category", b =>
                {
                    b.Navigation("Dishes");
                });

            modelBuilder.Entity("Food_Delivery.Model.Dishes", b =>
                {
                    b.Navigation("CompositionCarts");

                    b.Navigation("CompositionOrders");
                });

            modelBuilder.Entity("Food_Delivery.Model.Order", b =>
                {
                    b.Navigation("CompositionOrders");
                });

            modelBuilder.Entity("Food_Delivery.Model.OrderStatus", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("Food_Delivery.Model.Role", b =>
                {
                    b.Navigation("Accounts");
                });

            modelBuilder.Entity("Food_Delivery.Model.ShoppingCart", b =>
                {
                    b.Navigation("CompositionCarts");
                });
#pragma warning restore 612, 618
        }
    }
}
