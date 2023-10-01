using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TaletApi.Models
{
    [Index(nameof(User.UserName), IsUnique = true)]
    public class User
        {
            [Key]
            public int Id { get; set; }
            [Required]
            [MaxLength(50)]
            public string UserName { get; set; }
            public string? PasswordHash { get; set; }
        }
    
}
