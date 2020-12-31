using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Hosting;
using cms4seo.Common.Security;
using cms4seo.Data.Repositories;
using cms4seo.Model.Entities;
using cms4seo.Model.LekimaxType;
using cms4seo.Service.Provider;
using cms4seo.Service.Resolver;

namespace cms4seo.Service.Email
{

    // Value of this class must be reset to refresh data
    public class EmailSettings
    {

        public EmailSettings()
        {
            ISettingRepository settingRepository = new SettingRepository();

            Subject = settingRepository.Get(EmailSettingType.Subject);
            MailFromAddress = settingRepository.Get(EmailSettingType.MailFromAddress);
            MailToAddress = settingRepository.Get(EmailSettingType.MailToAddress);
            MailToAddress2 = settingRepository.Get(EmailSettingType.MailToAddress2);
            MailToAddressBcc = settingRepository.Get(EmailSettingType.MailToAddressBcc);

            ServerName = settingRepository.Get(EmailSettingType.ServerName);
            ServerPort = Convert.ToInt16(settingRepository.Get(EmailSettingType.ServerPort));
            Username = settingRepository.Get(EmailSettingType.Username);

            string hashcode = settingRepository.Get(EmailSettingType.Password);
            Password = AesOperation.Decrypt(hashcode);

            UseSsl = Convert.ToBoolean(settingRepository.Get(EmailSettingType.UseSsl));

            WriteAsFile = Convert.ToBoolean(settingRepository.Get(EmailSettingType.WriteAsFile));
            FileLocation = settingRepository.Get(EmailSettingType.FileLocation) ?? "/Mail/";
        }


        public string Subject { get; set; }
        public string MailFromAddress { get; set; }
        public string MailToAddress { get; set; }
        public string MailToAddress2 { get; set; }
        public string MailToAddressBcc { get; set; }

        public string ServerName { get; set; }
        public int ServerPort { get; set; }
        public string Username { get; set; }

        public string Password { get; set; }

        public bool UseSsl { get; set; }

        public bool WriteAsFile { get; set; }
        public string FileLocation { get; set; }
    }

    public class EmailProcessor : IOrderProcessor
    {
        //private EmailSettings emailSettings;

        //public EmailProcessor(EmailSettings settings)
        //{
        //    emailSettings = settings;
        //}

        //public EmailProcessor()
        //{
        //    emailSettings = new EmailSettings();
        //}


        public void ProcessOrder(Cart cart, ShippingDetails shippingInfo)
        {
            EmailSettings emailSettings = new EmailSettings();

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials
                    = new NetworkCredential(emailSettings.Username,
                        emailSettings.Password);
                if (emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod
                        = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation =
                        HostingEnvironment.MapPath(emailSettings.FileLocation);
                    smtpClient.EnableSsl = false;
                }
                var body = new StringBuilder()
                    .AppendLine("A new order has been submitted")
                    .AppendLine("---")
                    .AppendLine("Items:");
                foreach (var line in cart.Lines)
                {
                    var subtotal = line.Product.Price * line.Quantity;
                    body.AppendFormat("{0}  x  {1}  (subtotal:  {2:c}",
                        line.Quantity,
                        line.Product.Name,
                        subtotal);
                }
                body.AppendFormat("Total  order  value:  {0:c}",
                    cart.ComputeTotalValue())
                    .AppendLine("---")
                    .AppendLine("Ship to:")
                    .AppendLine(shippingInfo.Name)
                    .AppendLine(shippingInfo.Line1)
                    .AppendLine(shippingInfo.Line2 ?? "")
                    .AppendLine(shippingInfo.Line3 ?? "")
                    .AppendLine(shippingInfo.City)
                    .AppendLine(shippingInfo.State ?? "")
                    .AppendLine(shippingInfo.Country)
                    .AppendLine(shippingInfo.Zip)
                    .AppendLine("---")
                    .AppendFormat("Gift wrap: {0}",
                        shippingInfo.GiftWrap ? "Yes" : "No");

                var mailMessage = new MailMessage(
                    emailSettings.MailFromAddress, // From
                    emailSettings.MailToAddress, // To
                    "New order submitted!", // Subject
                    body.ToString()); // Body

                if (emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }
                smtpClient.Send(mailMessage);
            }
        }


        public string ProcessContact(string name, string email, string phone, string message)
        {
            EmailSettings emailSettings = new EmailSettings();


            if (string.IsNullOrWhiteSpace(emailSettings.Subject))
                return Setting.Contents["Emails.Template.Empty"];

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);
                if (emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod
                        = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation =
                        HostingEnvironment.MapPath(emailSettings.FileLocation);
                    smtpClient.EnableSsl = false;
                }


