namespace AuthService.Dto
{
	public class RegisterResponse
	{

        public Guid Id { get; set; }
		public string Email { get; set; }
		public string Username { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }

	}
}

