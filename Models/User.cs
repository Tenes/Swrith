using System.ComponentModel.DataAnnotations;

namespace Miniblog.Core.Models
{
    public class User
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public bool IsAdmin { get; set; }
    }
}