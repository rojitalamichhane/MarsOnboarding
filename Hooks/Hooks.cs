// // using OpenQA.Selenium;
// // using OpenQA.Selenium.Chrome;
// // using Reqnroll;
// // using Reqnroll.BoDi;
// // using WebDriverManager;
// // using WebDriverManager.DriverConfigs.Impl;
// // using AventStack.ExtentReports;
// // using AventStack.ExtentReports.Reporter;
// // using System.IO;
// // using System.Text.Json;
// // using qa_dotnet_cucumber.Config;
// // using qa_dotnet_cucumber.Pages;
// // namespace qa_dotnet_cucumber.Hooks
// // {
// //     [Binding]
// //     public class Hooks
// //     {
// //         private readonly IObjectContainer _objectContainer;
// //         private static ExtentReports? _extent;
// //         private static ExtentSparkReporter? _htmlReporter;
// //         private static TestSettings _settings = new TestSettings();
// //         private ExtentTest? _test;
// //         private static readonly object _reportLock = new object();

// //         public static TestSettings Settings => _settings;

// //         public Hooks(IObjectContainer objectContainer)
// //         {
// //             _objectContainer = objectContainer;
// //         }

// //         [BeforeTestRun]
// //         public static void BeforeTestRun()
// //         {
// //             string currentDir = Directory.GetCurrentDirectory();
// //             string settingsPath = Path.Combine(currentDir, "settings.json");
// //             string json = File.ReadAllText(settingsPath);
// //             _settings = JsonSerializer.Deserialize<TestSettings>(json) ?? new TestSettings();

// //             // Get project root by navigating up from bin/Debug/net8.0
// //             string projectRoot = Path.GetFullPath(Path.Combine(currentDir, "..", ".."));
// //             string reportFileName = _settings.Report.Path.TrimStart('/'); // e.g., "TestReport.html"
// //             string reportPath = Path.Combine(projectRoot, reportFileName);

// //             _htmlReporter = new ExtentSparkReporter(reportPath);
// //             _extent = new ExtentReports();
// //             _extent.AttachReporter(_htmlReporter);
// //             _extent.AddSystemInfo("Environment", _settings.Environment.BaseUrl);
// //             _extent.AddSystemInfo("Browser", _settings.Browser.Type);
// //             Console.WriteLine($"BeforeTestRun started at {DateTime.Now}, Report Path: {reportPath}");
// //         }

// //         [BeforeScenario]
// //         public void BeforeScenario(ScenarioContext scenarioContext)
// //         {
// //             Console.WriteLine($"Starting {scenarioContext.ScenarioInfo.Title} on Thread {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now}");
// //             new DriverManager().SetUpDriver(new ChromeConfig());
// //             var chromeOptions = new ChromeOptions();
// //             if (_settings.Browser.Headless)
// //             {
// //                 chromeOptions.AddArgument("--headless");
// //             }
// //             var driver = new ChromeDriver(chromeOptions);
// //             driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(_settings.Browser.TimeoutSeconds);
// //             driver.Manage().Window.Maximize();

// //             _objectContainer.RegisterInstanceAs<IWebDriver>(driver);
// //             _objectContainer.RegisterInstanceAs(new NavigationHelper(driver));
// //             _objectContainer.RegisterInstanceAs(new LoginPage(driver));

// //             lock (_reportLock)
// //             {
// //                 _test = _extent!.CreateTest(scenarioContext.ScenarioInfo.Title);
// //             }
// //             Console.WriteLine($"Created test: {scenarioContext.ScenarioInfo.Title} on Thread {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now}");
// //         }

// //         [AfterStep]
// //         public void AfterStep(ScenarioContext scenarioContext)
// //         {
// //             var stepType = scenarioContext.StepContext.StepInfo.StepDefinitionType.ToString();
// //             var stepText = scenarioContext.StepContext.StepInfo.Text;
// //             lock (_reportLock)
// //             {
// //                 if (scenarioContext.TestError == null)
// //                 {
// //                     _test!.Log(Status.Pass, $"{stepType} {stepText}");
// //                     Console.WriteLine($"Logged pass: {stepType} {stepText} on Thread {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now}");
// //                 }
// //                 else
// //                 {
// //                     var driver = _objectContainer.Resolve<IWebDriver>();
// //                     var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
// //                     var screenshotPath = Path.Combine(Directory.GetCurrentDirectory(), $"Screenshot_{DateTime.Now.Ticks}_{Thread.CurrentThread.ManagedThreadId}.png");
// //                     screenshot.SaveAsFile(screenshotPath);
// //                     _test!.Log(Status.Fail, $"{stepType} {stepText}", MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotPath).Build());
// //                     Console.WriteLine($"Logged fail with screenshot: {screenshotPath} on Thread {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now}");
// //                 }
// //             }
// //         }

