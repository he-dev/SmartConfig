using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;

namespace SmartConfig.Data
{
    /// <summary>
    /// Provides <c>DbContext</c> for retreiving configuration from a database.
    /// </summary>
    public sealed class SmartConfigContext<TSetting> : DbContext where TSetting : class
    {
        public SmartConfigContext(string connectionString) : base(connectionString)
        {
            Debug.Assert(!string.IsNullOrEmpty(connectionString));
        }

        public string SettingsTableName { get; set; }

        public IEnumerable<string> SettingsTableKeyNames { get; set; }

        /// <summary>
        /// Gets or sets config elements.
        /// </summary>
        public DbSet<TSetting> Settings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (string.IsNullOrEmpty(SettingsTableName))
            {
                throw new InvalidOperationException("SettingsTableName must not be empty.");
            }

            if (SettingsTableKeyNames == null)
            {
                throw new InvalidOperationException("SettingsTableKeyNames must not be empty.");
            }

            modelBuilder.Entity<TSetting>().ToTable(SettingsTableName);

            // configure keys
            var columnOrder = 0;
            foreach (var keyName in SettingsTableKeyNames)
            {
                var columnOrderClosure = columnOrder;
                modelBuilder
                    .Properties<string>()
                    .Where(p => p.Name == keyName)
                    .Configure(p => p.IsKey().HasColumnOrder(columnOrderClosure).HasMaxLength(200));
                columnOrder++;
            }

            // configure value column
            modelBuilder
                .Properties<string>()
                .Where(p => p.Name == "Value")
                .Configure(p => p.IsMaxLength());

            base.OnModelCreating(modelBuilder);
        }
    }
}
