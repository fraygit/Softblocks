using softblocks.data.Model;
using softblocks.data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Mail;
using System.Net;

namespace softblocks.EmailSender
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                MainAsync().Wait();
            }
            catch (Exception ex)
            {

            }
        }

        static async Task MainAsync()
        {

            var emailRepository = new EmailNotificationRepository();
            var emailToBeSent = await emailRepository.GetByStatus(0);
            foreach (var email in emailToBeSent)
            {
                email.Status = 2;
                await emailRepository.Update(Convert.ToString(email.Id), email);
            }
            foreach (var email in emailToBeSent)
            {
                Send(email);
            }

        }

        private static void Send(EmailNotification request)
        {
            using (ManualResetEvent waitHandle = new ManualResetEvent(false)) // handle blocking of asynchronous sending
            {
                var emailRepo = new EmailNotificationRepository();
                var message = new MailMessage();
                message.Subject = request.Subject;

                message.Body = request.Message;
                var from = request.From;
                message.From = new MailAddress(from);
                message.ReplyTo = new MailAddress("system@softblocks.co.nz");

                foreach (var to in request.To)
                {
                    message.To.Add(to);
                }

                if (request.IsHtml)
                {
                    message.IsBodyHtml = true;
                }

                var smtp = new SmtpClient();
                smtp.Host = ConfigurationManager.AppSettings["smtphost"];
                smtp.Port = int.Parse(ConfigurationManager.AppSettings["smtpport"]);
                smtp.UseDefaultCredentials = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["smtpusername"], ConfigurationManager.AppSettings["smtppassword"]);
                smtp.EnableSsl = ConfigurationManager.AppSettings["smtpssl"].ToLower() == "true";
                var token = request.Id;

                smtp.SendCompleted += (s, e) =>
                {
                    if (e.Error != null)
                    {
                        request.Status = -1;

                        var emailErrorRepo = new EmailErrorRepository();
                        emailErrorRepo.CreateSync(new EmailError
                        {
                            EmailId = request.Id.ToString(),
                            ErrorDate = DateTime.Now,
                            Message = e.Error.Message,
                            Source = e.Error.Source,
                            StackTrace = e.Error.StackTrace
                        }).Wait();
                    }
                    else
                    {
                        request.Status = 1;
                    }
                    emailRepo.Update(Convert.ToString(request.Id), request).Wait();

                    waitHandle.Set(); // tell handler that sending completed
                };

                smtp.SendCompleted += (s, e) =>
                {
                    message.Dispose();
                    smtp.Dispose();
                };

                try
                {
                    smtp.SendAsync(message, token);
                }
                catch (SmtpException e)
                {
                    var errormessage = e.StatusCode;
                }
                waitHandle.WaitOne(); // tell handler to wait until sending completed
            }
        }
    }
}
