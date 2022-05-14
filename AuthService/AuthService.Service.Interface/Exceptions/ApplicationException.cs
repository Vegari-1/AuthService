namespace AuthService.Service.Interface.Exceptions
{
	public class ApplicationException : Exception
	{

        public int StatusCode { get; set; }

        public ApplicationException(string message) : base(message) {}
	}
}

