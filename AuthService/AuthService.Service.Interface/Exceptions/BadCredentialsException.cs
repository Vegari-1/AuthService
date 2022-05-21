using System;
namespace AuthService.Service.Interface.Exceptions
{
	public class BadCredentialsException : BaseException
	{
		public BadCredentialsException(string message) : base(message)
		{
			StatusCode = 401;
		}
	}
}

