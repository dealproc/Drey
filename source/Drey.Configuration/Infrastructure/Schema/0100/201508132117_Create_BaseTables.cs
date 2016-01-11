using FluentMigrator;
using System;

namespace Drey.Configuration.Infrastructure.Schema._0100
{
    /// <summary>
    /// Base Tables migration.
    /// </summary>
    [Migration(0100201508132117)]
    public class Create_BaseTables : AutoReversingMigration
    {
        /// <summary>
        /// Steps to migrate the database forward.
        /// </summary>
        public override void Up()
        {
            Create.Table("Releases")
                .WithColumn("Id").AsString(255).PrimaryKey()
                .WithColumn("Version").AsString(20).PrimaryKey()
                .WithColumn("Description").AsString(Int32.MaxValue).Nullable()
                .WithColumn("IconUrl").AsString(Int32.MaxValue).Nullable()
                .WithColumn("Listed").AsBoolean().Nullable()
                .WithColumn("Published").AsDateTime().Nullable()
                .WithColumn("ReleaseNotes").AsString(Int32.MaxValue).Nullable()
                .WithColumn("Summary").AsString(Int32.MaxValue).Nullable()
                .WithColumn("Tags").AsString(Int32.MaxValue).Nullable()
                .WithColumn("Title").AsString(Int32.MaxValue).Nullable()
                .WithColumn("SHA1").AsString(40)
                .WithColumn("CreatedOn").AsDateTime().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("UpdatedOn").AsDateTime().WithDefault(SystemMethods.CurrentDateTime);

            Create.Table("ConnectionStrings")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("PackageId").AsString(255)
                .WithColumn("Name").AsString(100)
                .WithColumn("ConnectionString").AsString(Int32.MaxValue)
                .WithColumn("ProviderName").AsString(100)
                .WithColumn("CreatedOn").AsDateTime().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("UpdatedOn").AsDateTime().WithDefault(SystemMethods.CurrentDateTime);

            Create.Index("IX_ConnectionStrings_PackageId_Name")
                .OnTable("ConnectionStrings")
                .OnColumn("PackageId").Ascending()
                .OnColumn("Name").Ascending()
                .WithOptions().Unique();

            Create.Table("PackageSettings")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("PackageId").AsString(255)
                .WithColumn("Key").AsString(100)
                .WithColumn("Value").AsString(Int32.MaxValue)
                .WithColumn("CreatedOn").AsDateTime().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("UpdatedOn").AsDateTime().WithDefault(SystemMethods.CurrentDateTime);

            Create.Index("IX_PackageSettings_PackageId_Key")
                .OnTable("PackageSettings")
                .OnColumn("PackageId").Ascending()
                .OnColumn("Key").Ascending()
                .WithOptions().Unique();

            Create.Table("GlobalSettings")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Key").AsString(100)
                .WithColumn("Value").AsString(Int32.MaxValue)
                .WithColumn("CreatedOn").AsDateTime().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("UpdatedOn").AsDateTime().WithDefault(SystemMethods.CurrentDateTime);
        }
    }
}