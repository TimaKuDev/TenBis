using System.Runtime.CompilerServices;

namespace TenBis.Classes
{
    internal static class Helper
    {
        public static string GetCurrentMethod([CallerMemberName] string method = "")
        {
            return method;
        }
    }
}
