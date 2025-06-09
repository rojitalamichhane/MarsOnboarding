using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.BiDi.Modules.Log;
using OpenQA.Selenium.Support.UI;
using RazorEngine;
using SeleniumExtras.WaitHelpers;

namespace qa_dotnet_cucumber.Pages
{
    public class SkillPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        public IWebDriver Driver => _driver;

        // Locators
        private readonly By SkillsButton = By.XPath("//div[@class='ui fluid container']//a[2]");

        

        private readonly By AddNewButton = By.XPath("(//table[@class='ui fixed table'])[2]//div");
        
        private readonly By SkillField = By.XPath("(//input[@type='text'])[4]");
        private readonly By SkillLevelField = By.XPath("//select[@class='ui fluid dropdown']");
        private readonly By AddButton = By.XPath("(//input[@type='button'])[1]");
        private readonly By AddedSkill = By.XPath("(//table[@class='ui fixed table']//tbody[last()]//tr/td[1])[1]");
        private readonly By AddedLevel = By.XPath("//table[@class='ui fixed table']//tbody[last()]//tr/td[2]");
        private readonly By SkillAddedMsg = By.XPath("//div[contains(text(),'has been added to your skills')]");
        private readonly By CancelButton = By.XPath("(//input[@type='button'])[2]");

        //Update Locators
        private readonly By SkillEditButton = By.XPath("(//i[@class='outline write icon'])[2]");
        private readonly By SkillRow = By.XPath("//table[@class='ui fixed table']/tbody/tr");
        private readonly By SkillUpdatedMsg = By.XPath("//div[contains(text(),'has been updated to your skills')]");


        //Delete Locators
        private readonly By SkillDeleteButton = By.XPath("//div[@data-tab='second']//i[@class='remove icon']");
        private readonly By SkillDeletedMsg = By.XPath("//div[@class='ns-box ns-growl ns-effect-jelly ns-type-error ns-show']//div");

        //Duplicate Skill locators
        private readonly By DupSkillErrMsg = By.XPath("//div[@class='ns-box-inner']");


