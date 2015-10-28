using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;

namespace SmartConfig.Data
{
    /// <summary>
    /// Provides <c>DbContext</c> for retreiving configuration from a database.
    /// </summary>
    internal sealed class SmartConfigContext<TSetting> : DbContext where TSetting : class
    {
        public SmartConfigContext(string connectionString) : base(connectionString)
        {
            Debug.Assert(!string.IsNullOrEmpty(connectionString));
        }

        internal string SettingsTableName { get; set; }

        internal IEnumerable<string> SettingsTableKeyNames { get; set; }

        /// <summary>
        /// Gets or sets config elements.
        /// </summary>
        public DbSet<TSetting> Settings { get; set; }

        internal static SmartConfigContext<TSetting> Create(string connectionString, string settingsTableName, IEnumerable<string> keyNames)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            if (string.IsNullOrEmpty(settingsTableName)) throw new ArgumentNullException(nameof(settingsTableName));
            if (keyNames == null || !keyNames.Any()) throw new ArgumentNullException(nameof(settingsTableName));

            return new SmartConfigContext<TSetting>(connectionString)
            {
                SettingsTableName = settingsTableName,
                SettingsTableKeyNames = keyNames
            };
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Debug.Assert(!string.IsNullOrEmpty(SettingsTableName), $"{SettingsTableName} must not be empty.");
            Debug.Assert(SettingsTableKeyNames != null && SettingsTableKeyNames.Any(), $"{SettingsTableKeyNames} must not be empty.");

            modelBuilder.Entity<TSetting>().ToTable(SettingsTableName);

            // configure keys
            var columnOrder = 0;
            foreach (var keyName in SettingsTableKeyNames)
            {
                var columnOrderClosure = columnOrder++;
                modelBuilder
                    .Properties<string>()
                    .Where(p => p.Name == keyName)
                    .Configure(p => p.IsKey().HasColumnOrder(columnOrderClosure).HasMaxLength(200));
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
