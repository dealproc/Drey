using FluentMigrator;
using System;

namespace Drey.Configuration.Infrastructure.Schema._0100
{
    [Migration(0100201508132117)]
    public class Create_BaseTables : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("Releases")
                .WithColumn("Id").AsString(255).PrimaryKey()
                .WithColumn("Version").AsString(20).PrimaryKey()
                .WithColumn("Description").AsString(Int32.MaxValue)
                .WithColumn("IconUrl").AsString(Int32.MaxValue)
                .WithColumn("Listed").AsBoolean()
                .WithColumn("Published").AsDateTime()
                .WithColumn("ReleaseNotes").AsString(Int32.MaxValue)
                .WithColumn("Summary").AsString(Int32.MaxValue)
                .WithColumn("Tags").AsString(Int32.MaxValue)
                .WithColumn("Title").AsString(Int32.MaxValue)
                .WithColumn("SHA1").AsString(40)
                .WithColumn("Filename").AsString(255)
                .WithColumn("CreatedOn").AsDateTime().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("UpdatedOn").AsDateTime().WithDefault(SystemMethods.CurrentDateTime);

            Create.Table("ConnectionStrings")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("PackageId").AsString(255).Indexed()
                .WithColumn("Name").AsString(100)
                .WithColumn("ConnectionString").AsString(Int32.MaxValue)
                .WithColumn("ProviderName").AsString(100)
                .WithColumn("CreatedOn").AsDateTime().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("UpdatedOn").AsDateTime().WithDefault(SystemMethods.CurrentDateTime);

            Create.Table("PackageSettings")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("PackageId").AsString(255).Indexed()
                .WithColumn("Key").AsString(100)
                .WithColumn("Value").AsString(Int32.MaxValue)
                .WithColumn("CreatedOn").AsDateTime().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("UpdatedOn").AsDateTime().WithDefault(SystemMethods.CurrentDateTime);

            Create.Table("GlobalSettings")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Key").AsString(100)
                .WithColumn("Value").AsString(Int32.MaxValue)
                .WithColumn("CreatedOn").AsDateTime().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("UpdatedOn").AsDateTime().WithDefault(SystemMethods.CurrentDateTime);
        }
    }
}