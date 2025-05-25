using OpenQA.Selenium;
using Reqnroll;
using qa_dotnet_cucumber.Pages;
using System;

namespace qa_dotnet_cucumber.Steps
{
    [Binding]
    [Scope(Feature = "Login Functionality")]
    public class LoginSteps
    {
        private readonly IWebDriver _driver;
        private readonly LoginPage _loginPage;

        public LoginSteps(ScenarioContext context)
        {
            _driver = (IWebDriver)context["driver"];
            _loginPage = new LoginPage(_driver);
        }

        [Given(@"I navigate to the login page")]
        public void GivenINavigateToTheLoginPage()
        {
            _loginPage.NavigateToLoginPage();
        }

        [Given(@"I click the sign in button")]
        public void GivenIClickTheSignInButton()
        {
            _loginPage.ClickSignInButton();
        }

        [When(@"I enter valid username and password")]
        public void WhenIEnterValidUsernameAndPassword()
        {
            _loginPage.EnterCredentials("rose@gmail.com", "rose123");
        }

        [When(@"I submit the login form")]
        public void WhenISubmitTheLoginForm()
        {
            _loginPage.SubmitLogin();
        }

        [Then(@"I should be logged in and see the dashboard")]
        public void ThenIShouldBeLoggedInAndSeeTheDashboard()
        {
            if (!_loginPage.IsDashboardVisible())
            {
                throw new Exception("Dashboard is not visible after login.");
            }
        }

        // Scenario 2: Failed login with invalid email address
        [When(@"I enter invalid email and valid password")]
        public void WhenIEnterInvalidEmail()
        {
            _loginPage.EnterCredentials("invalidemail", "rose123");
        }

        [Then(@"I should see an incorrect email error message")]
        public void ThenIShouldSeeAnIncorrectEmailMessage()
        {
            if (!_loginPage.IsInvalidEmailErrorDisplayed())
            {
                throw new Exception("Invalid email error message is not displayed.");
            }
        }

        // Scenario 3: Failed login with invalid password
        [When(@"I enter valid email and invalid password")]
        public void WhenIEnterInvalidPassword()
        {
            _loginPage.EnterCredentials("rose@gmail.com", "wrongpassword");
        }

        [Then(@"I should see an incorrect password error message")]
        public void ThenIShouldSeeAnIncorrectPasswordErrorMessage()
        {
            if (!_loginPage.IsInvalidPasswordErrorDisplayed())
            {
                throw new Exception("Invalid password error message is not displayed.");
            }
        }

        // Scenario 4: Failed login with empty credentials
        [When(@"I enter empty username and password")]
        public void WhenIEnterEmptyUsernameAndPassword()
        {
            _loginPage.EnterCredentials("", "");
        }

        [Then(@"I should see a required field error message")]
        public void ThenIShouldSeeARequiredFieldErrorMessage()
        {
            if (!_loginPage.IsRequiredFieldErrorMessageDisplayed())
            {
                throw new Exception("Required field error message is not displayed.");
            }
        }
    }
}
