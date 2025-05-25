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
      | English   | Fluent  |


  #Updating Existing language and level
  @Order2
  Scenario Outline: Update the existing language and level with valid data
    When I update existing language '<Language>' to new language '<New Language>' and level '<New Level>'
    Then The updated language '<New Language>' and '<New Level>' should be listed successfully

  Examples:
    | Language | New Language | New Level      |
    | Nepali   | Hindi        | Conversational |
    | English  | Maori        | Basic          |


 # Deleting all languages and levels
  @Order3 @DeleteLanguage
  Scenario: Delete all existing language and level
  When I delete all languages in my profile and successful message should appear
  Then The deleted language should not appear in the list


 # Duplicate language and level entries handling
  @Order4
  Scenario Outline: Try to add duplicate language and level entries
    When I try to add a duplicate language '<Language>' with level '<Level>' in my profile
    Then I should see the duplicate error message '<ExpectedMessage>'

    Examples:
      | Language | Level             | ExpectedMessage                                   |
      | English  | Fluent            | This language is already exist in your language list. |
      | Nepali   | Native/Bilingual  | This language is already exist in your language list. |



  # Language field validation - empty inputs
  @Order5
  Scenario Outline: Try to add a language with empty language or level
    When I try to add language '<Language>' with level '<Level>' in my profile
    Then I should see the error message '<ExpectedMessage>'

    Examples:
      | Language | Level                | ExpectedMessage                            |
      |          | Choose Language Level| Please enter language and level            |
      | English  | Choose Language Level| Please enter language and level            |
      |          | Basic                | Please enter language and level            |


  #Language Field validation for alphabet, numbers and special characters
  @Order6 @LanguageFieldValidation
    Scenario Outline: As a user I should not be able to add invalid inputs
    When I add the following languages and select level:
      | Language       | Level |
      | <Language>     | <Level> |
    Then Error message prompt should be displayed 

    Examples:
      | Language                               | Level |
      | 123456789012345678901234567890      | Basic  |
      | !@#$%^&*()_+                         | Conversational |
      | 1a                                   | Fluent     |

  
 # Language add cancel
  @Order7 @LanguageCancellation
  Scenario: As a user, I should be able to cancel the add operation
    When I start adding the language "Mandarin" with level "Basic" and cancel the operation
    Then the language "Mandarin" should not be added to the list


     
      









