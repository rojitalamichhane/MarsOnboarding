Feature: Login Functionality
  As a user, I want to log in to the Mars portal application with valid and invalid credentials.

  Background:
    Given I navigate to the login page
    And I click the sign in button

  @Order(1)
  Scenario: Successful login with valid credentials
    When I enter valid username and password
    And I submit the login form
    Then I should be logged in and see the dashboard

  @Order(2)
  Scenario: Failed login with invalid email
    When I enter invalid email and valid password
    And I submit the login form
    Then I should see an incorrect email error message

  @Order(3)
  Scenario: Failed login with invalid password
    When I enter valid email and invalid password
    And I submit the login form
    Then I should see an incorrect password error message

  @Order(4)
  Scenario: Failed login with empty credentials
    When I enter empty username and password
    And I submit the login form
    Then I should see a required field error message

  