// //         [AfterScenario]
// //         public void AfterScenario()
// //         {
// //             var driver = _objectContainer.Resolve<IWebDriver>();
// //             driver?.Quit();
// //             Console.WriteLine($"Finished scenario on Thread {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now}");
// //         }

// //         [AfterTestRun]
// //         public static void AfterTestRun()
// //         {
// //             lock (_reportLock)
// //             {
// //                 Console.WriteLine("AfterTestRun executed - Flushing report to: " + _settings.Report.Path + " at " + DateTime.Now);
// //                 _extent!.Flush();
// //             }
// //         }
// //     }
// // }

// using OpenQA.Selenium;
// using OpenQA.Selenium.Chrome;
// using Reqnroll;
// using Reqnroll.BoDi;
// using WebDriverManager;
// using WebDriverManager.DriverConfigs.Impl;
// using AventStack.ExtentReports;
// using AventStack.ExtentReports.Reporter;
// using System.IO;
// using System.Text.Json;
// using qa_dotnet_cucumber.Config;
// using qa_dotnet_cucumber.Pages;
// using System.Collections.Generic; // For List<string>
// using System.Threading;          // For Thread.Sleep

// namespace qa_dotnet_cucumber.Hooks
// {
//     [Binding]
//     public class TestHooks  // <-- Renamed class here
//     {
//         private readonly IObjectContainer _objectContainer;
//         private static ExtentReports? _extent;
//         private static ExtentSparkReporter? _htmlReporter;
//         private static TestSettings _settings = new TestSettings();
//         private ExtentTest? _test;
//         private static readonly object _reportLock = new object();

//         // List to track languages added by the current scenario
//         private readonly List<string> _addedLanguages = new();

//         public static TestSettings Settings => _settings;

//         public TestHooks(IObjectContainer objectContainer)
//         {
//             _objectContainer = objectContainer;
//         }

//         [BeforeTestRun]
//         public static void BeforeTestRun()
//         {
//             string currentDir = Directory.GetCurrentDirectory();
//             string settingsPath = Path.Combine(currentDir, "settings.json");
//             string json = File.ReadAllText(settingsPath);
//             _settings = JsonSerializer.Deserialize<TestSettings>(json) ?? new TestSettings();

//             string projectRoot = Path.GetFullPath(Path.Combine(currentDir, "..", ".."));
//             string reportFileName = _settings.Report.Path.TrimStart('/');
//             string reportPath = Path.Combine(projectRoot, reportFileName);

//             _htmlReporter = new ExtentSparkReporter(reportPath);
//             _extent = new ExtentReports();
//             _extent.AttachReporter(_htmlReporter);
//             _extent.AddSystemInfo("Environment", _settings.Environment.BaseUrl);
//             _extent.AddSystemInfo("Browser", _settings.Browser.Type);
//             Console.WriteLine($"BeforeTestRun started at {DateTime.Now}, Report Path: {reportPath}");
//         }

//         [BeforeFeature("Language")]
//         public static void BeforeLanguageFeatureCleanup()
//         {
//             Console.WriteLine("BeforeFeature: Cleaning all languages to reset state");

//             new DriverManager().SetUpDriver(new ChromeConfig());
//             var options = new ChromeOptions();
//             if (_settings.Browser.Headless)
//             {
//                 options.AddArgument("--headless");
//             }

//             using (var driver = new ChromeDriver(options))
//             {
//                 driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(_settings.Browser.TimeoutSeconds);
//                 driver.Manage().Window.Maximize();

//                 driver.Navigate().GoToUrl($"{_settings.Environment.BaseUrl}/Profile");

//                 var languagePage = new LanguagePage(driver);
//                 languagePage.DeleteAllLanguages();

//                 driver.Quit();
//             }
//         }

//         [BeforeScenario]
//         public void BeforeScenario(ScenarioContext scenarioContext)
//         {
//             Console.WriteLine($"Starting {scenarioContext.ScenarioInfo.Title} on Thread {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now}");

