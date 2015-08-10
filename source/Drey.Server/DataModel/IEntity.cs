using System;
using System.ComponentModel.DataAnnotations;

namespace Drey.Server.DataModel
{
    /// <summary>
    /// The base entity interface in the system.  Basically, this is allowing for the use of generics at the repository level.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [Key]
        long Id { get; set; }

        /// <summary>
        /// Gets or sets the created on UTC.
        /// </summary>
        DateTimeOffset CreatedOnUTC { get; set; }
        
        /// <summary>
        /// Gets or sets the updated on UTC.
        /// </summary>
        DateTimeOffset UpdatedOnUTC { get; set; }
    }
}