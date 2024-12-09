using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SIMCMD.Core;

namespace SIMCMD.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;
        private readonly Regex _expression;
        private readonly bool _validateEmail;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _sender;
        private readonly string _username;
        private readonly string _password;
        private readonly string[] _recipients;
        private readonly bool _enabled;
        private readonly bool _smtpSSL;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _validateEmail = Convert.ToBoolean(_configuration.GetValue<string>("ServiceSetting:EmailConfig:ValidateEmail"));
            _expression = new Regex(_configuration.GetValue<string>("ServiceSetting:EmailConfig:EmailValidatePattern"));
            _sender = _configuration.GetValue<string>("ServiceSetting:EmailConfig:Sender");
            _smtpHost = _configuration.GetValue<string>("ServiceSetting:EmailConfig:SmtpHost");
            _smtpPort = Convert.ToInt32(_configuration.GetValue<string>("ServiceSetting:EmailConfig:SmtpPort"));
            _username = _configuration.GetValue<string>("ServiceSetting:EmailConfig:Username");
            _password = _configuration.GetValue<string>("ServiceSetting:EmailConfig:Password");
            _enabled = Convert.ToBoolean(_configuration.GetValue<string>("ServiceSetting:EmailEnabled"));
            _smtpSSL = Convert.ToBoolean(_configuration.GetValue<string>("ServiceSetting:EmailConfig:SmtpSsl"));
            _recipients = _configuration.GetValue<string>("ServiceSetting:EmailConfig:Recipients").Split(";");
        }

        public Result EmailSend(EmailMessage message)
        {
            if (!_enabled)
            {
                _logger.LogDebug("SMTP function has been disabled");
                return Result.Ok();
            }

            if (message.Recipients.Length == 0)
            {
                _logger.LogDebug("Email recipient is required. Trying appsetting file for data.");
                message.Recipients = _recipients;

                if (message.Recipients.Length == 0)
                    return Result.Fail("Appsetting variable null. Email recipient is required");
            }

            try
            {
                _logger.LogDebug($"Sending email to {string.Join(", ", message.Recipients)}");
                using (var client = new SmtpClient(_smtpHost, _smtpPort))
                {
                    client.Credentials = new NetworkCredential(_username, _password);
                    client.EnableSsl = _smtpSSL;
                    using (var mail = new MailMessage())
                    {
                        foreach (string email in message.Recipients)
                        {
                            if (!ValidateEmailAddress(email)) continue;
                            mail.To.Add(new MailAddress(email));
                        }

                        if (!string.IsNullOrEmpty(message.Sender))
                        {
                            mail.From = new MailAddress(message.Sender);
                        }
                        else
                        {
                            var sender = _sender;
                            mail.From = new MailAddress(sender);
                        }

                        mail.Subject = message.Subject;
                        mail.Body = message.Message;
                        mail.IsBodyHtml = message.SendAsHtml;
                        mail.BodyEncoding = Encoding.UTF8;
                        mail.Priority = message.Priority;

                        if (message.Cc != null && message.Cc.Length != 0)
                        {
                            foreach (string email in message.Cc)
                            {
                                if (!ValidateEmailAddress(email)) continue;
                                mail.CC.Add(new MailAddress(email));
                            }
                        }

                        if (message.Bcc != null && message.Bcc.Length != 0)
                        {
                            foreach (string email in message.Bcc)
                            {
                                if (!ValidateEmailAddress(email)) continue;
                                mail.Bcc.Add(new MailAddress(email));
                            }
                        }

                        if (message.Attachments != null && message.Attachments.Length != 0)
                        {
                            foreach (Attachment attachment in message.Attachments)
                            {
                                mail.Attachments.Add(attachment);
                            }
                        }
                        client.Send(mail);
                    }
                }
                _logger.LogDebug("Email sent successfully");
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email");
                return Result.Fail("Error sending email");
            }
        }

        private bool ValidateEmailAddress(string email)
        {
            if (!_validateEmail) return true;
            return _expression.IsMatch(email);
        }
    }
}


