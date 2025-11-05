using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.Models
{
    /// <summary>
    /// Data Transfer Object for updating an existing user
    /// </summary>
    public class UpdateUserDto
    {
        /// <summary>
        /// Full name of the user
        /// </summary>
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Email address of the user
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email must be a valid email address")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Contact phone number, optional
        /// </summary>
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string? Phone { get; set; }

        /// <summary>
        /// Website URL, optional
        /// </summary>
        [StringLength(255, ErrorMessage = "Website URL cannot exceed 255 characters")]
        public string? Website { get; set; }

        /// <summary>
        /// Company name, optional
        /// </summary>
        [StringLength(255, ErrorMessage = "Company name cannot exceed 255 characters")]
        public string? Company { get; set; }
    }
}
