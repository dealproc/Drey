using System;
using System.ComponentModel.DataAnnotations;

namespace Drey.Configuration.DataModel
{
    public abstract class DataModelBase : IEntity
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the created on UTC.
        /// </summary>
        public DateTimeOffset CreatedOnUTC { get; set; }
        
        /// <summary>
        /// Gets or sets the updated on UTC.
        /// </summary>
        public DateTimeOffset UpdatedOnUTC { get; set; }
    }
}