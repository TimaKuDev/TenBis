using FluentResults;

namespace TenBis.Interfaces
{
    internal interface ICommunicator
    {
        internal Task<Result> SendValidationMessage();

        internal Task<Result> SendMessage(string message);
    }
}
