using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace qa_dotnet_cucumber.Pages
{
    public class LanguagePage
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;

        public LanguagePage(IWebDriver driver)
        {
            this.driver = driver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // Language add locators 
        private By languageTab => By.XPath("//a[@class='item active' and @data-tab='first' and text()='Languages']");
        private By addNewButton => By.XPath("//table[@class='ui fixed table']//th[last()]");
        private By addLanguageTextbox => By.XPath("//input[contains(@placeholder,'Add Language')]");
        private By languageLevelDropdown => By.XPath("//select[@class='ui dropdown']");
        private By saveButton => By.XPath("//input[@value='Add']");

        // Update Locators
        private IWebElement LanguageTable => wait.Until(driver =>
            driver.FindElement(By.XPath("//table[@class='ui fixed table'][.//th[normalize-space(text())='Language']]")));
        private By LanguageRow => By.XPath(".//tbody/tr");
        private By LanguageCell => By.XPath("./td[1]");
        private By EditIcon => By.XPath(".//span[@class='button']/i[contains(@class, 'write')]");
        private By EditRow => By.XPath(".//tr[.//input[@placeholder='Add Language']]");
        private By LanguageInput => By.XPath(".//input[@type='text']");
        private By LevelDropdownInEdit => By.XPath(".//select[@name='level']");
        private By UpdateButtonInEdit => By.XPath(".//input[@value='Update']");
        private By LanguageUpdatedMsg => By.XPath("//div[contains(text(),'has been updated to your languages')]");


        //Delete Locators
        private readonly By LanguageDeleteButton = By.XPath("(//i[@class='remove icon'])");
        private readonly By LanguageDeletedMsg = By.XPath("//div[contains(text(),'has been deleted from your languages')]");

        private readonly By cancelButton = By.XPath("//input[@type='button' and @value='Cancel' and contains(@class, 'ui button')]");


        //Language Validation locators
        private readonly By DuplicateLangErrMsg = By.XPath("//div[@class='ns-box ns-growl ns-effect-jelly ns-type-error ns-show']//div");
        private readonly By EmptyLangErrMsg = By.XPath("//div[@class='ns-box ns-growl ns-effect-jelly ns-type-error ns-show']//div[contains(text(),'Please enter language and level')]");

        private readonly By LanguageSuccessMsg = By.XPath("//div[@class='ns-box ns-growl ns-effect-jelly ns-type-success ns-show']//div[contains(text(),'has been added to your languages')]");

        private readonly By LanguageErrorMsg = By.XPath("//div[contains(@class,'ns-type-error') or contains(text(),'invalid input')]");



        // Validation locators

        // ------------------ Actions ------------------

        // Navigate to Language Tab
        public void NavigateToLanguageTab()
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(languageTab)).Click();
        }

        // Add new language and level
        public void AddLanguage(string language, string level)
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(addNewButton)).Click();

            var nameInput = wait.Until(ExpectedConditions.ElementIsVisible(addLanguageTextbox));
            nameInput.Clear();
            nameInput.SendKeys(language);

            var levelDropdown = wait.Until(ExpectedConditions.ElementToBeClickable(languageLevelDropdown));
            new SelectElement(levelDropdown).SelectByText(level);

            wait.Until(ExpectedConditions.ElementToBeClickable(saveButton)).Click();
        }

        // Verify if the language and level are displayed
        public bool IsLanguageDisplayed(string language, string level)
        {
            try
            {
                var locator = By.XPath($"//td[normalize-space(text())='{language}']/following-sibling::td[normalize-space(text())='{level}']");
                return wait.Until(driver => driver.FindElement(locator)).Displayed;
            }
            catch
            {
                return false;
            }
        }

        // -------- Update Method --------
        // Update existing language and level
        public void UpdateLanguage(string currentLanguage, string newLanguage, string newLevel)
        {
            try
            {
                var rows = LanguageTable.FindElements(LanguageRow);
                IWebElement? targetRow = null;

                foreach (var row in rows)
                {
                    var languageCell = row.FindElement(LanguageCell);
                    if (languageCell.Text.Trim().Equals(currentLanguage, StringComparison.OrdinalIgnoreCase))
                    {
                        targetRow = row;
                        break;
                    }
                }

                if (targetRow == null)
                    throw new Exception($"Language '{currentLanguage}' not found.");

                // Click edit icon on the target row
                targetRow.FindElement(EditIcon).Click();

                // Find the edit row inside the table (where input textbox appears)
                var editRow = LanguageTable.FindElement(EditRow);

                var languageInput = editRow.FindElement(LanguageInput);
                languageInput.Clear();
                languageInput.SendKeys(newLanguage);

                var levelDropdown = editRow.FindElement(LevelDropdownInEdit);
                new SelectElement(levelDropdown).SelectByText(newLevel);

                var updateButton = editRow.FindElement(UpdateButtonInEdit);
                wait.Until(ExpectedConditions.ElementToBeClickable(updateButton)).Click();

                // Wait for updated confirmation message
                wait.Until(ExpectedConditions.ElementIsVisible(LanguageUpdatedMsg));
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update language: {ex.Message}");
            }
        }

        // Delete all languages
        public void DeleteAllLanguages()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            while (true)
            {
                var deleteButtons = driver.FindElements(LanguageDeleteButton);
                if (deleteButtons.Count == 0)
                {
                    Console.WriteLine("All languages are deleted.");
                    break;
                }
                int initialCount = deleteButtons.Count;
                try
                {
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(deleteButtons[0]));
                    deleteButtons[0].Click();
                    wait.Until(driver =>
                    {
                        var newDeleteButtons = driver.FindElements(LanguageDeleteButton);
                        return newDeleteButtons.Count < initialCount;
                    });
                    // Thread.Sleep(500);
                }
                catch (WebDriverTimeoutException ex)
                {
                    Console.WriteLine("Timeout waiting for delete action to complete: " + ex.Message);
                    break;
                }
            }
        }
        public bool AreLanguagesPresent()
        {
            var languageRows = wait.Until(d => d.FindElements(LanguageRow));
            return languageRows.Any();
        }





        // Get duplicate language error message after trying to add a duplicate language
        public string GetDuplicateLanguageMessage()
        {
            try
            {
                var messageElement = wait.Until(ExpectedConditions.ElementIsVisible(DuplicateLangErrMsg));
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
            IWebElement errorMessageElement = driver.FindElement(EmptyLangErrMsg);
            return errorMessageElement.Text;
        }


        //Langugae validation check method for alphanumeric and special characters
        public string GetLanguageErrorMessage()
        {
            try
            {
                // Check if error message element is visible
                var error = wait.Until(ExpectedConditions.ElementIsVisible(LanguageErrorMsg)); // define LanguageErrorMsg locator
                return error.Text.Trim();
            }
            catch (WebDriverTimeoutException)
            {
                try
                {
                    var success = wait.Until(ExpectedConditions.ElementIsVisible(LanguageSuccessMsg)); // define LanguageSuccessMsg locator
                    return success.Text.Trim();
                }
                catch (WebDriverTimeoutException)
                {
                    return "No error or success message found";
                }
            }
        }

        public void AddLanguageAndCancel(string language, string level)
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(addNewButton)).Click();
            var nameInput = wait.Until(ExpectedConditions.ElementIsVisible(addLanguageTextbox));
            nameInput.Clear();
            nameInput.SendKeys(language);

            // Select language level
            var levelDropdown = wait.Until(ExpectedConditions.ElementToBeClickable(languageLevelDropdown));
            new SelectElement(levelDropdown).SelectByText(level);

            try
            {
                var cancelIcon = driver.FindElement(cancelButton);
                cancelIcon.Click();

            }
            catch (NoSuchElementException)
            {
                driver.Navigate().Refresh();
                NavigateToLanguageTab();
            }
        }
    }
        
}

