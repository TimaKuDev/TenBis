using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using TenBis.Interfaces;

namespace TenBis.Classes.Notifiers
{
    internal class EmailNotifier : INotifier
    {
        private readonly string _notifyTo;
        private readonly string _currentBalanceAmount;
        private readonly bool? _isSuccessfullyAggregation;

        public EmailNotifier(string notifyTo, string currentBalanceAmount, bool? isSuccessfullyAggregation)
        {
            _notifyTo = notifyTo;
            _currentBalanceAmount = currentBalanceAmount;
            _isSuccessfullyAggregation = isSuccessfullyAggregation;
        }

        public void Notify()
        {
            if (string.IsNullOrEmpty(_notifyTo))
            {
                return;
            }

            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("Ten Bis Notifier", "TenBisNotifier@gmail.com"));
            message.To.Add(MailboxAddress.Parse(_notifyTo));
            message.Subject = "Ten Bis";
            string currentBalance = string.IsNullOrEmpty(_currentBalanceAmount) ? null : $"Your current balance is: {_currentBalanceAmount}";
            string updateStatus = _isSuccessfullyAggregation.HasValue && _isSuccessfullyAggregation.Value ? "successfully updated" : "failed to update";
            message.Body = new TextPart(TextFormat.Text)
            {
                Text = @$"10 Bis {updateStatus}
{currentBalance}"
            };

            SmtpClient client = new SmtpClient();
            try
            {
                client.Connect("smtp.gmail.com", 465, true);
                client.Authenticate("TenBisNotifier@gmail.com", "Need to input password here");
                client.Send(message);
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
    }
}
