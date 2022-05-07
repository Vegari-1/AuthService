namespace AuthService.Model
{
	public class User
	{
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public User(string username, string email, string password, string name, string surname)
        {
            Username = username;
            Email = email;
            Password = password;
            Name = name;
            Surname = surname;
        }
	}
}

