using OpenQA.Selenium;
using qa_dotnet_cucumber.Pages;
using Reqnroll;
using NUnit.Framework;

namespace qa_dotnet_cucumber.Steps
{
    [Binding]
    public class LanguageSteps
    {
        private readonly IWebDriver _driver;
        private readonly LanguagePage _languagePage;
        private readonly LoginPage _loginPage;

        public LanguageSteps(ScenarioContext scenarioContext)
        {
            _driver = scenarioContext["driver"] as IWebDriver
                      ?? throw new ArgumentNullException("WebDriver is not initialized in ScenarioContext.");
            _languagePage = new LanguagePage(_driver);
            _loginPage = new LoginPage(_driver);
        }

        [Given(@"I sign in to the profile page with valid email address and password")]
        public void GivenISignInToTheProfilePageWithValidEmailAddressAndPassword()
        {
            _driver.Navigate().GoToUrl("http://localhost:5003/");
            _loginPage.Login("rose@gmail.com", "rose123");
        }

        // ------------------ Add Language Steps ------------------
        [When(@"I create a new '([^']*)' and '([^']*)' in my profile")]
        public void WhenICreateANewLanguageAndLevelInMyProfile(string language, string level)
        {
            _languagePage.NavigateToLanguageTab();
            _languagePage.AddLanguage(language, level);
        }

        [Then(@"The '([^']*)' and '([^']*)' should be created and listed successfully")]
        public void ThenTheLanguageAndLevelShouldBeCreatedAndListedSuccessfully(string language, string level)
        {
            Assert.IsTrue(_languagePage.IsLanguageDisplayed(language, level), $"Language '{language}' with level '{level}' was not found in the list.");
        }

        // ------------------ Update Language Steps ------------------
        [When(@"I update existing language '([^']*)' to new language '([^']*)' and level '([^']*)'")]
        public void WhenIUpdateExistingLanguageToNewLanguageAndLevel(string currentLanguage, string newLanguage, string newLevel)
        {
            _languagePage.NavigateToLanguageTab();
            _languagePage.UpdateLanguage(currentLanguage, newLanguage, newLevel);
        }

        [Then(@"The updated language '([^']*)' and '([^']*)' should be listed successfully")]
        public void ThenTheUpdatedLanguageAndLevelShouldBeListedSuccessfully(string language, string level)
        {
            Assert.IsTrue(_languagePage.IsLanguageDisplayed(language, level), $"Updated language '{language}' with level '{level}' was not found.");
        }

        // ------------------ Delete Language Steps ------------------

        [When("I delete all languages in my profile and successful message should appear")]
        public void WhenIDeleteAllLanguagesInMyProfileAndSuccessfulMessageShouldAppear()
        {
            _languagePage.DeleteAllLanguages();
        }
        [Then("The deleted language should not appear in the list")]
        public void ThenTheDeletedLanguageShouldNotAppearInTheList()
        {
            bool languagesExist = _languagePage.AreLanguagesPresent();
            Assert.That(languagesExist, Is.False, "Languages are still present in the profile list. Expected all to be deleted.");
        }


        // ------------------ Duplicate Language Steps ------------------
        [When(@"I try to add a duplicate language '(.*)' with level '(.*)' in my profile")]
        public void WhenITryToAddDuplicateLanguageWithLevelInMyProfile(string language, string level)
        {
            _languagePage.AddLanguage(language, level);
        }

        [Then(@"I should see the duplicate error message '(.*)'")]
        public void ThenIShouldSeeTheDuplicateErrorMessage(string expectedMessage)
        {
            string actualMessage = _languagePage.GetDuplicateLanguageMessage();

            Assert.That(actualMessage, Is.EqualTo(expectedMessage), "Duplicate language error message did not match!");
        }

        //Language Empty check 
        [When(@"I try to add language '([^']*)' with level '([^']*)' in my profile")]
        public void WhenITryToAddLanguageWithLevelInMyProfile(string language, string level)
        {
            _languagePage.NavigateToLanguageTab(); // if needed
            _languagePage.AddLanguage(language, level);
        }


        [Then(@"I should see the error message '(.*)'")]
        public void ThenIShouldSeeTheErrorMessage(string expectedMessage)
        {
            string actualMessage = _languagePage.GetValidationErrorMessage();
            Assert.That(actualMessage, Is.EqualTo(expectedMessage), "Validation error message did not match!");
        }

        //Language validation for alphanumeric and special characters
        [When(@"I add the following languages and select level:")]
        public void WhenIAddTheFollowingLanguagesAndSelectLevel(Table table)
        {
            foreach (var row in table.Rows)
            {
                string language = row["Language"];
                string level = row["Level"];
                _languagePage.AddLanguage(language, level);
            }
        }


        [Then(@"Error message prompt should be displayed")]
        public void ThenErrorMessageShouldBeDisplayed()
        {
            string message = _languagePage.GetLanguageErrorMessage();

            if (message.Contains("has been added to your languages"))
            {
                Assert.Fail("Test Failed: Invalid language was accepted with success message.");
            }
            else if (message.ToLower().Contains("invalid") || message.ToLower().Contains("error"))
            {
                Assert.Pass("Correct error message displayed.");
            }
            else
            {
                Assert.Fail($"Unexpected message: {message}");
            }
        }

        // Cancel adding language

        [When(@"I start adding the language ""([^""]*)"" with level ""([^""]*)"" and cancel the operation")]
        public void WhenIStartAddingLanguageWithLevelAndCancel(string language, string level)
        {
            _languagePage.NavigateToLanguageTab();
            _languagePage.AddLanguageAndCancel(language, level);
        }

        [Then(@"the language ""([^""]*)"" should not be added to the list")]
        public void ThenLanguageShouldNotBeAdded(string language)
        {
            bool isPresent = _languagePage.IsLanguageDisplayed(language, "Basic");
            Assert.IsFalse(isPresent, $"Language '{language}' was incorrectly added to the list.");
        }
    }
}
