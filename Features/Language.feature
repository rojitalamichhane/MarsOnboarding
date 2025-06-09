Feature: Language Functionality
  As a user, I want to log in to the Mars portal application to create, edit, and delete languages.
  I should be able to add languages I know.

  Background:
    Given I sign in to the profile page with valid email address and password

  #Adding Language and level
  @Order1 @AddLanguage
  Scenario: Create new language and level record with valid data
    When I create a new '<Language>' and '<Level>' in my profile
    Then The '<Language>' and '<Level>' should be created and listed successfully

    Examples:
      | Language | Level   |
      | Nepali  | Native/Bilingual   |
      | English | Fluent           |
      | French | Conversational   |
      | Spanish | Basic            |
      

  
 #Updating Existing language and level
  @Order2 @UpdateLanguage
  Scenario Outline: Update the existing language and level with valid data
    When I update an Existing Language and Existing Level in my profile
    | Language          | New Language | New Level           |
    | English           | Hindi       | Basic           |
    | Spanish           | Arab         | Conversational  |
    Then The New Language and New Level should be updated and listed successfully


 # Deleting all languages and levels
  @Order3 @DeleteLanguage
  Scenario: Delete all existing language and level
  When I delete all languages in my profile and successful message should appear
  Then The deleted language should not appear in the list


 # Duplicate language and level entries handling
  @Order4 @DuplicateLang
  Scenario Outline: Try to add duplicate language and level entries
    When I try to add the following language entries:
      | DuplicateLanguage | FirstLevel | SecondLevel    | ExpectedMessage                                       |
      | English     | Basic      | Basic          | This language is already exist in your language list. |
      | English     | Basic      | Conversational | Duplicated data    |
    Then ExpectedMessage should be displayed



  # Language empty input check
    @Order5 @EmptyLang
    Scenario Outline: Try to add a language with empty language or level
      When I try to add a language without language or level
      | Language | Level                 |
      |          | Choose Language Level |
      | English  | Choose Language Level |
      |          | Basic                 |
      Then Please enter language and level should be displayed



  #Language Field validation for alphabet, numbers and special characters
  @Order6 @LanguageFieldValidation
    Scenario Outline: As a user I should not be able to add invalid inputs
    When I try to enter non-alphabet characters in language field
      | InvalidLanguage                       | Level            | ExpectedMessage            |
      | 12345678901234567890 | Basic            | Please enter only language |
      | !@#$%^&*(){}!@#| Conversational   | Please enter only language |
      | $$$$$$$$$$$$$$$$$ | Fluent           | Please enter only language |
      | %12232nglish*****Spanish#Nepali  | Native/Bilingual | Please enter only language |
    Then Error message should be displayed

 