using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Reqnroll;

namespace qa_dotnet_cucumber.Hooks;

[Binding]
public class Hook
{
    private readonly ScenarioContext _scenarioContext;
    private IWebDriver? _driver;  // ✅ nullable to suppress warning

    public Hook(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeScenario]
    public void BeforeScenario()
    {
        _driver = new ChromeDriver();
        _scenarioContext["driver"] = _driver;
    }

    [AfterScenario]
    public void AfterScenario()
    {
        _driver?.Quit();
    }
}
//  | !@#$%^&*(){}!@#$%^&*(){}!@#$%^   | Intermediate  |
//    | %%%%%%%%%%%%$$$$$$$$$$$$$$$$$$   | Expert        |