        // Definining Constructor
        public SkillPage(IWebDriver driver) // Inject IWebDriver directly
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30)); // 10-second timeout
            
        }

        public void SkillsTab()
        {
            try
            {
                // Click on Skills button
                var skillsButtonElement = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(SkillsButton));
                skillsButtonElement.Click();
            }
            catch (Exception)
            {
                Assert.Fail("Skills Button has not been found");
            }
        }

        //Adding New Skill and Level
        public void CreateSkillLevel(string skill, string level)
        {   
            //Create Skill and level through Add New
            // Click Add New
            try
            {
                // Click on Add New button
                var addNewButtonElement = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(AddNewButton));
                addNewButtonElement.Click();
            }
            catch (Exception)
            {
                Assert.Fail("Add New Button has not been found");
            }

            //Enter Skill
            var SkillElement = _wait.Until(ExpectedConditions.ElementIsVisible(SkillField));
            SkillElement.SendKeys(skill);

            //Choose skill level from dropdown
            var skillLevelElement = _wait.Until(d => d.FindElement(SkillLevelField));
            SelectElement skillLevel = new SelectElement(skillLevelElement);
            skillLevel.SelectByText(level);

            //Click Add button

            var AddButtonElement = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(AddButton));
            var AddButtonElementClickable = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(AddButton));
            AddButtonElementClickable.Click();
            Thread.Sleep(2000);

        }
        public void clickCancelButton()
        {
            var CancelButtonElement = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(CancelButton));
            var CancelButtonElementClickable = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(CancelButton));
            CancelButtonElementClickable.Click();
            Thread.Sleep(3000);
        }
        public string SkillListing()
        {
            var SavedSkill = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(AddedSkill));
            return SavedSkill.Text;
            
        }
        public string LevelListing()
        {
            var SavedLevel = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(AddedLevel));
            return SavedLevel.Text;
        }

        public string SkillAddedSuccessMsg()
        {
            var SkillAddedMessage = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(SkillAddedMsg));
            return SkillAddedMessage.Text;
        }

        //Updating existing Skill and Level

        public void UpdateSkillAndLevel(string skill, string newSkill, string newLevel)
        {
            try
            {
                // Find all rows in the table body
                var skillRows = _wait.Until(d => d.FindElements(SkillRow));

                foreach (var row in skillRows)
                {
                    var skillText = row.FindElement(By.XPath("./td[1]")).Text.Trim();
                    if (skillText.Equals(skill, StringComparison.OrdinalIgnoreCase))
                    {
                        // Click the edit icon in that row
                        var editButton = row.FindElement(By.XPath(".//i[contains(@class, 'outline write icon')]"));
                        editButton.Click();

                        Thread.Sleep(1000); // Wait for input to appear

                        // Update the skill and level
                        var skillElement = _wait.Until(ExpectedConditions.ElementIsVisible(SkillField));
                        skillElement.Clear();
                        skillElement.SendKeys(newSkill);

                        // Choose skill level from dropdown
                        var skillLevelElement = _wait.Until(d => d.FindElement(SkillLevelField));
                        SelectElement skillLevel = new SelectElement(skillLevelElement);
                        skillLevel.SelectByText(newLevel);

                        // Click Add button
                        var addButtonElement = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(AddButton));
                        addButtonElement.Click();

                        Thread.Sleep(3000);

                        return;
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Test failed: " + ex.Message);
            }
        }
        public string SkillUpdatedSuccessMsg()
        {
            var SKillUpdatedMessage = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(SkillUpdatedMsg));
            return SKillUpdatedMessage.Text;
        }

        public bool IsSkillAndLevelPresent(string skill, string level)
        {
            // Find all rows in the skill table
            var rows = _driver.FindElements(SkillRow);

            foreach (var row in rows)
            {
                var skillCell = row.FindElement(By.XPath("./td[1]"));
                var levelCell = row.FindElement(By.XPath("./td[2]"));

                // Check if the skill and level in the row match the provided values
                if (skillCell.Text.Trim() == skill && levelCell.Text.Trim() == level)
                {
                    return true; // Found the matching skill and level
                }
            }

            return false;
        }



        //Deleting SKill and Level
        public void DeleteAllSkills()
        {
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
            while (true)
            {
                var deleteButtons = _driver.FindElements(SkillDeleteButton);

                if (deleteButtons.Count == 0)
                {
                    Console.WriteLine("All skills are deleted.");
                    break;
                }
                int initialCount = deleteButtons.Count;
                try
                {

                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(deleteButtons[0]));
                    deleteButtons[0].Click();
                    wait.Until(driver =>
                    {
                        var newDeleteButtons = driver.FindElements(SkillDeleteButton);
                        return newDeleteButtons.Count < initialCount;
                    });

                    Thread.Sleep(500);
                }
                catch (WebDriverTimeoutException ex)
                {
                    Console.WriteLine("Timeout waiting for delete action to complete: " + ex.Message);
                    break;
                }
            }
        }

        public bool AreSkillsPresent()
        {
            var skillRows = _wait.Until(d => d.FindElements(SkillRow));
            return skillRows.Any();
        }

        //Checking for duplicates in skill field
        public string DuplicateSkillErrorMsg()
        {
            var DuplicateSkillErrMsg = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(DupSkillErrMsg));

            return DuplicateSkillErrMsg.Text;

        }
        public bool IsDupSkillAndLevelPresent(string dupSkill, string dupLevel)
        {
            // Find all rows in the skill table
            var rows = _driver.FindElements(SkillRow);

            foreach (var row in rows)
            {
                var skillCell = row.FindElement(By.XPath("./td[1]"));
                var levelCell = row.FindElement(By.XPath("./td[2]"));

                // Check if the skill and level in the row match the provided values
                if (skillCell.Text.Trim().Equals(dupSkill, StringComparison.OrdinalIgnoreCase) &&
    levelCell.Text.Trim().Equals(dupLevel, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"{skillCell.Text}, {levelCell.Text} - Duplicated data is getting saved");
                    return true;
                }
            }
            Console.WriteLine("Duplicated data is not getting saved and listed as expected");
            return false;

        }

        //skill and Level field validation
        public string SkillLevelFieldValidationErrMsg()
        {
            var ValidationErrMsg = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(DupSkillErrMsg));

            return ValidationErrMsg.Text;

        }

    }
}