//             new DriverManager().SetUpDriver(new ChromeConfig());
//             var chromeOptions = new ChromeOptions();
//             if (_settings.Browser.Headless)
//             {
//                 chromeOptions.AddArgument("--headless");
//             }
//             var driver = new ChromeDriver(chromeOptions);
//             driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(_settings.Browser.TimeoutSeconds);
//             driver.Manage().Window.Maximize();

//             _objectContainer.RegisterInstanceAs<IWebDriver>(driver);
//             _objectContainer.RegisterInstanceAs(new NavigationHelper(driver));
//             _objectContainer.RegisterInstanceAs(new LoginPage(driver));
//             _objectContainer.RegisterInstanceAs(new LanguagePage(driver)); // Register LanguagePage

//             _addedLanguages.Clear();

//             lock (_reportLock)
//             {
//                 _test = _extent!.CreateTest(scenarioContext.ScenarioInfo.Title);
//             }

//             Console.WriteLine($"Created test: {scenarioContext.ScenarioInfo.Title} on Thread {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now}");
//         }

//         [AfterStep]
//         public void AfterStep(ScenarioContext scenarioContext)
//         {
//             var stepType = scenarioContext.StepContext.StepInfo.StepDefinitionType.ToString();
//             var stepText = scenarioContext.StepContext.StepInfo.Text;
//             lock (_reportLock)
//             {
//                 if (scenarioContext.TestError == null)
//                 {
//                     _test!.Log(Status.Pass, $"{stepType} {stepText}");
//                     Console.WriteLine($"Logged pass: {stepType} {stepText} on Thread {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now}");
//                 }
//                 else
//                 {
//                     var driver = _objectContainer.Resolve<IWebDriver>();
//                     var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
//                     var screenshotPath = Path.Combine(Directory.GetCurrentDirectory(), $"Screenshot_{DateTime.Now.Ticks}_{Thread.CurrentThread.ManagedThreadId}.png");
//                     screenshot.SaveAsFile(screenshotPath);
//                     _test!.Log(Status.Fail, $"{stepType} {stepText}", MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotPath).Build());
//                     Console.WriteLine($"Logged fail with screenshot: {screenshotPath} on Thread {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now}");
//                 }
//             }
//         }

//         [AfterScenario]
//         public void AfterScenario()
//         {
//             var driver = _objectContainer.Resolve<IWebDriver>();
//             var languagePage = _objectContainer.Resolve<LanguagePage>();

//             // Delete only languages added during this scenario
//             foreach (var language in _addedLanguages)
//             {
//                 Console.WriteLine($"Cleaning up language added by test: {language}");
//                 languagePage.DeleteLanguage(language);
//                 Thread.Sleep(500);
//             }

//             driver?.Quit();
//             Console.WriteLine($"Finished scenario on Thread {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now}");
//         }

//         [AfterTestRun]
//         public static void AfterTestRun()
//         {
//             lock (_reportLock)
//             {
//                 Console.WriteLine("AfterTestRun executed - Flushing report to: " + _settings.Report.Path + " at " + DateTime.Now);
//                 _extent!.Flush();
//             }
//         }

//         // Register added language for cleanup after scenario
//         public void RegisterAddedLanguage(string language)
//         {
//             if (!_addedLanguages.Contains(language))
//             {
//                 _addedLanguages.Add(language);
//             }
//         }
//     }
// }

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using Reqnroll;
using Reqnroll.BoDi;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using System.Text.Json;
using qa_dotnet_cucumber.Config;
using qa_dotnet_cucumber.Pages;

namespace qa_dotnet_cucumber.Hooks
{
    [Binding]
    public class Hooks
    {
        private readonly IObjectContainer _objectContainer;
        private static ExtentReports? _extent;
        private static ExtentSparkReporter? _htmlReporter;
        private static TestSettings _settings = new TestSettings();
        private ExtentTest? _test;
        private static readonly object _reportLock = new object();
        private IWebDriver? _driver;
        public static TestSettings Settings => _settings;
        public Hooks(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            string currentDir = Directory.GetCurrentDirectory();
            string settingsPath = Path.Combine(currentDir, "settings.json");
            string json = File.ReadAllText(settingsPath);
            _settings = JsonSerializer.Deserialize<TestSettings>(json) ?? new TestSettings();

            string projectRoot = Path.GetFullPath(Path.Combine(currentDir, "..", ".."));
            string reportFileName = _settings.Report.Path.TrimStart('/');
            string reportPath = Path.Combine(projectRoot, reportFileName);

            _htmlReporter = new ExtentSparkReporter(reportPath);
            _extent = new ExtentReports();
            _extent.AttachReporter(_htmlReporter);
            _extent.AddSystemInfo("Environment", _settings.Environment.BaseUrl);
            _extent.AddSystemInfo("Browser", _settings.Browser.Type);
            Console.WriteLine($"BeforeTestRun started at {DateTime.Now}, Report Path: {reportPath}");
        }

