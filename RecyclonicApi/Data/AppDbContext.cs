using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecyclonicApi.Models;
using RecyclonicApi.Models.Domain;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace RecyclonicApi.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<MarketplaceItem> MarketplaceItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<EwasteItem> EwasteItems { get; set; }
        public DbSet<RecycleRequest> RecycleRequests { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<StatusTraking> StatusTrackings { get; set; }
        public DbSet<Images> Images { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Invitation> Invitations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            base.OnModelCreating(modelbuilder);

            modelbuilder.Entity<Cart>()
                .HasMany(c => c.CartItems)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelbuilder.Entity<CartItem>()
                .HasOne(ci => ci.marketplaceItem)
                .WithMany(ma => ma.CartItems)
                .HasForeignKey(ci => ci.MarketplaceItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelbuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelbuilder.Entity<OrderItem>()
                .HasOne(oi => oi.marketplaceItem)
                .WithMany(mai => mai.OrderItems)
                .HasForeignKey(oi => oi.MarketplaceItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelbuilder.Entity<EwasteItem>()
                .HasOne(e => e.recycleRequest)
                .WithOne(r => r.ewasteItem)
                .HasForeignKey<RecycleRequest>(e => e.EwasteItemId)
                .OnDelete(DeleteBehavior.Cascade);


            modelbuilder.Entity<Delivery>()
                .HasOne(d => d.RecycleRequest)
                .WithOne(r => r.delivery)
                .HasForeignKey<Delivery>(d => d.RecycleRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelbuilder.Entity<StatusTraking>()
                .HasOne(st => st.delivery)
                .WithMany(d => d.statusTraking)
                .HasForeignKey(st => st.DeliveryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelbuilder.Entity<Images>()
                .HasOne(i => i.ewasteItem)
                .WithMany(e => e.ImagesUrl)
                .HasForeignKey(i => i.EwasteitemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelbuilder.Entity<Images>()
                .HasOne(i => i.marketplaceItem)
                .WithMany(m => m.ImagesUrl)
                .HasForeignKey(i => i.MarketPlaceItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelbuilder.Entity<Transaction>()
                .HasOne(t => t.order)
                .WithOne(o => o.transaction)
                .HasForeignKey<Transaction>(t => t.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelbuilder.Entity<MarketplaceItem>()
                .HasMany(m => m.FavoritedBy)
                .WithMany(us => us.FavouriteItems);

            modelbuilder.Entity<MarketplaceItem>()
                .HasOne(m => m.seller)
                .WithMany(u => u.MyItems)
                .HasForeignKey(m => m.SellerId)
                .OnDelete(DeleteBehavior.Restrict);


            modelbuilder.Entity<RecycleRequest>()
                .HasOne(r => r.user)
                .WithMany(u => u.RecycleRequests)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelbuilder.Entity<RecycleRequest>()
                .HasOne(r => r.Employee)
                .WithMany()
                .HasForeignKey(r => r.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelbuilder.Entity<Cart>(entity =>
            {
                entity.Property(e => e.DeliveryFees).HasPrecision(18, 2);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            });

            modelbuilder.Entity<CartItem>(entity =>
            {
                entity.Property(e => e.PriceAtAdd).HasPrecision(18, 2);
            });

            modelbuilder.Entity<Coupon>(entity =>
            {
                entity.Property(e => e.DiscountValue).HasPrecision(18, 2);
            });

            modelbuilder.Entity<EwasteItem>(entity =>
            {
                entity.Property(e => e.weight).HasPrecision(18, 3);
            });

            modelbuilder.Entity<MarketplaceItem>(entity =>
            {
                entity.Property(e => e.Price).HasPrecision(18, 2);
            });

            modelbuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.TotalAmountAfterCopoun).HasPrecision(18, 2);
                entity.Property(e => e.TotalAmountBeforeCopoun).HasPrecision(18, 2);
                entity.Property(e => e.deliveryfees).HasPrecision(18, 2);
            });

            modelbuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(e => e.PriceAtPurchase).HasPrecision(18, 2);
            });

            modelbuilder.Entity<RecycleRequest>(entity =>
            {
                entity.Property(e => e.OfferedPrice).HasPrecision(18, 2);
                entity.Property(e => e.AmountPaidToUser).HasPrecision(18, 2);
                entity.Property(e => e.AmountReceivedFromRecycler).HasPrecision(18, 2);
            });

            modelbuilder.Entity<Transaction>(entity =>
            {
                entity.Property(e => e.Amount).HasPrecision(18, 2);
            });
        }

    }

}

