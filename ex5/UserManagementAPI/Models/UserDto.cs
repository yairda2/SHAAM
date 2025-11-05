using System;

namespace UserManagementAPI.Models
{
    /// <summary>
    /// Data Transfer Object for returning user data with all fields including timestamps
    /// </summary>
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Website { get; set; }
        public string? Company { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