                var body = new StringBuilder()
                    .AppendLine(Setting.Contents["Emails.Template.Header"])
                    .AppendLine(Setting.Contents["Emails.Template.Fullname"] + name)
                    .AppendLine(Setting.Contents["Emails.Template.PhoneNumber"] + phone)
                    .AppendLine(Setting.Contents["Emails.Template.EmailAddress"] + email)
                    .AppendLine(message);


                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(emailSettings.MailFromAddress);

                    // To address 1
                    mailMessage.To.Add(emailSettings.MailToAddress);

                    // To address 2
                    if (!string.IsNullOrEmpty(emailSettings.MailToAddress2))
                    {
                        mailMessage.To.Add(emailSettings.MailToAddress2);
                    }

                    // To Bcc
                    if (!string.IsNullOrEmpty(emailSettings.MailToAddressBcc))
                    {
                        mailMessage.Bcc.Add(emailSettings.MailToAddressBcc);
                    }

                    // subject 
                    mailMessage.Subject = emailSettings.Subject;

                    // body
                    mailMessage.Body = body.ToString();

                    // deliver report
                    mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                    // send protofiles
                    if (emailSettings.WriteAsFile)
                    {
                        mailMessage.BodyEncoding = Encoding.ASCII;
                    }
                    smtpClient.Send(mailMessage);
                }
            }

            return null;
        }



        /// <summary>
        /// Send email link to reset password
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="callbackUrl">reset password link</param>
        public void ResetPassword(string email, string callbackUrl)
        {

            EmailSettings emailSettings = new EmailSettings();

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);
                if (emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod
                        = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation =
                        HostingEnvironment.MapPath(emailSettings.FileLocation);
                    smtpClient.EnableSsl = false;
                }
                var body = new StringBuilder()
                    .AppendLine($"You can click this link to reset password: {callbackUrl}");


                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(emailSettings.MailFromAddress);

                    // To user need reset link
                    mailMessage.To.Add(email);


                    // To Bcc
                    if (!string.IsNullOrEmpty(emailSettings.MailToAddressBcc) && emailSettings.MailToAddressBcc != "0")
                    {
                        mailMessage.Bcc.Add(emailSettings.MailToAddressBcc);
                    }

                    // subject 
                    mailMessage.Subject = "Reset Password - cms4seo.com";

                    // body
                    mailMessage.Body = body.ToString();

                    // deliver report
                    mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                    // send protofiles
                    if (emailSettings.WriteAsFile)
                    {
                        mailMessage.BodyEncoding = Encoding.ASCII;
                    }
                    smtpClient.Send(mailMessage);
                }
            }
        }



        /// <summary>
        /// Send email with custom
        /// </summary>
        /// <param name="destination">destination</param>
        /// <param name="subject">subject</param>
        /// <param name="body">body</param>
        public void SendMail(string destination, string subject, string body)
        {

            EmailSettings emailSettings = new EmailSettings();

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);
                if (emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod
                        = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation =
                        HostingEnvironment.MapPath(emailSettings.FileLocation);
                    smtpClient.EnableSsl = false;
                }


                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(emailSettings.MailFromAddress);

                    // To user need reset link
                    mailMessage.To.Add(destination);


                    // To Bcc
                    if (!string.IsNullOrEmpty(emailSettings.MailToAddressBcc) && emailSettings.MailToAddressBcc != "0")
                    {
                        mailMessage.Bcc.Add(emailSettings.MailToAddressBcc);
                    }

                    // subject 
                    mailMessage.Subject = subject;

                    // body
                    mailMessage.Body = body;

                    // deliver report
                    mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                    // send protofiles
                    if (emailSettings.WriteAsFile)
                    {
                        mailMessage.BodyEncoding = Encoding.ASCII;
                    }
                    smtpClient.Send(mailMessage);
                }
            }
        }




        public static string HtmlEncode(string text)
        {
            var chars = HttpUtility.HtmlEncode(text)?.ToCharArray();
            var result = new StringBuilder(text.Length + (int)(text.Length * 0.1));

            if (chars != null)
                foreach (var c in chars)
                {
                    var value = Convert.ToInt32(c);
                    if (value > 127)
                        result.AppendFormat("&#{0};", value);
                    else
                        result.Append(c);
                }

            return result.ToString();
        }
    }
}