using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;


namespace FriendifyMain.Models
{
    public class Role : IdentityRole<int>
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        // Add other properties as needed

        // Navigation property to link to AssignedRole
        public List<AssignedRole> AssignedRoles { get; set; }
    }

    public class AssignedRole
    {
        [Key]
        public int Id { get; set; }

        // Foreign key to link to User
        public int UserId { get; set; }

        // Foreign key to link to Role
        public int RoleId { get; set; }

        // Navigation property to link to User
        [JsonIgnore]
        public User User { get; set; }

        // Navigation property to link to Role
        public Role Role { get; set; }
    }

}
