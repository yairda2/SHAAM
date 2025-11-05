using System;
using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.Models
{
    /// <summary>
    /// Domain model representing a user entity in the system
    /// </summary>
    public class User
    {
        /// <summary>
        /// Unique identifier for the user
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Full name of the user
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Email address of the user, must be unique
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Contact phone number, optional
        /// </summary>
        [MaxLength(20)]
        public string? Phone { get; set; }

        /// <summary>
        /// Website URL, optional
        /// </summary>
        [MaxLength(255)]
        public string? Website { get; set; }

        /// <summary>
        /// Company name, optional
        /// </summary>
        [MaxLength(255)]
        public string? Company { get; set; }

        /// <summary>
        /// Timestamp when the user was created in UTC
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when the user was last updated in UTC
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
