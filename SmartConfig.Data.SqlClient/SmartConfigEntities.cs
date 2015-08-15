using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SmartConfig.Data
{
    /// <summary>
    /// Provides <c>DbContext</c> for retreiving configuration from a database.
    /// </summary>
    public class SmartConfigEntities<TConfigElement> : DbContext where TConfigElement : class
    {
        private readonly string _tableName;
        private readonly IEnumerable<string> _keys;

        public SmartConfigEntities(string connectionString, string tableName, IEnumerable<string> keys)
            : base(connectionString)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("tableName");
            }
            _tableName = tableName;
            _keys = keys;
        }

        /// <summary>
        /// Gets or sets config elements.
        /// </summary>
        public virtual DbSet<TConfigElement> ConfigElements { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TConfigElement>().ToTable(_tableName);

            // configure T columns
            var columnOrder = 1;
            foreach (var key in _keys)
            {
                modelBuilder
                    .Properties<string>()
                    .Where(p => p.Name == key)
                    .Configure(p => p.IsKey().HasColumnOrder(columnOrder++).HasMaxLength(200));
            }

            // configure value column
            modelBuilder
                    .Properties<string>()
                    .Where(p => p.Name == "Value")
                    .Configure(p => p.IsMaxLength());
        }
    }
}
