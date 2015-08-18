using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace SmartConfig.Data
{
    /// <summary>
    /// Provides <c>DbContext</c> for retreiving configuration from a database.
    /// </summary>
    public sealed class SmartConfigEntities<TSetting> : DbContext where TSetting : class
    {
        public SmartConfigEntities(string connectionString)
            : base(connectionString)
        {
            Debug.Assert(!string.IsNullOrEmpty(connectionString));
        }
        
        public string SettingsTableName { get; set; }

        /// <summary>
        /// Gets or sets config elements.
        /// </summary>
        public DbSet<TSetting> Settings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TSetting>().ToTable(SettingsTableName);

            // create a list with key names and initialize it with the default key
            var keyNames = new List<string> { KeyNames.DefaultKeyName };

            // get other keys but sort them alphabeticaly
            var declaredProperties = typeof(TSetting).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            keyNames.AddRange(declaredProperties.OrderBy(p => p.Name).Select(p => p.Name));
            
            var columnOrder = 0;
            foreach (var keyName in keyNames)
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
