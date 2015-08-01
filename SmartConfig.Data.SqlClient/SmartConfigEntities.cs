using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SmartConfig.Data
{
    /// <summary>
    /// Provides <c>DbContext</c> for retreiving configuration from a database.
    /// </summary>
    public partial class SmartConfigEntities<TConfigElement> : DbContext where TConfigElement : class 
    {
        private readonly string _tableName;

        public SmartConfigEntities(string connectionString, string tableName)
            : base(connectionString)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("tableName");
            }
            _tableName = tableName;
        }

        /// <summary>
        /// Gets or sets config elements.
        /// </summary>
        public virtual DbSet<TConfigElement> ConfigElements { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TConfigElement>().ToTable(_tableName);
        }
    }
}
