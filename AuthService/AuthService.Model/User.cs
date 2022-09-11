using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Model
{
    [Table("Users", Schema = "auth")]
    public class User
	{
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

	}
}

