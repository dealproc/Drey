using FluentMigrator;
using System;

namespace Drey.Configuration.Infrastructure.Schema._0100
{
    /// <summary>
    /// Registers the Drey.Configuration package in the repository.
    /// </summary>
    [Migration(0100201508241409)]
    public class Register_DreyConfigurationApp : Migration
    {
        /// <summary>
        /// Forward migrations.
        /// </summary>
        public override void Up()
        {
            Insert.IntoTable("Releases")
                .Row(new
                {
                    Id = "Drey.Configuration",
                    Version = "1.0.0.0",
                    Description = "Configuration Services Control",
                    IconUrl = "",
                    Listed = true,
                    Published = default(DateTime),
                    ReleaseNotes = "",
                    Summary = "",
                    Tags = "",
                    Title = "Drey - Configuration Web Console",
                    SHA1 = string.Empty,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now
                });
        }

        /// <summary>
        /// Reversal migrations.
        /// </summary>
        public override void Down()
        {
            // no-op
        }
    }
}