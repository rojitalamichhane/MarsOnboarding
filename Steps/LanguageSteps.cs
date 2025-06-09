using OpenQA.Selenium;
using qa_dotnet_cucumber.Pages;
using Reqnroll;
using NUnit.Framework;
using System.Text;

[Binding]
[Scope(Feature = "Language Functionality")]
public class LanguageFunctionalityStepDefinitions
{

    private readonly LoginPage _loginPage;
    private readonly NavigationHelper _navigationHelper;
    private readonly LanguagePage _languagePage;
    private readonly ScenarioContext _scenarioContext;
    public LanguageFunctionalityStepDefinitions(LoginPage loginPage, NavigationHelper navigationHelper, LanguagePage languagePage, ScenarioContext scenarioContext)
    {
        _loginPage = loginPage;
        _navigationHelper = navigationHelper;
        _languagePage = languagePage;
        _scenarioContext = scenarioContext;
    }

    [Given(@"I sign in to the profile page with valid email address and password")]
    public void GivenISignInToTheProfilePageWithValidEmailAddressAndPassword()
    {
        _navigationHelper.NavigateTo("http://localhost:5003/");
        _loginPage.Login("rose@gmail.com", "rose123");
    }

    // ------------------ Add Language Steps ------------------
    [When(@"I create a new '([^']*)' and '([^']*)' in my profile")]
    public void WhenICreateANewLanguageAndLevelInMyProfile(string language, string level)
    {
        _languagePage.AddLanguage(language, level);
    }

    [Then(@"The '([^']*)' and '([^']*)' should be created and listed successfully")]
    public void ThenTheLanguageAndLevelShouldBeCreatedAndListedSuccessfully(string language, string level)
    {
        string LanguageAddedMsg = _languagePage.LangAddedSuccessMsg();
        string SavedLanguage = _languagePage.LanguageListing();
        string SavedLevel = _languagePage.LevelListing();

        Assert.That(LanguageAddedMsg == language + " has been added to your languages", "Language Added Message is not displayed successfully");
        Assert.That(SavedLanguage == language, "Language has not been added successfully");
        Assert.That(SavedLevel == level, "Level has not been added successfully");
    }

