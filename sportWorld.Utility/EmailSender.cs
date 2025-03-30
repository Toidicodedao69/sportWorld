using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace sportWorld.Utility
{
	public class EmailSender : IEmailSender
	{
		private string _sendGridSecret;
		public EmailSender(IConfiguration _config)
		{
			if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
			{
				_sendGridSecret = Environment.GetEnvironmentVariable("SendGrid_SecretKey");
			}
				_sendGridSecret = _config.GetValue<string>("SendGrid:SecretKey");
		}
		public Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			var client = new SendGridClient(_sendGridSecret);

			var from = new EmailAddress("clashofclangodson123@gmail.com", "Sport World");
			var to = new EmailAddress(email);
			var message = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);

			return client.SendEmailAsync(message);
		}
	}
}
