﻿using GeneralUtils;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using NLog;
using TenBis.Interfaces;

namespace TenBis.Classes.Notifiers
{
    internal class EmailCommunication : ICommunication
    {
        private readonly string _notifyTo;
        private readonly IAggregate _aggrgate;
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public EmailCommunication(string notifyTo, IAggregate aggrgate)
        {
            _notifyTo = notifyTo;
            _aggrgate = aggrgate;
        }

        public void NotifyContact(string message)
        {
            if (string.IsNullOrEmpty(_notifyTo))
            {
                return;
            }

            MimeMessage mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("Ten Bis Notifier", EmailInformation.Address));
            mimeMessage.To.Add(MailboxAddress.Parse(_notifyTo));
            mimeMessage.Subject = "Ten Bis";
            mimeMessage.Body = new TextPart(TextFormat.Text)
            {
                Text = message
            };

            SmtpClient client = new SmtpClient();
            try
            {
                client.Connect(EmailInformation.Host, EmailInformation.Port, EmailInformation.UseSSL);
                client.Authenticate(EmailInformation.Address, EmailInformation.Password);
                client.Send(mimeMessage);
            }
            catch (Exception exception)
            {

                Console.WriteLine(exception);
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }

        public void ValidateRunningScript()
        {
            string? message = _aggrgate?.Aggregate();
            NotifyContact(message!);
            Environment.Exit(0);
        }

        public void AlertContactAboutScript()
        {
        }
    }
}
