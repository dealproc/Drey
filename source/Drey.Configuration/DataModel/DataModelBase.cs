using System;
using System.ComponentModel.DataAnnotations;

namespace Drey.Configuration.DataModel
{
    /// <summary>
    /// A base entity in the system.
    /// </summary>
    public abstract class DataModelBase : IEntity
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the date and time that this record was created.
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the last time this record has beeen updated.
        /// </summary>
        public DateTime UpdatedOn { get; set; }
    }
}