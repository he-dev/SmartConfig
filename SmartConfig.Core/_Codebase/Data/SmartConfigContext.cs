using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using SmartConfig.Collections;

namespace SmartConfig.Data
{
    /// <summary>
    /// Provides <c>DbContext</c> for retreiving configuration from a database.
    /// </summary>
    internal sealed class SmartConfigContext<TSetting> : DbContext where TSetting : Setting
    {
        private readonly string _settingsTableName;

        public SmartConfigContext(string connectionString, string settingsTableName) : base(connectionString)
        {
            Debug.Assert(!string.IsNullOrEmpty(connectionString));
            Debug.Assert(!string.IsNullOrEmpty(settingsTableName));

            _settingsTableName = settingsTableName;
        }

        /// <summary>
        /// Gets or sets config elements.
        /// </summary>
        public DbSet<TSetting> Settings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Debug.Assert(!string.IsNullOrEmpty(_settingsTableName));

            modelBuilder.Entity<TSetting>().ToTable(_settingsTableName);

            // configure keys
            var columnOrder = 0;
            var keyNames = SettingKeyNameReadOnlyCollection.Create<TSetting>();
            foreach (var keyName in keyNames)
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
                .Where(p => p.Name == nameof(Setting.Value))
                .Configure(p => p.IsMaxLength());

            base.OnModelCreating(modelBuilder);
        }
    }
}
