using TenBis.Classes.Browser;
using TenBis.Enums;
using TenBis.Interfaces;

namespace TenBis.Factories
{
    internal static class BrowserFactory
    {
        public static IBrowser CreateBrowser(BrowserType browserTypeEnum, string? userProfilePath)
        {
            if (string.IsNullOrEmpty(userProfilePath)) 
            {
                throw new ArgumentNullException();
            }

            switch (browserTypeEnum)
            {
                case BrowserType.Chrome:
                    return new ChromeTenBisBrowser(userProfilePath);
                case BrowserType.Edge:
                    return new EdgeTenBisBrowser(userProfilePath);
                case BrowserType.FireFox:
                    return new FireFoxTenBisBrowser(userProfilePath);
                default:
                    throw new ArgumentException($"Invalid  browserType: {browserTypeEnum}");
            }
        }
    }
}
