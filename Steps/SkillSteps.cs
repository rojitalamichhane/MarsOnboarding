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
        private readonly NavigationHelper _navigationHelper;
        private readonly ScenarioContext _scenarioContext;

        public SkillSteps(IWebDriver driver, SkillPage skillPage, LoginPage loginPage, NavigationHelper navigationHelper,
            ScenarioContext scenarioContext)  // <-- add this parameter
        {
            _driver = driver;
            _skillPage = skillPage;
            _loginPage = loginPage;
            _navigationHelper = navigationHelper;
            _scenarioContext = scenarioContext;    // <-- assign it here!
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

            if (!_scenarioContext.TryGetValue("SkillsToCleanup", out List<string> skills))
            {
                skills = new List<string>();
                _scenarioContext["SkillsToCleanup"] = skills;
            }
            skills.Add(skill);
        }


        [Then("The {string} and {string} should be added and listed successfully")]
        public void ThenTheAndShouldBeAddedAndListedSuccessfully(string skill, string level)
        {
            Assert.IsTrue(_skillPage.IsSkillDisplayed(skill, level), $"Skill '{skill}' with level '{level}' was not found.");
        }


        [Given("I have added a skill {string} with level {string}")]
        public void GivenIHaveAddedASkillWithLevel(string skill, string level)
        {
            _skillPage.DeleteAllSkills();
            _skillPage.AddSkill(skill, level);
        }

        [When("I update skill {string} to new skill {string} with level {string}")]
        public void WhenIUpdateSkillToNewSkillWithLevel(string oldSkill, string newSkill, string newLevel)
        {
            _skillPage.UpdateSkill(oldSkill, newSkill, newLevel);
        }

        [Then("The {string} and {string} should be updated and listed successfully")]
        public void ThenTheAndShouldBeUpdatedAndListedSuccessfully(string skill, string level)
        {
            bool exists = _skillPage.IsSkillAndLevelPresent(skill, level);
            Assert.IsTrue(exists, $"Skill '{skill}' with level '{level}' was not found after update.");
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


        [When("I try to add the following skill entries:")]
        public void WhenITryToAddTheFollowingSkillEntries(Table dupSkillCheckTable)
        {
            foreach (var row in dupSkillCheckTable.Rows)
            {
                string dupSkill = row["DupSkill"];
                string firstLevel = row["FirstLevel"];
                string secondLevel = row["SecondLevel"];
                string expectedMessage = row["ExpectedMessage"];

                _skillPage.DeleteAllSkills();

                _skillPage.AddSkill(dupSkill, firstLevel);
                _skillPage.AddSkill(dupSkill, secondLevel);


                string actualMessage = _skillPage.DuplicateSkillErrorMsg();
                _skillPage.clickCancelButton();
                Console.WriteLine("Message displayed: " + actualMessage);

                _scenarioContext["ExpectedMessage"] = expectedMessage;
                _scenarioContext["ActualMessage"] = actualMessage;
                
            }
        }

        [Then("Expected Message should be displayed")]
        public void ThenExpectedMessageShouldBeDisplayed()
        {
            string actualMessage = _scenarioContext["ActualMessage"] as string ?? string.Empty;
            string expectedMessage = _scenarioContext["ExpectedMessage"] as string ?? string.Empty;

            Assert.That(actualMessage, Is.EqualTo(expectedMessage),
                $"Expected message '{expectedMessage}', but got '{actualMessage}'.");


        }


        [When("I try to add the same skill with change of case")]
        public void WhenITryToAddTheSameSkillWithChangeOfCase()
        {
            _skillPage.DeleteAllSkills();
            _skillPage.AddSkill("Java", "Expert");
            _skillPage.AddSkill("java", "Expert");

            string actualMessage = _skillPage.DuplicateSkillErrorMsg();
            _scenarioContext["ActualErrorMessage"] = actualMessage;

        }

        [Then("The skill should not be added and listed")]
        public void ThenTheSkillShouldNotBeAddedAndListed()
        {
            string actualMessage = _scenarioContext["ActualErrorMessage"] as string ?? string.Empty;
            bool isDuplicate = _skillPage.IsDupSkillAndLevelPresent("java", "Expert");

            string errorMessage = actualMessage ?? string.Empty;

            // Combine both assertions with a custom message
            Assert.Multiple(() =>
            {
                Assert.That(isDuplicate, Is.False, "Duplicated skill and level is getting added, but it shouldn't be.");
                Assert.That(errorMessage, Is.EqualTo("This skill is already exist in your skill list."),
                            "The error message for duplicate skill is incorrect.");
            });
        }


        [When("I try to add a skill without skill or level")]
        public void WhenITryToAddASkillWithoutSkillOrLevel(Table fieldvalidationTable)
        {
            foreach (var row in fieldvalidationTable.Rows)
            {
                string language = row["Skill"];
                string level = row["Level"];

                _skillPage.AddSkill(language, level);



                string actualMessage = _skillPage.SkillLevelFieldValidationErrMsg();
                _scenarioContext["ActualErrorMessage"] = actualMessage;
                // Console.WriteLine("Message displayed: " + actualMessage);
                _skillPage.clickCancelButton();


            }
        }

        [Then("Please enter skill and level should be displayed")]
        public void ThenPleaseEnterSkillAndLevelShouldBeDisplayed()
        {
            string actualMessage = _scenarioContext["ActualErrorMessage"] as string ?? string.Empty;
          
            string errorMessage = actualMessage;
            Assert.That(errorMessage, Is.EqualTo("Please enter skill and experience level"),
                            "The error message for field validation is incorrect.");
        }

    }
}