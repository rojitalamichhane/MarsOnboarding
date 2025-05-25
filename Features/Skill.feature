@SkillTest
Feature: Skills Functionality
  As a user, I want to add skills into my profile page

  Background:
    Given I sign in to the profile page as a valid user
  
  @Order1 @Valid @Positive
  Scenario: Add new skill and skill level record with valid data
    When I add a new '<Skill>' and '<SkillLevel>' in my profile
    Then The '<Skill>' and '<SkillLevel>' should be added and listed successfully
    Examples:
      | Skill | SkillLevel        |
      | HTML  | Expert       |
      | CSS   | Intermediate |
      | JS   | Beginner |

  
  #Updating Existing Skill and Level
  @Order2 @UpdateSkill
  Scenario Outline: Update the existing skill and skill level with valid data
    When I update existing skill '<Skill>' to new skill '<New Skill>' and level '<New Skill Level>'
    Then The updated skill '<New Skill>' and '<New Skill Level>' should be listed successfully

  Examples:
    | Skill | New Skill | New Skill Level      |
    | HTML   | Database        | Beginner |


  # Deleting all skills and levels
  @Order3 @DeleteSkill
  Scenario: Delete all existing skill and level
  When I delete all skills in my profile and successful message should appear
  Then The deleted skill should not appear in the list

  # Duplicate skill and level entries handling
  @Order4 @DuplicateSkill
  Scenario Outline: Try to add duplicate skill and level entries
    When I try to add a duplicate skill '<Skill>' with level '<Level>' in my profile
    Then I should see the duplicate skill error message '<ExpectedMessage>'

    Examples:
      | Skill | Level             | ExpectedMessage                                   |
      | JS  | Beginner            | This skill is already exist in your skill list. |

  # Skill field validation - empty inputs
  @Order5 @EmptyInputs
  Scenario Outline: Try to add a skill with empty skill or level
    When I try to add skill '<Skill>' with level '<Level>' in my profile
    Then I should see the empty skill error message '<ExpectedMessage>'

    Examples:
      | Skill | Level                | ExpectedMessage                            |
      |          | Choose Skill Level| Please enter skill and experience level            |
      | JS  | Choose Skill Level| Please enter skill and experience level            |


   @Order6 @SkillFieldValidation
    Scenario Outline: As a user I should not be able to add invalid inputs
    When I add the following skills and select skill level:
      | Skill       | SkillLevel |
      | <Skill>     | <SkillLevel> |
    Then Error message should be displayed

    Examples:
      | Skill                               | SkillLevel |
      | 123456789012345678901234567890      | Beginner   |
      | !@#$%^&*()_+                         | Intermediate |
      | 123Skill                            | Expert     |
      | 1a                                   | Expert     |

  # Skill add cancel
  @Order7 @SkillCancellation
  Scenario: As a user, I should be able to cancel the skill add operation
    When I start adding the skill "HTML" with level "Fluent" and cancel the operation
    Then the skill "HTML" should not be added to the list




     
    


      




