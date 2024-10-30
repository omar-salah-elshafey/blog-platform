using System.ComponentModel.DataAnnotations;

namespace UserAuthentication.DTO_s
{
    public class UpdateUserDto
    {
        [Required]
        public string UserName { get; set; }
        [Required, MaxLength(50)]
        public string FirstName { get; set; }
        [Required, MaxLength(50)]
        public string LastName { get; set; }
    }
}
