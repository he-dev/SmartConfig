using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using SmartConfig.Collections;
using SmartConfig.Data;

namespace SmartConfig.DataStores.SqlServer
{
    /// <summary>
    /// Provides <c>DbContext</c> for retreiving configuration from a database.
    /// </summary>
    internal sealed class SqlServerContext<TSetting> : DbContext where TSetting : BasicSetting, new()
    {
        private readonly string _settingTableName;

        public SqlServerContext(string connectionString, string settingTableName) : base(connectionString)
        {
            Debug.Assert(!string.IsNullOrEmpty(connectionString));
            Debug.Assert(!string.IsNullOrEmpty(settingTableName));

            _settingTableName = settingTableName;
            //Database.SetInitializer<SqlServerContext<TSetting>>(null);
        }

        /// <summary>
        /// Gets or sets configuration elements.
        /// </summary>
        public DbSet<TSetting> Settings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Debug.Assert(!string.IsNullOrEmpty(_settingTableName));

            modelBuilder.Entity<TSetting>().ToTable(_settingTableName);

            // configure keys
            var columnOrder = 0;
            var key = SettingKey.From<TSetting>();
            foreach (var name in key.Select(x => x.Key))
            {
                var columnOrderClosure = columnOrder++;
                modelBuilder
                    .Properties<string>()
                    .Where(p => p.Name == name)
                    .Configure(p => p.IsKey().HasColumnOrder(columnOrderClosure).HasMaxLength(200));
            }

            // configure value column
            modelBuilder
                .Properties<string>()
                .Where(p => p.Name == nameof(BasicSetting.Value))
                .Configure(p => p.IsMaxLength());

            base.OnModelCreating(modelBuilder);
        }
    }
}