        [BeforeScenario]
        public void BeforeScenario(ScenarioContext scenarioContext)
        {
            Console.WriteLine($"Starting {scenarioContext.ScenarioInfo.Title} on Thread {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now}");

            switch (_settings.Browser.Type?.ToLower() ?? throw new InvalidOperationException("Browser type is not configured."))
            {
                case "chrome":
                    new DriverManager().SetUpDriver(new ChromeConfig());
                    var chromeOptions = new ChromeOptions();
                    if (_settings.Browser.Headless)
                        chromeOptions.AddArgument("--headless");
                    _driver = new ChromeDriver(chromeOptions);
                    break;

                case "edge":
                    new DriverManager().SetUpDriver(new EdgeConfig());
                    var edgeOptions = new EdgeOptions();
                    if (_settings.Browser.Headless)
                        edgeOptions.AddArgument("--headless");
                    _driver = new EdgeDriver(edgeOptions);
                    break;

                default:
                    throw new ArgumentException($"Unsupported Browser: {_settings.Browser.Type}");
            }

            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(_settings.Browser.TimeoutSeconds);
            _driver.Manage().Window.Maximize();

            _objectContainer.RegisterInstanceAs<IWebDriver>(_driver);
            _objectContainer.RegisterInstanceAs(new NavigationHelper(_driver));
            _objectContainer.RegisterInstanceAs(new LoginPage(_driver));
            _objectContainer.RegisterInstanceAs(new LanguagePage(_driver));
            _objectContainer.RegisterInstanceAs(new SkillPage(_driver));

            lock (_reportLock)
            {
                _test = _extent!.CreateTest(scenarioContext.ScenarioInfo.Title);
            }

            Console.WriteLine($"Created test: {scenarioContext.ScenarioInfo.Title} on Thread {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now}");
        }

        [AfterStep]
        public void AfterStep(ScenarioContext scenarioContext)
        {
            var stepType = scenarioContext.StepContext.StepInfo.StepDefinitionType.ToString();
            var stepText = scenarioContext.StepContext.StepInfo.Text;
            lock (_reportLock)
            {
                if (scenarioContext.TestError == null)
                {
                    _test!.Log(Status.Pass, $"{stepType} {stepText}");
                }
                else
                {
                    var driver = _objectContainer.Resolve<IWebDriver>();
                    var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                    var screenshotPath = Path.Combine(Directory.GetCurrentDirectory(), $"Screenshot_{DateTime.Now.Ticks}.png");
                    screenshot.SaveAsFile(screenshotPath);
                    _test!.Log(Status.Fail, $"{stepType} {stepText}", MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotPath).Build());
                }
            }
        }

        [AfterScenario]
        public void CleanUpAfterScenario(ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            try
            {
                // LANGUAGE CLEANUP
                if (scenarioContext.TryGetValue("LanguagesToCleanup", out List<string>? languages) &&
                    languages != null && languages.Any())
                {
                    var languagePage = _objectContainer.Resolve<LanguagePage>();
                    foreach (var language in languages)
                        languagePage.DeleteLanguage(language);
                }


                // SKILL CLEANUP
               
            if (scenarioContext.TryGetValue("SkillsToCleanup", out List<string>? skills) &&
                skills != null && skills.Any())
            {
                Console.WriteLine("Starting Skill cleanup...");

                var skillPage = _objectContainer.Resolve<SkillPage>();
                // skillPage.NavigateToSkillTab();

                foreach (var skill in skills)
                {
                    Console.WriteLine($"🧹 Trying to delete skill: {skill}");
                    skillPage.DeleteSpecificSkill(skill);
                }

                Console.WriteLine("Skill cleanup completed.");
            }
                _driver?.Quit();
                _driver?.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cleanup failed: {ex.Message}");
            }
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            lock (_reportLock)
            {
                Console.WriteLine("AfterTestRun executed - Flushing report");
                _extent!.Flush();
            }
        }
    }
}