    // ------------------ Update Language Steps ------------------
    [When("I update an Existing Language and Existing Level in my profile")]
    public void WhenIUpdateAnExistingLanguageAndExistingLevelInMyProfile(Table updateLangtable)
    {
        _languagePage.DeleteAllLanguages();
        _languagePage.WaitForAddNewButton();
        Thread.Sleep(1000); // Wait for deletion to complete
        _languagePage.AddLanguage("English", "Basic");
        _languagePage.AddLanguage("Spanish", "Fluent");

        var updatedLanguages = new List<(string newLang, string newLevel, string successMsg)>();

        foreach (var row in updateLangtable.Rows)
        {
            var language = row["Language"];
            var newLanguage = row["New Language"];
            var newLevel = row["New Level"];

            _languagePage.UpdateLanguage(language, newLanguage, newLevel);
            string successMsg = _languagePage.LangUpdatedSuccessMsg();
            updatedLanguages.Add((newLanguage, newLevel, successMsg));


        }
        _scenarioContext["updatedLanguages"] = updatedLanguages;
    }
    [Then("The New Language and New Level should be updated and listed successfully")]
    public void ThenTheNewLanguageAndNewLevelShouldBeUpdatedAndListedSuccessfully()
    {
        var updatedLanguages = (List<(string newLang, string newLevel, string successMsg)>)_scenarioContext["updatedLanguages"];

        foreach (var (newLang, newLevel, successMsg) in updatedLanguages)
        {
            var isLanguagePresent = _languagePage.IsLanguageAndLevelPresent(newLang, newLevel);
            Assert.That(isLanguagePresent,
                Is.True,
                $"Expected to find '{newLang}' with level '{newLevel}' in the language list, but it was not found.");
            Assert.That(successMsg.Contains(newLang),
                $"Expected success message to contain '{newLang}', but got: '{successMsg}'");
        }

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
    [When("I try to add the following language entries:")]
    public void WhenITryToAddTheFollowingLanguageEntries(Table dupLangCheckTable)
    {

        foreach (var row in dupLangCheckTable.Rows)
        {
            string sameLang = row["DupLanguage"];
            string firstLangLevel = row["FirstLevel"];
            string secondLangLevel = row["SecondLevel"];
            string expectedMessage = row["ExpectedMessage"];

            _languagePage.DeleteAllLanguages();

            _languagePage.AddLanguage(sameLang, firstLangLevel);
            _languagePage.AddLanguage(sameLang, secondLangLevel);


            string actualMessage = _languagePage.GetDuplicateLanguageMessage();
            _languagePage.clickCancelButton();
            Console.WriteLine("Message displayed: " + actualMessage);

            _scenarioContext["ActualMessage"] = actualMessage;
            _scenarioContext["ExpectedMessage"] = expectedMessage;

        }
    }

    [Then("ExpectedMessage should be displayed")]
    public void ThenExpectedMessageShouldBeDisplayed()
    {
        string actualMessage = _scenarioContext["ActualMessage"] as string ?? string.Empty;
        string expectedMessage = _scenarioContext["ExpectedMessage"] as string ?? string.Empty;

        Assert.That(actualMessage, Is.EqualTo(expectedMessage),
            $"Expected message '{expectedMessage}', but got '{actualMessage}'.");

    }


    //Language Empty check 
    [When("I try to add a language without language or level")]
    public void WhenITryToAddALanguageWithoutLanguageOrLevel(Table table)
    {
        _languagePage.DeleteAllLanguages(); // Clean up
        _languagePage.WaitForAddNewButton();
        Thread.Sleep(1000); // Optional short pause

        var validationMessages = new List<string>();

        foreach (var row in table.Rows)
        {
            string language = row["Language"];
            string level = row["Level"];

            _languagePage.AddLanguage(language, level); // This triggers the validation
            string message = _languagePage.GetValidationErrorMessage();
            validationMessages.Add(message);
        }

        _scenarioContext["validationMessages"] = validationMessages;
    }

    [Then("Please enter language and level should be displayed")]
    public void ThenPleaseEnterLanguageAndLevelShouldBeDisplayed()
    {
        var messages = (List<string>)_scenarioContext["validationMessages"];

        foreach (var msg in messages)
        {
            Assert.That(msg.ToLower(), Does.Contain("please"), $"Expected a validation message but got: {msg}");
        }
    }



    [Then(@"I should see the error message '(.*)'")]
    public void ThenIShouldSeeTheErrorMessage(string expectedMessage)
    {
        string actualMessage = _languagePage.GetValidationErrorMessage();
        Assert.That(actualMessage, Is.EqualTo(expectedMessage), "Validation error message did not match!");
    }

    //Language validation for alphanumeric and special characters
    [When("I try to enter non-alphabet characters in language field")]
        public void WhenITryToEnterNon_AlphabetCharactersInLanguageField(Table invalidLangDataSet)
        {
            var invalidLanguages = new List<(string invalidLanguage, string level, string expectedMessage, string actualMessage, bool isPresentInList)>();

            foreach (var row in invalidLangDataSet.Rows)
            {
                string invalidLanguage = row["InvalidLanguage"];
                string level = row["Level"];
                string expectedMessage = row["ExpectedMessage"];

                _languagePage.DeleteAllLanguages();  // Clear any existing data
                _languagePage.AddLanguage(invalidLanguage, level);  // Attempt to create a new language

                // Get the actual error message displayed for the invalid input
                string actualMessage = _languagePage.LangLevelFieldValidationErrMsg();

                // Check if the invalid language and level were added to the table (which should NOT happen)
                bool isPresent = _languagePage.IsLanguageAndLevelPresent(invalidLanguage, level);

                // Add the test data and results to the list
                invalidLanguages.Add((invalidLanguage, level, expectedMessage, actualMessage, isPresent));

            }
            _scenarioContext["InvalidLanguages"] = invalidLanguages;
        }

            [Then("Error message should be displayed")]
        public void ThenErrorMessageShouldBeDisplayed()
        {
            var invalidLanguages = (List<(string invalidLanguage, string level, string expectedMessage, string actualMessage, bool isPresentInList)>)_scenarioContext["InvalidLanguages"];

            // Use StringBuilder for error messages to improve efficiency
            var errorBuilder = new StringBuilder();
            bool hasError = false;  // Flag to check if any error occurred

            // Loop through each invalid language entry
            foreach (var result in invalidLanguages)
            {
                // Check if the actual message matches the expected one
                if (result.actualMessage != result.expectedMessage)
                {
                    hasError = true;
                    errorBuilder.AppendLine(
                        $"Mismatch in error message for language '{result.invalidLanguage}' and level '{result.level}'. " +
                        $"Expected: '{result.expectedMessage}', but got: '{result.actualMessage}'");
                }

                // Check if the invalid language was incorrectly added to the list
                if (result.isPresentInList)
                {
                    hasError = true;
                    errorBuilder.AppendLine(
                        $"Invalid language '{result.invalidLanguage}' with level '{result.level}' was incorrectly added to the language list.");
                }
            }

            // If errors exist, fail the test and display them
            Assert.That(hasError, Is.False, errorBuilder.ToString());
        }
}