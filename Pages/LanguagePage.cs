using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace qa_dotnet_cucumber.Pages
{
    public class LanguagePage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        public IWebDriver Driver => _driver;

        // Locators
        private readonly By AddNewButton = By.XPath("//div[@class='ui teal button ' and normalize-space(text())='Add New']");
        private readonly By LanguageField = By.XPath("//input[contains(@placeholder,'Add Language')]");
        private readonly By LanguageLevelField = By.XPath("//select[@class='ui dropdown']");
        private readonly By AddButton = By.XPath("(//input[@type='button'])[1]");
        private readonly By AddedLanguage = By.XPath("(//table[@class='ui fixed table']//tbody[last()]//tr/td[1])[1]");
        private readonly By AddedLevel = By.XPath("//table[@class='ui fixed table']//tbody[last()]//tr/td[2]");
        private readonly By LanguageAddedMsg = By.XPath("//div[contains(text(),'has been added to your languages')]");
        private readonly By CancelButton = By.XPath("(//input[@type='button'])[2]");


        //Update Locators
        private readonly By LanguageEditButton = By.XPath("(//i[@class='outline write icon'])[2]");
        private readonly By LanguageRow = By.XPath("//table[@class='ui fixed table']/tbody/tr");
        private readonly By LanguageUpdatedMsg = By.XPath("//div[contains(@class, 'ns-box') and contains(text(), 'updated')]");


        //Delete Locators
        private readonly By LanguageDeleteButton = By.XPath("(//i[@class='remove icon'])");
        private readonly By LanguageDeletedMsg = By.XPath("//div[contains(text(),'has been deleted from your languages')]");

        //Duplicate Language locators
        //private readonly By DupLangErrMsg = By.XPath("//div[@class='ns-box ns-growl ns-effect-jelly ns-type-error ns-show']//div");
        private readonly By DupLangErrMsg = By.XPath("//div[@class='ns-box-inner']");

        private readonly By EmptyLangErrMsg = By.XPath("//div[@class='ns-box ns-growl ns-effect-jelly ns-type-error ns-show']//div[contains(text(),'Please enter language and level')]");
        private readonly By LanguageErrorMsg = By.XPath("//div[contains(@class,'ns-type-error') or contains(text(),'invalid input')]");

        private readonly By LanguageSuccessMsg = By.XPath("//div[@class='ns-box ns-growl ns-effect-jelly ns-type-success ns-show']//div[contains(text(),'has been added to your languages')]");

        private readonly By DuplicateLangErrMsg = By.XPath("//div[@class='ns-box ns-growl ns-effect-jelly ns-type-error ns-show']//div");

        private readonly By successToast = By.XPath("//div[contains(@class, 'ns-box-success')]");

        private readonly By _languageTable = By.XPath("//table[@class='ui fixed table'][.//th[normalize-space(text())='Language']]"); //whole table 

        // Definining Constructor
        public LanguagePage(IWebDriver driver) // Inject IWebDriver directly
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        //Adding New Language and Level
        public void CreateLanguageLevel(string language, string level)
        {
            try
            {
                // Click on Add New
                var addNewButtonElement = _wait.Until(ExpectedConditions.ElementToBeClickable(AddNewButton));
                addNewButtonElement.Click();

                // Wait for the input field to appear after clicking
                var languageInput = _wait.Until(ExpectedConditions.ElementIsVisible(LanguageField));
                languageInput.Clear();
                languageInput.SendKeys(language);

                var languageLevelElement = _wait.Until(ExpectedConditions.ElementIsVisible(LanguageLevelField));
                var select = new SelectElement(languageLevelElement);
                select.SelectByText(level);

                var addButton = _wait.Until(ExpectedConditions.ElementToBeClickable(AddButton));
                addButton.Click();
            }
            catch (WebDriverTimeoutException ex)
            {
                Assert.Fail($"Timed out while creating language: {ex.Message}");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Unexpected error while creating language: {ex.Message}");
            }
        }

        public void clickCancelButton()
        {
            var CancelButtonElement = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(CancelButton));
            var CancelButtonElementClickable = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(CancelButton));
            CancelButtonElementClickable.Click();
            Thread.Sleep(3000);
        }

        public bool IsCancelButtonPresent()
        {
            try
            {
                return _driver.FindElement(CancelButton).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
        public string LanguageListing()
        {
            var SavedLanguage = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(AddedLanguage));
            return SavedLanguage.Text;
            //  var SavedUpdatedLanguage = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(UpdatedLanguage));
            // return SavedUpdatedLanguage.Text;
        }
        public string LevelListing()
        {
            var SavedLevel = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(AddedLevel));
            return SavedLevel.Text;
        }

        public string LangAddedSuccessMsg()
        {
            var LangAddedMsg = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(LanguageAddedMsg));
            return LangAddedMsg.Text;

        }


        //Updating existing Language and Level

        public void UpdateLanguageAndLevel(string oldLanguage, string newLanguage, string newLevel)
        {
            try
            {
                var rows = _wait.Until(driver =>
                {
                    var allRows = driver.FindElements(LanguageRow);
                    return allRows.Any() ? allRows : null;
                });

                foreach (var row in rows)
                {
                    var languageText = row.FindElement(By.XPath("./td[1]")).Text.Trim();
                    if (languageText.Equals(oldLanguage, StringComparison.OrdinalIgnoreCase))
                    {
                        // Click edit button
                        var editButton = row.FindElement(By.XPath(".//i[contains(@class, 'outline write icon')]"));
                        _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(editButton)).Click();

                        // Wait for editable fields to appear
                        var languageField = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(LanguageField));
                        var levelDropdown = _wait.Until(d => d.FindElement(LanguageLevelField));

                        // Clear and enter new values
                        languageField.Clear();
                        languageField.SendKeys(newLanguage);

                        var select = new SelectElement(levelDropdown);
                        select.SelectByText(newLevel);

                        // Click the update button (assuming AddButton is actually update)
                        var updateBtn = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(AddButton));
                        updateBtn.Click();

                        // Wait for updated language to appear in the table (this confirms success)
                        _wait.Until(driver =>
                        {
                            var updatedRows = driver.FindElements(LanguageRow);
                            return updatedRows.Any(r =>
                            {
                                var tds = r.FindElements(By.TagName("td"));
                                if (tds.Count < 2) return false;

                                return tds[0].Text.Trim().Equals(newLanguage, StringComparison.OrdinalIgnoreCase) &&
                                       tds[1].Text.Trim().Equals(newLevel, StringComparison.OrdinalIgnoreCase);
                            });
                        });

                        return;
                    }
                }

                throw new Exception($"Language '{oldLanguage}' not found to update.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update failed: {ex.Message}");
                throw;
            }
        }

        public bool IsLanguageAndLevelPresent(string language, string level)
        {
            for (int attempt = 0; attempt < 2; attempt++)
            {
                try
                {
                    var rows = _wait.Until(d =>
                    {
                        var allRows = d.FindElements(LanguageRow);
                        return allRows.Any() ? allRows : null;
                    });

                    foreach (var row in rows)
                    {
                        var tds = row.FindElements(By.TagName("td"));
                        if (tds.Count < 2)
                            continue; // skip rows without at least 2 cells

                        var languageText = tds[0].Text.Trim();
                        var levelText = tds[1].Text.Trim();

                        Console.WriteLine($"Checking Row: {languageText} - {levelText}");

                        if (languageText == language && levelText == level)
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



        //Deleting Language and Level
        public void DeleteAllLanguages()
        {
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            while (true)
            {
                IReadOnlyCollection<IWebElement> deleteButtons;

                try
                {
                    deleteButtons = _driver.FindElements(LanguageDeleteButton);
                }
                catch (StaleElementReferenceException)
                {
                    continue;
                }

                if (deleteButtons.Count == 0)
                {
                    Console.WriteLine("All languages are deleted.");
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
                        var newDeleteButtons = driver.FindElements(LanguageDeleteButton);
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

            //Refresh the page after deletion loop is complete
            _driver.Navigate().Refresh();
            Thread.Sleep(1000);

        }

        public bool AreLanguagesPresent()
        {
            try
            {
                return _wait.Until(driver =>
                {
                    var rows = driver.FindElements(LanguageRow);
                    return rows.Any();
                });
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public void DeleteLanguage(string language)
        {
            try
            {
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
                bool deleted = false;

                while (!deleted)
                {
                    var rows = wait.Until(d => d.FindElements(LanguageRow));
                    bool found = false;

                    foreach (var row in rows)
                    {
                        var languageText = row.FindElement(By.XPath("./td[1]")).Text.Trim();
                        if (string.Equals(languageText, language, StringComparison.OrdinalIgnoreCase))
                        {
                            found = true;
                            var deleteBtn = row.FindElement(By.XPath(".//i[contains(@class, 'remove icon')]"));

                            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(deleteBtn));
                            deleteBtn.Click();

                            wait.Until(ExpectedConditions.ElementIsVisible(LanguageDeletedMsg));

                            wait.Until(d => d.FindElements(LanguageRow).Count < rows.Count);

                            deleted = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        deleted = true;
                    }
                }
            }
            catch (WebDriverTimeoutException ex)
            {
                Console.WriteLine($"Timeout in DeleteLanguage: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in DeleteLanguage: {ex.Message}");
                throw;
            }
        }



        public string GetValidationErrorMessage()
        {
            IWebElement errorMessageElement = _driver.FindElement(EmptyLangErrMsg);
            return errorMessageElement.Text;
        }
        //Langugae validation check method for alphanumeric and special characters
         public string LangLevelFieldValidationErrMsg()
        {
            var ValidationErrMsg = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(DupLangErrMsg));

            return ValidationErrMsg.Text;

        }

        public void AddLanguageAndCancel(string language, string level)
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(AddNewButton)).Click();
            var nameInput = _wait.Until(ExpectedConditions.ElementIsVisible(LanguageField));
            nameInput.Clear();
            nameInput.SendKeys(language);

            // Select language level
            var levelDropdown = _wait.Until(ExpectedConditions.ElementToBeClickable(LanguageLevelField));
            new SelectElement(levelDropdown).SelectByText(level);

            try
            {
                var cancelIcon = _driver.FindElement(CancelButton);
                cancelIcon.Click();

            }
            catch (NoSuchElementException)
            {
                _driver.Navigate().Refresh();
            }
        }

        // Verify if the language and level are displayed
        public bool IsLanguageDisplayed(string language, string level)
        {
            try
            {
                var locator = By.XPath($"//td[normalize-space(text())='{language}']/following-sibling::td[normalize-space(text())='{level}']");
                return _wait.Until(driver => driver.FindElement(locator)).Displayed;
            }
            catch
            {
                return false;
            }
        }

        // Get duplicate language error message after trying to add a duplicate language
        public string GetDuplicateLanguageMessage()
        {
            try
            {
                var messageElement = _wait.Until(ExpectedConditions.ElementIsVisible(DuplicateLangErrMsg));
                return messageElement.Text.Trim();
            }
            catch (WebDriverTimeoutException)
            {
                return string.Empty;
            }
        }


        public bool IsLanguagePresent(string language)
        {
            try
            {
                var rows = _driver.FindElements(LanguageRow);
                foreach (var row in rows)
                {
                    var languageText = row.FindElement(By.XPath("./td[1]")).Text.Trim();
                    if (languageText.Equals(language, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
        }

        //**Checking for duplicates in language field
        public string DuplicateLanguageErrorMsg()
        {
            var DuplicateLangErrMsg = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(DupLangErrMsg));

            return DuplicateLangErrMsg.Text;

        }
        public bool IsDupLanguageAndLevelPresent(string dupLang, string dupLevel)
        {
            // Find all rows in the language table
            var rows = _driver.FindElements(LanguageRow);

            foreach (var row in rows)
            {
                var languageCell = row.FindElement(By.XPath("./td[1]"));
                var levelCell = row.FindElement(By.XPath("./td[2]"));

                // Check if the language and level in the row match the provided values
                if (languageCell.Text.Trim().Equals(dupLang, StringComparison.OrdinalIgnoreCase) &&
    levelCell.Text.Trim().Equals(dupLevel, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"{languageCell.Text}, {levelCell.Text} - Duplicated data is getting saved");
                    return true;
                }
            }
            Console.WriteLine("Duplicated data is not getting saved and listed as expected");
            return false;

        }

        //changes

        public string GetSuccessMessageForAddNewLanguage(string languageToBeAdded) //To get the success message for add new languages for validation
        {
            try
            {
                var successMessage = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath($"//div[@class='ns-box-inner' and  contains(text(), '{languageToBeAdded} has been added to your languages')]")));
                return successMessage.Text;
            }
            catch
            {
                return string.Empty;
            }
        }

        public List<string> GetAllAddedLanguages() //To get the languages list after adding for validation
        {
            try
            {
                var languageTable = _wait.Until(ExpectedConditions.ElementIsVisible(_languageTable));
                var addedLanguages = new List<string>();
                var rows = languageTable.FindElements(By.XPath(".//tbody/tr"));
                foreach (var row in rows)
                {
                    var languageCell = row.FindElement(By.XPath("./td[1]"));
                    addedLanguages.Add(languageCell.Text.Trim());
                }

                return addedLanguages;
            }
            catch
            {
                return new List<string>();
            }
        }
        
       
        
    }
}

