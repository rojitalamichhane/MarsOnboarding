using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using qa_dotnet_cucumber.Pages;
using Reqnroll;

namespace qa_dotnet_cucumber.Steps
{
    [Binding]
    [Scope(Feature = "Skill Functionality")]
    public class SkillSteps
    {
        private readonly LoginPage _loginPage;
        private readonly NavigationHelper _navigationHelper;
        private readonly SkillPage _skillPage;
        private readonly ScenarioContext _scenarioContext;
        public SkillSteps(LoginPage loginPage, NavigationHelper navigationHelper, SkillPage skillPage, ScenarioContext scenarioContext)
        {
            _loginPage = loginPage;
            _navigationHelper = navigationHelper;
            _skillPage = skillPage;
            _scenarioContext = scenarioContext;
        }

        [Given("I sign in to the profile page with valid username and password")]
        public void GivenISignInToTheProfilePageAsARegisteredUser()
        {
            _navigationHelper.NavigateTo("http://localhost:5003/");
            _loginPage.Login("rose@gmail.com", "rose123");
        }

        [When("I create a new {string} and {string} in my profile")]
        public void WhenICreateANewAndInMyProfile(string skill, string level)
        {
            
            _skillPage.CreateSkillLevel(skill, level);
        }

        [Then("The {string} and {string} should be created and listed successfully")]
        public void ThenTheAndShouldBeCreatedAndListedSuccessfully(string skill, string level)
        {
            string SavedSkillAddedMsg = _skillPage.SkillAddedSuccessMsg();
            string SavedSkill = _skillPage.SkillListing();
            string SavedLevel = _skillPage.LevelListing();

            Assert.That(SavedSkillAddedMsg == skill + " has been added to your skills", "Skill Added Message is not displayed successfully");
            Assert.That(SavedSkill == skill, "Skill has not been added successfully");
            Assert.That(SavedLevel == level, "Level has not been added successfully");

        }

        [When("I update an Existing Skill and Existing Level in my profile")]
        public void WhenIUpdateAnExistingSkillAndExistingLevelInMyProfile(Table updateSkilltable)
        {
            _skillPage.DeleteAllSkills();
            _skillPage.CreateSkillLevel("Java", "Expert");
            _skillPage.CreateSkillLevel("Python", "Beginner");

            var updatedSkills = new List<(string newSkill, string newLevel, string successMsg)>();

            foreach (var row in updateSkilltable.Rows)
            {
                var skill = row["Skill"];
                var newSkill = row["New Skill"];
                var newLevel = row["New Level"];

                _skillPage.UpdateSkillAndLevel(skill, newSkill, newLevel);
                string successMsg = _skillPage.SkillUpdatedSuccessMsg();
                updatedSkills.Add((newSkill, newLevel, successMsg));


            }
            _scenarioContext["updatedSkills"] = updatedSkills;
        }
        [Then("The New Skill and New Level should be updated and listed successfully")]
        public void ThenTheNewSkillAndNewLevelShouldBeUpdatedAndListedSuccessfully()
        {
            var updatedSkills = (List<(string newSkill, string newLevel, string successMsg)>)_scenarioContext["updatedSkills"];

            foreach (var (newSkill, newLevel, successMsg) in updatedSkills)
            {
                var isSkillPresent = _skillPage.IsSkillAndLevelPresent(newSkill, newLevel);
                Assert.That(isSkillPresent,
                    Is.True,
                    $"Expected to find '{newSkill}' with level '{newLevel}' in the skill list, but it was not found.");
                Assert.That(successMsg.Contains(newSkill),
                    $"Expected success message to contain '{newSkill}', but got: '{successMsg}'");
            }

        }

        [When("I delete all skills in my profile and successful message should appear")]
        public void WhenIDeleteAllSkillsInMyProfileAndSuccessfulMessageShouldAppear()
        {
            _skillPage.DeleteAllSkills();
        }

        [Then("The deleted skills should not appear in the list")]
        public void ThenTheDeletedSkillsShouldNotAppearInTheList()
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

                _skillPage.CreateSkillLevel(dupSkill, firstLevel);
                _skillPage.CreateSkillLevel(dupSkill, secondLevel);


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
            _skillPage.CreateSkillLevel("Java", "Expert");
            _skillPage.CreateSkillLevel("java", "Expert");

            string actualMessage = _skillPage.DuplicateSkillErrorMsg();
            _scenarioContext["ActualErrorMessage"] = actualMessage;

        }

        [Then("The skill should not be added and listed")]
        public void ThenTheSkillShouldNotBeAddedAndListed()
        {
            string actualMessage = _scenarioContext["ActualErrorMessage"] as string ?? string.Empty;
            bool isDuplicate = _skillPage.IsDupSkillAndLevelPresent("java", "Expert");

            string errorMessage = actualMessage;

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

                _skillPage.CreateSkillLevel(language, level);



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