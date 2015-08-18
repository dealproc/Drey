using FluentMigrator;
using System;

namespace Drey.Configuration.Infrastructure.Schema._0100
{
    [Migration(0100201508132117)]
    public class Create_BaseTables : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("RegisteredPackages")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("PackageId").AsString(255)
                .WithColumn("CreatedOn").AsDateTime().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("UpdatedOn").AsDateTime().WithDefault(SystemMethods.CurrentDateTime);

            Create.Table("Releases")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("RegisteredPackageId").AsString(255).ForeignKey("FK_Releases_RegisteredPackages_Id", "RegisteredPackages", "Id")
                .WithColumn("SHA1").AsString(40)
                .WithColumn("Filename").AsString(255)
                .WithColumn("Ordinal").AsInt32()
                .WithColumn("CreatedOn").AsDateTime().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("UpdatedOn").AsDateTime().WithDefault(SystemMethods.CurrentDateTime);

            Create.Table("ConnectionStrings")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("RegisteredPackageId").AsString(255).ForeignKey("FK_ConnectionStrings_RegisteredPackages_Id", "RegisteredPackages", "Id")
                .WithColumn("Name").AsString(100)
                .WithColumn("ConnectionString").AsString(Int32.MaxValue)
                .WithColumn("ProviderName").AsString(100)
                .WithColumn("CreatedOn").AsDateTime().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("UpdatedOn").AsDateTime().WithDefault(SystemMethods.CurrentDateTime);

            Create.Table("PackageSettings")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("RegisteredPackageId").AsString(255).ForeignKey("FK_PackageSettings_RegisteredPackages_Id", "RegisteredPackages", "Id")
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