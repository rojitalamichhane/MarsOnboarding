@SkillTest
Feature: Skills Functionality
  As a user, I want to add skills into my profile page

  Background:
    Given I sign in to the profile page as a valid user

  @Order1 @Valid @Positive @AddSkills 
  Scenario: Add new skill and skill level record with valid data
    When I add a new '<Skill>' and '<SkillLevel>' in my profile
    Then The '<Skill>' and '<SkillLevel>' should be added and listed successfully
    Examples:
      | Skill | SkillLevel        |
      | HTML  | Expert       |
      | CSS   | Intermediate |


	@Order2 @UpdateSkill
	Scenario Outline: Update existing skill and level
		Given I have added a skill '<OldSkill>' with level '<OldSkillLevel>'
		When I update skill '<OldSkill>' to new skill '<NewSkill>' with level '<NewLevel>'
		Then The '<NewSkill>' and '<NewLevel>' should be updated and listed successfully

	Examples:
		| OldSkill | OldSkillLevel         | NewSkill | NewLevel          |
		| HTML       | Expert | CSS      | Intermediate            |

      
	# Deleting all skills and levels
	@Order3 @DeleteSkill
	Scenario: Delete all existing skill and level
	When I delete all skills in my profile and successful message should appear
	Then The deleted skill should not appear in the list


	# Duplicate values check  while adding skill and level
  Scenario: Duplicate skill entries handling
    When I try to add the following skill entries:
      | DupSkill | FirstLevel | SecondLevel    | ExpectedMessage                                       |
      | Java     | Intermediate      | Intermediate          | This skill is already exist in your skill list. |
      | Java     | Intermediate      | Expert | Duplicated data    |
    Then Expected Message should be displayed

  Scenario: Duplicate skill check with change of case
    When I try to add the same skill with change of case
    Then The skill should not be added and listed


  #Skill Field Validation
  Scenario: Skill or Level field should not be empty
  When I try to add a skill without skill or level
  | Skill | Level              |
  |       | Choose Skill Level |
  | Java  | Choose Skill Level |
  |       | Beginner           |
  Then Please enter skill and level should be displayed    



	
