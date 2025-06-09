using OpenQA.Selenium;

namespace qa_dotnet_cucumber.Pages
{
    public class NavigationHelper
    {
        private readonly IWebDriver _driver;

        public NavigationHelper(IWebDriver driver)
        {
            _driver = driver;
        }

        public void NavigateTo(string urlPath)
        {
            if (Hooks.Hooks.Settings?.Environment?.BaseUrl != null)
            {
                _driver.Navigate().GoToUrl(Hooks.Hooks.Settings.Environment.BaseUrl + urlPath);
            }
            else
            {
                throw new InvalidOperationException("BaseUrl is null. Cannot navigate to the specified URL.");
            }
        }
    }
}