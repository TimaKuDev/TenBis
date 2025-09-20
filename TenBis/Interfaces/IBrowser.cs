using FluentResults;

namespace TenBis.Interfaces
{
    internal interface IBrowser
    {
       Result StartTenBisWebsite();
       Result IsUserLoggedInValidation();
       Result AggregateMoneyToPoints();
       Result<string?> GetMessage();
    }
}
