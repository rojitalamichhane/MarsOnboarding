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
        private readonly By SkillRow = By.XPath("//table[@class='ui fixed table']//tbody/tr");
        private By SkillCell => By.XPath("./td[1]");
        private By skillEditIcon => By.XPath("//table[@class='ui fixed table']//th[text()='Skill']/ancestor::table//i[@class='outline write icon']");
        private By skillEditRow => By.XPath(".//tr[.//input[@placeholder='Add Skill']]");
        private By skillInputField => By.XPath(".//input[@type='text']");
        private By skillLevelDropdownInEdit => By.XPath("//select[@name='level' and contains(@class, 'ui fluid dropdown')]");
        private By skillUpdateButtonInEdit => By.XPath(".//input[@value='Update']");
        private readonly By CancelButton = By.XPath("(//input[@type='button'])[2]");
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

        private readonly By DupSkillErrMsg = By.XPath("//div[@class='ns-box-inner']");


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

        public void clickCancelButton()
        {
            var CancelButtonElement = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(CancelButton));
            var CancelButtonElementClickable = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(CancelButton));
            CancelButtonElementClickable.Click();
            Thread.Sleep(3000);
        }

        //Update existing skill and level

        public void UpdateSkill(string oldSkill, string newSkill, string newLevel)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                // Wait until at least one skill row is present
                wait.Until(d => d.FindElements(By.XPath("//table[@class='ui fixed table']//tbody/tr")).Count > 0);

                var rows = driver.FindElements(By.XPath("//table[@class='ui fixed table']//tbody/tr"));
                bool skillFound = false;

                foreach (var row in rows)
                {
                    var skillText = row.FindElement(By.XPath("./td[1]")).Text.Trim();

                    Console.WriteLine($"Checking skill: '{skillText}'");

                    if (skillText.Equals(oldSkill, StringComparison.OrdinalIgnoreCase))
                    {
                        skillFound = true;

                        // Click edit icon
                        row.FindElement(By.XPath("./td[3]/span[1]/i")).Click();

                        // Wait for input fields to be visible
                        wait.Until(ExpectedConditions.ElementIsVisible(By.Name("name")));

                        // Input new skill
                        var skillInput = row.FindElement(By.Name("name"));
                        skillInput.Clear();
                        skillInput.SendKeys(newSkill);

                        // Select new level
                        var levelDropdown = new SelectElement(row.FindElement(By.Name("level")));
                        levelDropdown.SelectByText(newLevel);

                        // Click update
                        row.FindElement(By.XPath("./td[3]/span/input[@value='Update']")).Click();

                        // Wait for success toast (adjust selector if needed)
                        wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("toast-message")));

                        return;
                    }
                }

                if (!skillFound)
                    throw new Exception($"Skill '{oldSkill}' not found to update.");
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to update skill: " + ex.Message);
            }
        }

        public bool IsSkillAndLevelPresent(string skill, string level)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            for (int attempt = 0; attempt < 2; attempt++)
            {
                try
                {
                    var rows = wait.Until(d =>
                    {
                        var allRows = d.FindElements(SkillRow);
                        return allRows.Any() ? allRows : null;
                    });

                    foreach (var row in rows)
                    {
                        var tds = row.FindElements(By.TagName("td"));
                        if (tds.Count < 2)
                            continue;

                        var skillText = tds[0].Text.Trim();
                        var levelText = tds[1].Text.Trim();

                        Console.WriteLine($"Checking Row: {skillText} - {levelText}");

                        if (skillText.Equals(skill, StringComparison.OrdinalIgnoreCase) &&
                            levelText.Equals(level, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }

                    return false;
                }
                catch (StaleElementReferenceException)
                {
                    Thread.Sleep(1000);
                }
                catch (WebDriverTimeoutException ex)
                {
                    Console.WriteLine("Timeout while waiting for rows: " + ex.Message);
                    return false;
                }
            }

            return false;
        }

        //Delete SKills
        public void DeleteAllSkills()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            while (true)
            {
                IReadOnlyCollection<IWebElement> deleteButtons;

                try
                {
                    deleteButtons = driver.FindElements(skillDeleteButton);
                }
                catch (StaleElementReferenceException)
                {
                    continue;
                }

                if (deleteButtons.Count == 0)
                {
                    Console.WriteLine("All skills are deleted.");
                    break;
                }

                try
                {
                    int initialCount = deleteButtons.Count;

                    // Click the first delete button
                    IWebElement deleteButton = wait.Until(
                        SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(deleteButtons.First()));
                    deleteButton.Click();

                    // Wait for toast message confirming deletion
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(
                        By.XPath("//div[contains(text(),'has been deleted')]")));

                    // Wait until a delete button disappears
                    wait.Until(driver =>
                    {
                        var newDeleteButtons = driver.FindElements(skillDeleteButton);
                        return newDeleteButtons.Count < initialCount;
                    });
                }
                catch (WebDriverTimeoutException ex)
                {
                    Console.WriteLine("Timeout waiting for delete action to complete: " + ex.Message);
                    break;
                }
                catch (StaleElementReferenceException)
                {
                    continue;
                }
            }

            // Optional: refresh to stabilize UI after all deletions
            driver.Navigate().Refresh();
            Thread.Sleep(1000);
        }

        public bool AreSkillsPresent()
        {
            var deleteButtons = driver.FindElements(skillDeleteButton);
            return deleteButtons.Count > 0;
        }

        public void DeleteSpecificSkill(string skill)
        {
            try
            {
                Console.WriteLine($"🔍 Trying to delete skill: {skill}");
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

                bool deleted = false;
                int retry = 0;

                while (!deleted && retry < 5)
                {
                    // Re-fetch table rows each time to avoid stale elements
                    var rows = driver.FindElements(SkillRow);
                    foreach (var row in rows)
                    {
                        var skillText = row.FindElement(By.XPath("./td[1]")).Text.Trim();
                        if (string.Equals(skillText, skill.Trim(), StringComparison.OrdinalIgnoreCase))
                        {
                            var deleteBtn = row.FindElement(By.XPath(".//i[contains(@class, 'remove icon')]"));
                            wait.Until(ExpectedConditions.ElementToBeClickable(deleteBtn)).Click();

                            // Wait for toast confirmation
                            wait.Until(ExpectedConditions.ElementIsVisible(skillDeletedMsg));

                            Console.WriteLine($"Deleted skill: {skill}");
                            deleted = true;
                            break;
                        }
                    }

                    retry++;
                    if (!deleted)
                    {
                        Console.WriteLine($"🔁 Retry #{retry} for skill: {skill}");
                        Thread.Sleep(1000); // Optional: Give DOM time to settle
                    }
                }

                if (!deleted)
                    Console.WriteLine($"Could not find or delete skill: {skill}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Exception while deleting skill '{skill}': {ex.Message}");
            }
        }

        //**Checking for duplicates in skill field
        public string DuplicateSkillErrorMsg()
        {
            var DuplicateSkillErrMsg = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(DupSkillErrMsg));

            return DuplicateSkillErrMsg.Text;

        }
        public bool IsDupSkillAndLevelPresent(string dupSkill, string dupLevel)
        {
            // Find all rows in the skill table
            var rows = driver.FindElements(SkillRow);

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

        //**Skill and Level field validation
        public string SkillLevelFieldValidationErrMsg()
        {
            var ValidationErrMsg = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(DupSkillErrMsg));

            return ValidationErrMsg.Text;

        }

    }
}
