using FluentResults;
using GeneralUtils;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System;
using TenBis.Interfaces;
using TenBis.Logging;
using TenBis.SettingsFolder.Models;

namespace TenBis.Classes.Notifiers
{
    internal class EmailCommunicator : ICommunicator
    {
        private readonly EmailSettings m_EmailSettings;

        public EmailCommunicator(EmailSettings? emailSettings)
        {
            Logger.FunctionStarted();

            if (emailSettings is null)
            {
                throw new ArgumentNullException(nameof(emailSettings));
            }

            if (string.IsNullOrWhiteSpace(emailSettings.SmtpServer))
            {
                throw new ArgumentNullException(nameof(emailSettings));
            }

            if (string.IsNullOrWhiteSpace(emailSettings.Username))
            {
                throw new ArgumentNullException(nameof(emailSettings));
            }

            if (string.IsNullOrWhiteSpace(emailSettings.Password))
            {
                throw new ArgumentNullException(nameof(emailSettings));
            }

            if (string.IsNullOrWhiteSpace(emailSettings.RecipientEmail))
            {
                throw new ArgumentNullException(nameof(emailSettings));
            }

            m_EmailSettings = emailSettings;

            Logger.FunctionFinished();
        }

        Task<Result> ICommunicator.SendValidationMessage()
        {
            Logger.FunctionStarted();
            Logger.FunctionFinished();
            return Task.FromResult(Result.Ok());
        }


        Task<Result> ICommunicator.SendMessage(string message)
        {
            Logger.FunctionStarted();

            MimeMessage mimeMessage = new MimeMessage
            {
                Subject = "Ten Bis",
                Body = new TextPart(TextFormat.Text) { Text = message }
            };
            mimeMessage.From.Add(new MailboxAddress("Ten Bis Notifier", EmailInformation.Address));
            mimeMessage.To.Add(MailboxAddress.Parse(m_EmailSettings.RecipientEmail));

            try
            {
                using SmtpClient client = new SmtpClient();
                client.Connect(m_EmailSettings.SmtpServer, m_EmailSettings.Port, useSsl: true);
                client.Authenticate(m_EmailSettings.Username, m_EmailSettings.Password);
                client.Send(mimeMessage);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message);
                return Task<Result>.FromResult(Result.Fail("Failed to send email").WithError(exception.Message));
            }

            Logger.FunctionFinished();
            return Task<Result>.FromResult(Result.Ok());
        }
    }
}
