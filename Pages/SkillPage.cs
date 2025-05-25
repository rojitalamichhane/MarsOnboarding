using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace qa_dotnet_cucumber.Pages
{
    public class SkillPage
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;

        public SkillPage(IWebDriver driver)
        {
            this.driver = driver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // Skill tab and add locators
        private By skillTab => By.XPath("//a[@class='item' and @data-tab='second' and text()='Skills']");
        private By addNewSkillButton => By.XPath("//div[contains(@class, 'active') and @data-tab='second']//table[@class='ui fixed table']//th[last()]");
        private By addSkillTextbox => By.XPath("//input[contains(@placeholder,'Add Skill')]");
        private By skillLevelDropdown => By.XPath("//select[@class='ui fluid dropdown']");
        private By skillSaveButton => By.XPath("//input[@value='Add']");

        // Update Locators
        private IWebElement SkillTable => wait.Until(driver =>
            driver.FindElement(By.XPath("//table[@class='ui fixed table'][.//th[normalize-space(text())='Skill']]")));
        private By SkillRow => By.XPath(".//tbody/tr");
        private By SkillCell => By.XPath("./td[1]");
        private By skillEditIcon => By.XPath("//table[@class='ui fixed table']//th[text()='Skill']/ancestor::table//i[@class='outline write icon']");
        private By skillEditRow => By.XPath(".//tr[.//input[@placeholder='Add Skill']]");
        private By skillInputField => By.XPath(".//input[@type='text']");
        private By skillLevelDropdownInEdit => By.XPath("//select[@name='level' and contains(@class, 'ui fluid dropdown')]");
        private By skillUpdateButtonInEdit => By.XPath(".//input[@value='Update']");
        private By skillUpdatedMsg => By.XPath("//div[contains(text(),'has been updated to your skills')]");

        // Delete Locators
        private readonly By skillDeleteButton = By.XPath("//table[.//th[text()='Skill']]//i[@class='remove icon']");
        private readonly By skillDeletedMsg = By.XPath("//div[contains(text(),'has been deleted')]");

        private readonly By skillCancelButton = By.XPath("//input[@type='button' and @value='Cancel' and contains(@class, 'ui button')]");

        // Validation locators
        private readonly By DuplicateSkillErrMsg = By.XPath("//div[@class='ns-box-inner' and contains(text(), 'skill is already exist')]");
        private readonly By EmptySkillErrMsg = By.XPath("//div[@class='ns-box ns-growl ns-effect-jelly ns-type-error ns-show']//div[contains(text(),'Please enter skill and experience level')]");
        private readonly By SkillSuccessMsg = By.XPath("//div[@class='ns-box ns-growl ns-effect-jelly ns-type-success ns-show']//div[contains(text(),'has been added to your skills')]");

        private readonly By SkillErrorMsg = By.XPath("//div[contains(@class,'ns-type-error') or contains(text(),'invalid')]");

        // ------------------ Actions ------------------

        // Navigate to Skill Tab
        public void NavigateToSkillTab()
        {
            Console.WriteLine("Waiting for Skill tab to be clickable...");
            wait.Until(ExpectedConditions.ElementToBeClickable(skillTab)).Click();
            Console.WriteLine("Clicked on Skill tab.");
        }

        // Add new skill and level
        public void AddSkill(string skill, string level)
        {
            Console.WriteLine("Clicking 'Add New' for skill...");
            wait.Until(ExpectedConditions.ElementToBeClickable(addNewSkillButton)).Click();

            Console.WriteLine("Entering skill...");
            var nameInput = wait.Until(ExpectedConditions.ElementIsVisible(addSkillTextbox));
            nameInput.Clear();
            nameInput.SendKeys(skill);

            Console.WriteLine("Selecting level...");
            var levelDropdown = wait.Until(ExpectedConditions.ElementToBeClickable(skillLevelDropdown));
            new SelectElement(levelDropdown).SelectByText(level);

            Console.WriteLine("Clicking 'Add' button...");
            wait.Until(ExpectedConditions.ElementToBeClickable(skillSaveButton)).Click();
        }

        // Verify if the skill and level are displayed in the table
        public bool IsSkillDisplayed(string skill, string level)
        {
            try
            {
                Console.WriteLine("Checking if skill is displayed...");
                var locator = By.XPath($"//td[normalize-space(text())='{skill}']/following-sibling::td[normalize-space(text())='{level}']");
                return wait.Until(driver => driver.FindElement(locator)).Displayed;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        //Update existing skill and level
        public void UpdateSkill(string currentSkill, string newSkill, string newLevel)
        {
            try
            {
                Console.WriteLine("Already on Skill tab. Starting update...");
                // No need to call NavigateToSkillTab() here

                var rows = SkillTable.FindElements(SkillRow);
                IWebElement? targetRow = null;

                foreach (var row in rows)
                {
                    var skillCell = row.FindElement(SkillCell);
                    if (skillCell.Text.Trim().Equals(currentSkill, StringComparison.OrdinalIgnoreCase))
                    {
                        targetRow = row;
                        break;
                    }
                }

                if (targetRow == null)
                    throw new Exception($"Skill '{currentSkill}' not found.");

                // Click edit icon on the target row
                targetRow.FindElement(skillEditIcon).Click();

                var editRow = SkillTable.FindElement(skillEditRow);

                var skillInput = editRow.FindElement(skillInputField);
                skillInput.Clear();
                skillInput.SendKeys(newSkill);

                var levelDropdown = editRow.FindElement(skillLevelDropdownInEdit);
                new SelectElement(levelDropdown).SelectByText(newLevel);

                var updateButton = editRow.FindElement(skillUpdateButtonInEdit);
                wait.Until(ExpectedConditions.ElementToBeClickable(updateButton)).Click();

                wait.Until(ExpectedConditions.ElementIsVisible(skillUpdatedMsg));
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update skill: {ex.Message}");
            }
        }

        //Delete SKills
       public void DeleteAllSkills()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            while (true)
            {
                // Find delete buttons fresh on each iteration
                var deleteButtons = driver.FindElements(skillDeleteButton);
                if (deleteButtons.Count == 0)
                {
                    Console.WriteLine("All skills deleted.");
                    break;
                }

                try
                {
                    var deleteButton = deleteButtons[0];
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(deleteButton));
                    deleteButton.Click();

                    try
                    {
                        wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent());
                        var alert = driver.SwitchTo().Alert();
                        alert.Accept();
                    }
                    catch (WebDriverTimeoutException)
                    {
                        // No alert appeared, continue
                    }

                    // Wait until the skill is removed from the list (count decreases)
                    bool skillDeleted = wait.Until(driver =>
                    {
                        var currentCount = driver.FindElements(skillDeleteButton).Count;
                        return currentCount < deleteButtons.Count;
                    });

                    if (!skillDeleted)
                    {
                        throw new Exception("Skill deletion did not reduce the skill count.");
                    }
                }
                catch (StaleElementReferenceException)
                {
                    continue;
                }
                catch (WebDriverTimeoutException ex)
                {
                    Console.WriteLine("Timeout waiting for delete action: " + ex.Message);
                    break;
                }
            }
        }
       public bool AreSkillsPresent()
        {
            var deleteButtons = driver.FindElements(skillDeleteButton);
            return deleteButtons.Count > 0;
        }

        // Get duplicate skill error message after trying to add a duplicate skill
        public string GetDuplicateSkillMessage()
        {
            try
            {
                var messageElement = wait.Until(ExpectedConditions.ElementIsVisible(DuplicateSkillErrMsg));
                return messageElement.Text.Trim();
            }
            catch (WebDriverTimeoutException)
            {
                return string.Empty;
            }
        }

        // Get empty language error message when trying to add an empty language and level
        public string GetValidationErrorMessage()
        {
            IWebElement errorMessageElement = driver.FindElement(EmptySkillErrMsg);
            return errorMessageElement.Text;
        }

        //Validation check for skill fields

        public string GetSkillErrorMessage()
        {
            try
            {
                var error = wait.Until(ExpectedConditions.ElementIsVisible(SkillErrorMsg));
                return error.Text.Trim();
            }
            catch (WebDriverTimeoutException)
            {
                try
                {
                    var success = wait.Until(ExpectedConditions.ElementIsVisible(SkillSuccessMsg));
                    return success.Text.Trim(); // This means invalid input was wrongly accepted
                }
                catch (WebDriverTimeoutException)
                {
                    return "No error or success message found";
                }
            }
        }

                  }
        }
    



