using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using RestSharp.Extensions.MonoHttp;
using MimeKit;
using MailKit.Net.Smtp;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace OneDirect.Helper
{
    public class Smtp
    {
        private static ILogger logger = Utilities.AppLoggerFactory.CreateLogger<Smtp>();

        public static async void SendEmail(string recipientEmail, string subject, string message)
        {
            logger.LogDebug("Send Mail");
           
            try
            {
                //From Address  
                string FromAddress = ConfigVars.NewInstance.From;
                string FromAdressTitle = ConfigVars.NewInstance.DisplayName;
                //To Address  
                string ToAddress = recipientEmail;
                
                string Subject = subject;
                string BodyContent = message;

                //Smtp Server  
                string SmtpServer = ConfigVars.NewInstance.Host;
                //Smtp Port Number  
                int SmtpPortNumber = ConfigVars.NewInstance.Port;

                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(FromAdressTitle, FromAddress));
                mimeMessage.To.Add(new MailboxAddress("", ToAddress));
                mimeMessage.Subject = Subject;
                mimeMessage.Body = new TextPart("html")
                {
                    Text = BodyContent

                };
                await Task.Delay(1000);
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {

                    client.Connect(SmtpServer, SmtpPortNumber, false);
                    // Note: only needed if the SMTP server requires authentication  
                    // Error 5.5.1 Authentication   
                    client.Authenticate(ConfigVars.NewInstance.From, ConfigVars.NewInstance.Password);
                    client.Send(mimeMessage);
                   
                    client.Disconnect(true);

                }
            }
            catch (Exception ex)
            {
                
                logger.LogDebug("MAIL-ERROR:" + ex.Message, ex);
                
            }
           

        }

        public static bool SendEmail(string recipientEmail, string subject, byte[] pdf)
        {
            try
            {
                string SmtpServer = ConfigVars.NewInstance.Host;// "smtp.gmail.com";
                //Smtp Port Number  
                int SmtpPortNumber = ConfigVars.NewInstance.Port;
                System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient(SmtpServer);
                MailMessage mMessage = new MailMessage();
                string FromAddress = ConfigVars.NewInstance.From;
                string FromAdressTitle = ConfigVars.NewInstance.DisplayName;
                mMessage.From = new MailAddress(FromAddress);


                mMessage.To.Add(recipientEmail);


                mMessage.Subject = subject;
                mMessage.Body = "mail with attachment";
                mMessage.IsBodyHtml = true;

                var contentType = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);
                var myStream = new System.IO.MemoryStream(pdf);
                myStream.Seek(0, SeekOrigin.Begin);

                System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(myStream, "test.pdf");

                mMessage.Attachments.Add(attachment);

                smtpClient.Port = SmtpPortNumber;
                smtpClient.Credentials = new System.Net.NetworkCredential(FromAddress, ConfigVars.NewInstance.Password);
                
                smtpClient.Send(mMessage);
                attachment.Dispose();
                mMessage.Dispose();
                smtpClient.Dispose();
                return true;
            }
            catch (Exception err)
            {
                Console.WriteLine("Error in sending mail: " + err.Message);
                return false;
            }
        }
        public static async void SendGridEmail(string recipientEmail, string subject, string message)
        {
            logger.LogDebug("Send Grid Mail");
            
            try
            {
                var apiKey = ConfigVars.NewInstance.sendgridapi;
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress("prabhun@softtrends.com", "OneDirect Trex Team");
                var lsubject = subject;
                var to = new EmailAddress(recipientEmail);
                var plainTextContent = "";
                var htmlContent = subject;
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                
                logger.LogDebug("MAIL-ERROR:" + ex.Message, ex);
                
            }
            

        }
        public static async void SendGridEmailWithAttachment(string recipientEmail, string subject, string message, string filename, byte[] pdf)
        {
            logger.LogDebug("Send Grid Mail");
            
            try
            {
                var apiKey = ConfigVars.NewInstance.sendgridapi;
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress("prabhun@softtrends.com", "OneDirect Trex Team");
                var lsubject = subject;
                var to = new EmailAddress(recipientEmail);
                var plainTextContent = "";
                var htmlContent = message;
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                msg.Subject = subject;
                SendGrid.Helpers.Mail.Attachment attachment = new SendGrid.Helpers.Mail.Attachment();
                var file = Convert.ToBase64String(pdf);
                msg.AddAttachment(filename, file);
                var response = await client.SendEmailAsync(msg);

            }
            catch (Exception ex)
            {
                
                logger.LogDebug("MAIL-ERROR:" + ex.Message, ex);
               
            }
           

        }
       
    }
}