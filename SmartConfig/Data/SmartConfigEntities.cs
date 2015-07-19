namespace SmartConfig.Data
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    /// <summary>
    /// Provides <c>DbContext</c> for retreiving configuration from a database.
    /// </summary>
    public partial class SmartConfigEntities : DbContext
    {
        public SmartConfigEntities(string connectionString)
            : base(connectionString)
        {
        }

        /// <summary>
        /// Gets or sets config elements.
        /// </summary>
        public virtual DbSet<ConfigElement> ConfigElements { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
