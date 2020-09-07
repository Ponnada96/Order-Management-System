using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Business.Helper
{
    /// <summary>
    /// Smtp Helpet Class
    /// </summary>
    public class SmtpHelper
    {
        private const string fromEmail = "durgaprasadponnadaa@gmail.com";
        private const string password = "Prasad@0637";
        private const int port = 587;
        private const   string subject = "Order Status";
        public static bool SendEmail(string UserEmailAddress, string UserName)
        {
            using (var mail = new MailMessage())
            {
                mail.From = new MailAddress(fromEmail);
                mail.To.Add(UserEmailAddress);
                mail.Subject = subject;
                mail.Body = $"Hellow '{UserName}' ,your order has been placed";
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", port))
                {
                    smtp.Credentials = new NetworkCredential(fromEmail, password);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                    return true;
                }
            }
        }
    }
}
