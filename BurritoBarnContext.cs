namespace BurritoBarn
{
	using System;
	using System.Data.Entity;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Linq;

	public partial class BurritoBarnContext : DbContext
	{
		public BurritoBarnContext()
			: base("name=BurritoBarn")
		{
		}

		public virtual DbSet<Cuisine> Cuisines { get; set; }
		public virtual DbSet<Employee> Employees { get; set; }
		public virtual DbSet<Rating> Ratings { get; set; }
		public virtual DbSet<Restaurant> Restaurants { get; set; }
		public virtual DbSet<Supply> Supplies { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Cuisine>()
				.HasMany(e => e.Restaurants)
				.WithRequired(e => e.Cuisine)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Employee>()
				.HasMany(e => e.Ratings)
				.WithRequired(e => e.Employee)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Employee>()
				.HasMany(e => e.Supplies)
				.WithRequired(e => e.Employee)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Restaurant>()
				.HasMany(e => e.Ratings)
				.WithRequired(e => e.Restaurant)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Restaurant>()
				.HasMany(e => e.Supplies)
				.WithRequired(e => e.Restaurant)
				.WillCascadeOnDelete(false);
		}
	}
}
