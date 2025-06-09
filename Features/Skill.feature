Feature: Skill Functionality
  As a user, I want to log in to the Mars portal application to create, edit
  and delete skill functionality.  I would be able to show what skills I have

  Background: 
  Given I sign in to the profile page with valid username and password

  # Adding New Skill and level
  @Order1 @AddSkills
  Scenario Outline: Create new skill and level record with valid data
    When I create a new '<Skill>' and '<Level>' in my profile
    Then The '<Skill>' and '<Level>' should be created and listed successfully
    Examples: 
      | Skill              | Level        |
      | HTML  | Expert       |
      | CSS   | Intermediate |
      | JS   | Beginner |
  
  #Updating Existing skill and level
  @Order2 @UpdateSkills
  Scenario: Update the existing skill and level with valid data
    When I update an Existing Skill and Existing Level in my profile
    | Skill  | New Skill  | New Level    |
    | Java   | C#         | Intermediate |
    | Python | TypeScript | Expert       |
    Then The New Skill and New Level should be updated and listed successfully
   

  #Deleting All Skills and level
  @Order3 @DeleteSkills
  Scenario: Delete all existing skill and level
    When I delete all skills in my profile and successful message should appear
    Then The deleted skills should not appear in the list


  #Duplicate values check  while adding skill and level
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

 