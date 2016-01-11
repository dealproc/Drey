using System;
using System.ComponentModel.DataAnnotations;

namespace Drey.Configuration.DataModel
{
    /// <summary>
    /// The base entity interface in the system.  Basically, this allows generics in the repository.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [Key]
        int Id { get; set; }

        /// <summary>
        /// Gets or sets the date and time that this record was created.
        /// </summary>
        DateTime CreatedOn { get; set; }
        
        /// <summary>
        /// Gets or sets the last time this record has beeen updated.
        /// </summary>
        DateTime UpdatedOn { get; set; }
    }
}