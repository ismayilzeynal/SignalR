using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;

namespace FrontToBack.Services.Email
{
    public class EmailSend : IEmailSend
    {

        public void Send(string userEmail, string subject, string body)
        {
            // create (complete) email message 
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("ismayilbz@code.edu.az")); // config ile de yazmaq olar
            email.To.Add(MailboxAddress.Parse(userEmail));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };


            // send email message
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);  //465 for gmail new
            smtp.Authenticate("ismayilbz@code.edu.az", "apaswqtxbhcriskl");
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
