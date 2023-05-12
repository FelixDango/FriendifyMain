using System.ComponentModel.DataAnnotations;


namespace FriendifyMain.Models
{
    public class Role
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
        public string Id { get; set; }

        // Foreign key to link to User
        public int UserId { get; set; }

        // Foreign key to link to Role
        public int RoleId { get; set; }

        // Navigation property to link to User
        public User User { get; set; }

        // Navigation property to link to Role
        public Role Role { get; set; }
    }

}
