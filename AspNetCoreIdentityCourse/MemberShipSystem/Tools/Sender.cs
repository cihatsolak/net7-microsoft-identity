using System;
using System.Net;
using System.Net.Mail;

namespace MemberShip.Web.Tools
{
    public static class Sender
    {
        public static void PasswordResetMail(string link, string address)
        {
            try
            {
                using MailMessage mail = new MailMessage();
                using SmtpClient smtpClient = new SmtpClient("mail.teknohub.net");

                mail.From = new MailAddress("fcakiroglu@teknohub.net");
                mail.To.Add(address);

                mail.Subject = "www.identity.com::Şifre Sıfırlama";
                mail.Body = "<h2>Şifrenizi yenilemek için linke tıklayınız";
                mail.Body += $"<a href='{link}'>Şifre yenileme linki</a>";

                mail.IsBodyHtml = true;
                smtpClient.Port = 587;

                smtpClient.Credentials = new NetworkCredential("fcakiroglu@teknohub.net", "FatihFatih30");
                smtpClient.Send(mail);
            }
            catch (Exception)
            {
            }
        }

        public static void EmailVerification(string link, string address)
        {
            try
            {
                using MailMessage mail = new MailMessage();
                using SmtpClient smtpClient = new SmtpClient("mail.teknohub.net");

                mail.From = new MailAddress("fcakiroglu@teknohub.net");
                mail.To.Add(address);

                mail.Subject = "www.identity.com::Email Doğrulama";
                mail.Body = "<h2>ŞEmail adresini doğrulamak için linke tıklayınız";
                mail.Body += $"<a href='{link}'>Email doğrulama linki</a>";

                mail.IsBodyHtml = true;
                smtpClient.Port = 587;

                smtpClient.Credentials = new NetworkCredential("fcakiroglu@teknohub.net", "FatihFatih30");
                smtpClient.Send(mail);
            }
            catch (Exception)
            {
            }
        }
    }
}
