namespace AuthService.Model
{
    public class Profile
    {
		public Guid Id { get; set; }
		public string Email { get; set; }
		public string Username { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public string Gender { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string Phone { get; set; }
		public bool Public { get; set; }
		public string Picture { get; set; }
		public string Biography { get; set; }
	}
}
