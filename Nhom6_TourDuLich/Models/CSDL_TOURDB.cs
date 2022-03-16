using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Nhom6_TourDuLich.Models
{
	public partial class CSDL_TOURDB : DbContext
	{
		public CSDL_TOURDB()
		    : base("name=CSDL_TOURDB")
		{
		}

		public virtual DbSet<Bill> Bills { get; set; }
		public virtual DbSet<Category> Categories { get; set; }
		public virtual DbSet<News> News { get; set; }
		public virtual DbSet<Order> Orders { get; set; }
		public virtual DbSet<Tour> Tours { get; set; }
		public virtual DbSet<User> User { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Category>()
			    .Property(e => e.Images)
			    .IsUnicode(false);

			modelBuilder.Entity<Category>()
			    .HasMany(e => e.Tours)
			    .WithRequired(e => e.Category)
			    .WillCascadeOnDelete(false);

			modelBuilder.Entity<News>()
			    .Property(e => e.Images)
			    .IsUnicode(false);

			modelBuilder.Entity<Order>()
			    .HasMany(e => e.Bills)
			    .WithRequired(e => e.Order)
			    .WillCascadeOnDelete(false);

			modelBuilder.Entity<Tour>()
			    .Property(e => e.Images)
			    .IsUnicode(false);

			modelBuilder.Entity<Tour>()
			    .HasMany(e => e.Bills)
			    .WithRequired(e => e.Tour)
			    .WillCascadeOnDelete(false);

			modelBuilder.Entity<User>()
			    .HasMany(e => e.Orders)
			    .WithRequired(e => e.User)
			    .WillCascadeOnDelete(false);
		}
	}
}
