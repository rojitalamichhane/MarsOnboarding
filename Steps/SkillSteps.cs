using OpenQA.Selenium;
using qa_dotnet_cucumber.Pages;
using Reqnroll;
using NUnit.Framework;

namespace qa_dotnet_cucumber.Steps
{
    [Binding]
    public class SkillSteps
    {
        private readonly IWebDriver _driver;
        private readonly SkillPage _skillPage;
        private readonly LoginPage _loginPage;

        public SkillSteps(ScenarioContext scenarioContext)
        {
            _driver = scenarioContext["driver"] as IWebDriver
                      ?? throw new ArgumentNullException("WebDriver is not initialized in ScenarioContext.");
            _skillPage = new SkillPage(_driver);
            _loginPage = new LoginPage(_driver);
        }

        [Given(@"I sign in to the profile page as a valid user")]
        public void GivenISignInToTheProfilePageAsAValidUser()
        {
            _driver.Navigate().GoToUrl("http://localhost:5003/");
            _loginPage.Login("rose@gmail.com", "rose123");
            _skillPage.NavigateToSkillTab();
        }

        // ------------------ Add skill Steps ------------------
        [When("I add a new {string} and {string} in my profile")]
        public void WhenIAddANewAndInMyProfile(string skill, string level)
        {
            _skillPage.AddSkill(skill, level);
        }

        [Then("The {string} and {string} should be added and listed successfully")]
        public void ThenTheAndShouldBeAddedAndListedSuccessfully(string skill, string level)
        {
            Assert.IsTrue(_skillPage.IsSkillDisplayed(skill, level), $"Skill '{skill}' with level '{level}' was not found.");
        }

        // ------------------ Update skill Steps ------------------
        [When(@"I update existing skill '([^']*)' to new skill '([^']*)' and level '([^']*)'")]
        public void WhenIUpdateExistingSkillToNewSkillAndLevel(string currentSkill, string newSkill, string newLevel)
        {

            _skillPage.UpdateSkill(currentSkill, newSkill, newLevel);
        }

        [Then(@"The updated skill '([^']*)' and '([^']*)' should be listed successfully")]
        public void ThenTheUpdatedSkillAndLevelShouldBeListedSuccessfully(string skill, string level)
        {
            Assert.IsTrue(_skillPage.IsSkillDisplayed(skill, level), $"Updated skill '{skill}' with level '{level}' was not found.");
        }

        //------------------- Delete skill Steps ------------------

        [When("I delete all skills in my profile and successful message should appear")]
        public void WhenIDeleteAllSkillsInMyProfileAndSuccessfulMessageShouldAppear()
        {
            _skillPage.DeleteAllSkills();
        }
        [Then("The deleted skill should not appear in the list")]
        public void ThenTheDeletedSkillShouldNotAppearInTheList()
        {
            bool skillsExist = _skillPage.AreSkillsPresent();
            Assert.That(skillsExist, Is.False, "Skills are still present in the profile list. Expected all to be deleted.");
        }

        // ------------------ Duplicate Skill Steps ------------------
        [When(@"I try to add a duplicate skill '(.*)' with level '(.*)' in my profile")]
        public void WhenITryToAddDuplicateSkillWithLevelInMyProfile(string skill, string level)
        {
            _skillPage.AddSkill(skill, level);
        }

        [Then(@"I should see the duplicate skill error message '(.*)'")]
        public void ThenIShouldSeeTheDuplicateSkillErrorMessage(string expectedMessage)
        {
            string actualMessage = _skillPage.GetDuplicateSkillMessage();

            Assert.That(actualMessage, Is.EqualTo(expectedMessage), "Duplicate skill error message did not match!");
        }

        //Skill Empty check 
        [When(@"I try to add skill '([^']*)' with level '([^']*)' in my profile")]
        public void WhenITryToAddSkillWithLevelInMyProfile(string skill, string level)
        {
            _skillPage.AddSkill(skill, level);
        }


        [Then(@"I should see the empty skill error message '(.*)'")]
        public void ThenIShouldSeeTheEmptySkillErrorMessage(string expectedMessage)
        {
            string actualMessage = _skillPage.GetValidationErrorMessage();
            Assert.That(actualMessage, Is.EqualTo(expectedMessage), "Validation error message did not match!");
        }

        // Validation check for skill fileds
        [When(@"I add the following skills and select skill level:")]
        public void WhenIAddTheFollowingSkillsAndSelectSkillLevel(Table table)
        {
            foreach (var row in table.Rows)
            {
                string skill = row["Skill"];
                string skillLevel = row["SkillLevel"];
                _skillPage.AddSkill(skill, skillLevel);
            }
        }

        [Then(@"Error message should be displayed")]
        public void ThenErrorMessageShouldBeDisplayed()
        {
            string message = _skillPage.GetSkillErrorMessage();

            if (message.Contains("has been added to your skills"))
            {
                Assert.Fail("Test Failed: Invalid skill was accepted with success message.");
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

    }
}
