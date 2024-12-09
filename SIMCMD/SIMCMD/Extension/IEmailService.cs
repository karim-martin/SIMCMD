using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SIMCMD.Core
{
    public interface IEmailService 
    {
        public Result EmailSend(EmailMessage message);
    }

    public struct EmailMessage
    {
        public EmailMessage(string subject, string message, string recipient, bool sendAsHtml = false, MailPriority priority = MailPriority.Normal)
            : this("", subject, message, recipient.Split(';'), null, null, null, sendAsHtml, priority)
        {

        }

        public EmailMessage(string subject, string message, string recipient, Attachment[] attachments, bool sendAsHtml = false, MailPriority priority = MailPriority.Normal)
            : this("", subject, message, recipient.Split(';'), null, null, attachments, sendAsHtml, priority)
        {

        }

        public EmailMessage(string sender, string subject, string message, string[] recipients, bool sendAsHtml = false, MailPriority priority = MailPriority.Normal)
         : this(sender, subject, message, recipients, null, null, null, sendAsHtml, priority)
        {

        }

        public EmailMessage(string sender, string subject, string message, string[] recipients, string[] cc, string[] bcc, Attachment[] attachments, bool sendAsHtml = false, MailPriority priority = MailPriority.Normal)
        {
            if (string.IsNullOrEmpty(subject)) throw new ArgumentNullException(nameof(subject));
            if (string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message));
            if (recipients.Length == 0) throw new ArgumentNullException(nameof(recipients));

            Sender = sender;
            Subject = subject;
            Message = message;
            Recipients = recipients;
            Cc = cc;
            Bcc = bcc;
            Attachments = attachments;
            SendAsHtml = sendAsHtml;
            Priority = priority;
        }

        public EmailMessage(string from, string subject, string message)
        {
            if (string.IsNullOrEmpty(subject)) throw new ArgumentNullException(nameof(subject));
            if (string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message));

            Subject = subject;
            Message = message;
            From = from;
        }

        public string Sender { get; set; }
        public string Subject { get;  set; }
        public string Message { get; set; }
        public string From { get; set; }
        public string[] Recipients { get;  set; }
        public string[] Cc { get; }
        public string[] Bcc { get; }
        public Attachment[] Attachments { get; set; }
        public bool SendAsHtml { get; set; }
        public MailPriority Priority { get; set; }
    }
}
