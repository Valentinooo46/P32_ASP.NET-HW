using Domain.Entities;
using Domain.Entities.Idenity;
using Domain.Entities.Location;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Domain;

public class AppDbTransferContext : IdentityDbContext<UserEntity, RoleEntity, int,
 IdentityUserClaim<int>, UserRoleEntity, IdentityUserLogin<int>,
 IdentityRoleClaim<int>, IdentityUserToken<int>>
{
 public AppDbTransferContext(DbContextOptions<AppDbTransferContext> options)
 : base(options)
 {
 }

 public DbSet<CountryEntity> Countries { get; set; }
 public DbSet<CityEntity> Cities { get; set; }
 public DbSet<TransportationStatusEntity> TransportationStatuses { get; set; }
 public DbSet<TransportationEntity> Transportations { get; set; }
 public DbSet<CartEntity> Carts { get; set; }

 protected override void OnModelCreating(ModelBuilder builder)
 {
 base.OnModelCreating(builder);

 // Identity relations
 builder.Entity<UserRoleEntity>()
 .HasOne(ur => ur.User)
 .WithMany(u => u.UserRoles)
 .HasForeignKey(ur => ur.UserId);

 builder.Entity<UserRoleEntity>()
 .HasOne(ur => ur.Role)
 .WithMany(u => u.UserRoles)
 .HasForeignKey(ur => ur.RoleId);

 // City -> Transportation relations
 builder.Entity<CityEntity>()
 .HasMany(c => c.Departures)
 .WithOne(t => t.FromCity)
 .HasForeignKey(t => t.FromCityId);

 builder.Entity<CityEntity>()
 .HasMany(c => c.Arrivals)
 .WithOne(t => t.ToCity)
 .HasForeignKey(t => t.ToCityId);

 // Cart composite key and relationships
 builder.Entity<CartEntity>()
 .HasKey(pi => new { pi.TransportationId, pi.UserId });

 // Ensure key properties are not treated as database generated values
 builder.Entity<CartEntity>()
 .Property(c => c.TransportationId)
 .ValueGeneratedNever();

 builder.Entity<CartEntity>()
 .Property(c => c.UserId)
 .ValueGeneratedNever();

 // Explicitly configure foreign keys and navigation properties
 builder.Entity<CartEntity>()
 .HasOne(c => c.Transportation)
 .WithMany(t => t.Carts)
 .HasForeignKey(c => c.TransportationId)
 .OnDelete(DeleteBehavior.Cascade);

 builder.Entity<CartEntity>()
 .HasOne(c => c.User)
 .WithMany(u => u.Carts)
 .HasForeignKey(c => c.UserId)
 .OnDelete(DeleteBehavior.Cascade);
 }
}
