using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace qa_dotnet_cucumber.Pages
{
    public class LoginPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        // Locators
        private const string SignInButton = "//a[@class='item' and text()='Sign In']";
        private const string EmailField = "//input[@name='email']";
        private const string PasswordField = "//input[@name='password']";
        private const string LoginButton = "//button[text()='Login']";
        private const string Dashboard = "//h3[@class='ui dividing header' and text()='Description']";

        // Error locators
        private const string InvalidEmailError = "//div[contains(@class,'prompt') and text()='Please enter a valid email address']";
        private const string InvalidPasswordError = "//button[@id='submit-btn' and text()='Send Verification Email']";
        private const string RequiredFieldError = "//div[contains(@class,'prompt') and text()='Please enter a valid email address']";

        // Constructor
        public LoginPage(IWebDriver driver, int timeoutInSeconds = 15)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        // Navigate to login page
        public void NavigateToLoginPage(string url = "http://localhost:5003")
        {
            Console.WriteLine($"Navigating to the login page: {url}");
            _driver.Navigate().GoToUrl(url);
        }

        // Click Sign In button
        public void ClickSignInButton()
        {
            try
            {
                Console.WriteLine("Waiting for the Sign In button to be clickable...");
                var signInButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(SignInButton)));
                signInButton.Click();
                Console.WriteLine("Sign In button clicked.");
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("Sign In button was not clickable within the specified time.");
            }
        }

        // Enter credentials
        public void EnterCredentials(string email, string password)
        {
            try
            {
                Console.WriteLine($"Entering credentials: Email = {email}, Password = [REDACTED]");
                var emailField = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(EmailField)));
                emailField.Clear();
                emailField.SendKeys(email);

                var passwordField = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(PasswordField)));
                passwordField.Clear();
                passwordField.SendKeys(password);
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("Failed to locate email or password field within the timeout.");
            }
        }

        // Submit login form
        public void SubmitLogin()
        {
            try
            {
                Console.WriteLine("Submitting login form...");
                var loginButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(LoginButton)));
                loginButton.Click();
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("Login button was not clickable within the timeout.");
            }
        }

        // Check if dashboard is visible after successful login
        public bool IsDashboardVisible()
        {
            Console.WriteLine("Checking if the dashboard is visible...");
            return IsElementVisible(Dashboard);
        }

        // Check if invalid email error is displayed
        public bool IsInvalidEmailErrorDisplayed()
        {
            Console.WriteLine("Checking if invalid email error message is visible...");
            return IsElementVisible(InvalidEmailError);
        }

        // Check if invalid password error (verification email button) is displayed
        public bool IsInvalidPasswordErrorDisplayed()
        {
            Console.WriteLine("Checking if invalid password error (Send Verification Email button) is visible...");
            return IsElementVisible(InvalidPasswordError);
        }

        // Check if required field error message is displayed (assuming it's the same as InvalidEmailError)
        public bool IsRequiredFieldErrorMessageDisplayed()
        {
            Console.WriteLine("Checking if required field error message is visible...");
            return IsElementVisible(RequiredFieldError);
        }

        // Helper method to check if an element is visible
        private bool IsElementVisible(string xpath)
        {
            try
            {
                Console.WriteLine($"Checking visibility for element with XPath: {xpath}");
                _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xpath)));
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine($"Element with XPath: {xpath} is not visible within the timeout.");
                return false;
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine($"Element with XPath: {xpath} not found.");
                return false;
            }
        }

        public void Login(string email, string password)
        {
            NavigateToLoginPage();
            ClickSignInButton();
            EnterCredentials(email, password);
            SubmitLogin();
        }

        public bool IsAtLoginPage()
        {
            return _driver.Title.Contains("Home");
        }
    }
}
