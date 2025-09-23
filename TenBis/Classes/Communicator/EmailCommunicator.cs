using FluentResults;
using GeneralUtils;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using TenBis.Interfaces;
using TenBis.Logging;
using TenBis.SettingsFolder.Models;

namespace TenBis.Classes.Communicator
{
    internal class EmailCommunicator : ICommunicator
    {
        private readonly EmailSettings m_EmailSettings;

        public EmailCommunicator(EmailSettings? emailSettings, ValidationMessageConfig? validationMessageConfig)
        {
            Logger.FunctionStarted();

            if (emailSettings is null)
            {
                throw new ArgumentNullException(nameof(emailSettings));
            }

            if (string.IsNullOrWhiteSpace(emailSettings.SMTPServer))
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

            if (validationMessageConfig is null)
            {
                throw new ArgumentNullException(nameof(validationMessageConfig));
            }

            if (!validationMessageConfig.ResendIntervalMinutes.HasValue)
            {
                throw new ArgumentNullException(nameof(validationMessageConfig));
            }

            if (!validationMessageConfig.ResponseTimeoutMinutes.HasValue)
            {
                throw new ArgumentNullException(nameof(validationMessageConfig));
            }

            m_EmailSettings = emailSettings;

            Logger.FunctionFinished();
        }

        Task<Result<bool>> ICommunicator.SendValidationMessage()
        {
            Logger.FunctionStarted();
            Logger.FunctionFinished();
            return Task.FromResult(Result.Ok(true));
        }


        Task<Result> ICommunicator.SendMessage(string message)
        {
            Logger.FunctionStarted();

            TextPart textPart = new(TextFormat.Text) { Text = message };
            MimeMessage mimeMessage = new()
            {
                Subject = "Ten Bis",
                Body = textPart
            };
            mimeMessage.From.Add(new MailboxAddress("Ten Bis Notifier", EmailInformation.Address));
            mimeMessage.To.Add(MailboxAddress.Parse(m_EmailSettings.RecipientEmail));

            try
            {
                using SmtpClient client = new();
                client.Connect(m_EmailSettings.SMTPServer, m_EmailSettings.Port, useSsl: true);
                client.Authenticate(m_EmailSettings.Username, m_EmailSettings.Password);
                client.Send(mimeMessage);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message);
                return Task.FromResult(Result.Fail("Failed to send email").WithError(exception.Message));
            }

            Logger.FunctionFinished();
            return Task.FromResult(Result.Ok());
        }
    }
}
