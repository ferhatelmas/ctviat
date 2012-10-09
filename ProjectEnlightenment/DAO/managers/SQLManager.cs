using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Common.test;
using Common.question;
using Common.user;
using Common.answer;

using DAO.CTVIATBankDataSetTableAdapters;

// Do you really want to learn what is happening here?
namespace DAO.managers
{
  public sealed class SQLManager
  {
    //table adapter definitions for each table
    private static UsersTableAdapter usersAdapter = new UsersTableAdapter();
    private static TestsTableAdapter testsAdapter = new TestsTableAdapter();
    private static TestsCandidatesTableAdapter testsCandidatesAdapter = new TestsCandidatesTableAdapter();
    private static TestsTakeOrderTableAdapter testsTakeOrderAdapter = new TestsTakeOrderTableAdapter();
    private static TestsTimeTableAdapter testsTimeAdapter = new TestsTimeTableAdapter();
    private static IncompleteTestsCandidatesTableAdapter incompleteTestsAdapter = 
        new IncompleteTestsCandidatesTableAdapter();
    private static QuestionsTableAdapter questionsAdapter = new QuestionsTableAdapter();
    private static QuestionsTestsTableAdapter questionsTestsAdapter = new QuestionsTestsTableAdapter();

    #region User Methods

    /// <summary>
    ///   login window uses this method to authenticate user
    /// </summary>
    /// <param name="username">username supplied by user</param>
    /// <param name="password">password supplied by user</param>
    /// <returns>
    ///   if authentication is successful, Candidate or Admin object is returned according to user type, 
    ///   otherwise null 
    /// </returns>
    public static User authenticateUser(String username, String password)
    {
      //get user
      CTVIATBankDataSet.UsersDataTable table = usersAdapter.GetUserByUserNameAndPassword(username, password);
        
      //if no user with supplied credentials, return null
      if (table.Rows.Count == 0)
      {
        return null;
      }
      //otherwise create user object
      else
      {
        DataRow row = table.Rows[0];
        if (Convert.ToByte(row["userType"]) == 1)
        {
          return new Admin(Convert.ToInt32(row["userID"]), Convert.ToString(row["userName"]), 
            Convert.ToString(row["userPassword"]));
        }
        else
        {
          return new Candidate(Convert.ToInt32(row["userID"]), Convert.ToString(row["userName"]), 
            Convert.ToString(row["userPassword"]));
        }
      }
    }

    /// <summary>
    ///   Adds user with given credentials
    /// </summary>
    /// <param name="userName">Username of the new user</param>
    /// <param name="password">Password of the new user</param>
    /// <param name="type">Type of the new user => 1=admin, other than 1=candidate</param>
    public static void addUser(String userName, String password, byte type)
    {
      usersAdapter.InsertUser(userName, password, type);
    }

    /// <summary>
    ///   Used to get information of all users
    /// </summary>
    /// <returns>
    ///   Arraylist of user objects
    /// </returns>
    public static ArrayList getAllUsers()
    {
      CTVIATBankDataSet.UsersDataTable allUsersTable = usersAdapter.GetAllUsers();
      ArrayList users = new ArrayList();
      foreach (DataRow row in allUsersTable)
      {
        if (row["userType"].ToString() == "1")
        {
          Admin temp = new Admin(Convert.ToInt32(row["userID"]), 
            Convert.ToString(row["userName"]), Convert.ToString(row["userPassword"]));
          users.Add(temp);
        }
        else
        {
          Candidate temp = new Candidate(Convert.ToInt32(row["userID"]), 
            Convert.ToString(row["userName"]), Convert.ToString(row["userPassword"]));
          users.Add(temp);
        }
      }
      return users;
    }

    /// <summary>
    ///   Username is unique, user object is created by querying username
    /// </summary>
    /// <param name="userName">username of user</param>
    /// <returns>
    ///   User object
    /// </returns>
    public static User getUserByUserName(String userName) 
    {
      CTVIATBankDataSet.UsersDataTable userTable = usersAdapter.GetUserByUserName(userName);
      if(userTable.Count == 0) 
      {
        return null;
      }
      else 
      {
        if (userTable[0]["userType"].ToString() == "1")
        {
          return new Admin(Convert.ToInt32(userTable[0]["userID"]), 
            Convert.ToString(userTable[0]["userName"]), Convert.ToString(userTable[0]["userPassword"]));
        }
        else 
        {
          return new Candidate(Convert.ToInt32(userTable[0]["userID"]), 
            Convert.ToString(userTable[0]["userName"]), Convert.ToString(userTable[0]["userPassword"]));
        }
      }
    }

    /// <summary>
    ///   Used to get information of all candidates
    /// </summary>
    /// <returns>
    ///   Arraylist of candidate objects
    /// </returns>
    public static ArrayList getAllCandidates(String pattern) 
    {
      CTVIATBankDataSet.UsersDataTable table = usersAdapter.GetCandidates("%" + pattern + "%");
      ArrayList users = new ArrayList();
      foreach (DataRow row in table) 
      {
        users.Add(new Candidate(Convert.ToInt32(row["userID"]), 
          Convert.ToString(row["userName"]), Convert.ToString(row["userPassword"])));
        }
      return users;
    }
    #endregion

    #region Test Methods

    /// <summary>
    ///   to add test 
    /// </summary>
    /// <param name="test">test object to be added</param>
    public static void addTest(Test test)
    {
      testsAdapter.InsertTest(test.Subject);
      CTVIATBankDataSet.TestsDataTable table = testsAdapter.GetAllTests();
      int testID = Convert.ToInt32(table.Rows[table.Count - 1]["testID"]);
      foreach (Question question in test.Questions)
      {
        questionsTestsAdapter.InsertQuestionOfTest(question.QuestionID, testID);
      }
    }

    /// <summary>
    ///   filter tests by subject
    /// </summary>
    /// <param name="candidateID">to set take orders for each test</param>
    /// <param name="subject">subject for filtering</param>
    /// <returns>
    ///   arraylist of tests
    /// </returns>
    public static ArrayList getAllTestsBySubject(int candidateID, int subject)
    {
      CTVIATBankDataSet.TestsDataTable testTable = 
        testsAdapter.GetAllTestsBySubject(subject);
      ArrayList tests = new ArrayList();
      foreach (DataRow row in testTable.Rows)
      {      
        int testId = Convert.ToByte(row["testID"]);
        CTVIATBankDataSet.QuestionsTestsDataTable questionsTable = 
          questionsTestsAdapter.GetQuestionsOfTest(testId);

        Queue<Question> queue = new Queue<Question>();
        foreach (DataRow r in questionsTable.Rows)
        {
          queue.Enqueue(getQuestionById(Convert.ToInt32(r["questionID"])));
        }
                
        CTVIATBankDataSet.TestsTakeOrderDataTable table = 
          testsTakeOrderAdapter.GetTakeOrder(candidateID, testId);
            
        int takeOrder = 0;
        if (table.Rows.Count > 0)
        {
          takeOrder = Convert.ToInt32(table.Rows[0]["takeOrder"]);
        }

        tests.Add(new Test(testId, queue.ToArray(), subject, 
          takeOrder, 0, true, 0, DateTime.Now));
      }
      return tests;
    }

    /// <summary>
    ///   filter incomplete tests by subject
    /// </summary>
    /// <param name="candidateID">to get incomplete tests and to set take orders for each test</param>
    /// <param name="subject">subject for filtering</param>
    /// <returns>
    ///   arraylist of tests
    /// </returns>
    public static ArrayList getAllIncompleteTestsBySubject(int candidateID, int subject)
    {
      CTVIATBankDataSet.IncompleteTestsCandidatesDataTable table = 
        new IncompleteTestsCandidatesTableAdapter().GetAllIncompleteTests(candidateID);
      ArrayList tests = new ArrayList();
      foreach (DataRow row in table.Rows)
      {
        int testID = Convert.ToInt32(row["testID"]);
        Test test = getIncompleteTestById(candidateID, testID);
        if (test.Subject == subject)
        {
          test.StartQuestionNo = Convert.ToInt32(row["questionID"]);
          test.TakeOrder = Convert.ToInt32(row["takeOrder"]);
          DataRow r = testsTimeAdapter.GetTime(candidateID, testID, test.TakeOrder).Rows[0];
          test.Time = Convert.ToInt32(r["time"]);
          test.Date = Convert.ToDateTime(r["date"]);
          getAnswersOfTest(candidateID, test);
          tests.Add(test);
        }
      }
      return tests;
    }

    /// <summary>
    ///   used to get all tests that are solved by candidate (complete + incomplete)
    /// </summary>
    /// <param name="candidateID">for which candidate</param>
    /// <returns>
    ///   arraylist of tests
    /// </returns>
    public static ArrayList getAllTestsOfCandidate(int candidateID)
    {
      CTVIATBankDataSet.TestsTakeOrderDataTable table = testsTakeOrderAdapter
        .GetAllTestAndTakeOrderOfCandidate(candidateID);

      ArrayList tests = new ArrayList();
      foreach (DataRow row in table)
      {
        int testID = Convert.ToInt32(row["testID"]);
        int maxTakeOrder = Convert.ToInt32(row["takeOrder"]);
        int subject = Convert.ToInt32(testsAdapter.GetTestByID(testID)[0]["testSubject"]); 

        for (int i = 1; i <= maxTakeOrder; i++) 
        {
          CTVIATBankDataSet.IncompleteTestsCandidatesDataTable incompleteTable = 
            incompleteTestsAdapter.GetStartQuestionNo(candidateID, testID, i); 
                
          bool isNew = true;
          int startQuestionNo = 0;
          if (incompleteTable.Rows.Count > 0)
          {
            isNew = false;
            startQuestionNo = Convert.ToInt32(incompleteTable.Rows[0]["questionID"]);
          }

          int time = Convert.ToInt32(testsTimeAdapter
            .GetTime(candidateID, testID, i)[0]["time"]);
          DateTime date = Convert.ToDateTime(testsTimeAdapter
            .GetTime(candidateID, testID, i)[0]["date"]);
           
          Test test = new Test(testID, getQuestionsOfTest(testID), subject, 
            i, time, isNew, startQuestionNo, date);

          getAnswersOfTest(candidateID, test);
          tests.Add(test);
        }
      }
      return tests;
    }

    /// <summary>
    ///   to get questions of test
    /// </summary>
    /// <param name="testID">for which test</param>
    /// <returns>
    ///   array of questions that are included in the test
    /// </returns>
    public static Question[] getQuestionsOfTest(int testID) 
    {
      Queue<Question> questions = new Queue<Question>();
      CTVIATBankDataSet.QuestionsTestsDataTable table = 
        questionsTestsAdapter.GetQuestionsOfTest(testID);

      foreach (DataRow row in table) 
      {
        questions.Enqueue(getQuestionById(Convert.ToInt32(row["questionID"])));
      }
      return questions.ToArray();
    }

    /// <summary>
    ///   to get incomplete test of candidate according to id since id is unique
    /// </summary>
    /// <param name="candidateID">which candidate</param>
    /// <param name="testId">which test</param>
    /// <returns>
    ///   test object with specified test ID
    /// </returns>
    public static Test getIncompleteTestById(int candidateID, int testId)
    {
      CTVIATBankDataSet.TestsDataTable testTable = testsAdapter.GetTestByID(testId);
      byte subject = Convert.ToByte(testTable.Rows[0]["testSubject"]);
      CTVIATBankDataSet.QuestionsTestsDataTable questionsTable = 
        questionsTestsAdapter.GetQuestionsOfTest(testId);

      Queue<Question> queue = new Queue<Question>();

      foreach (DataRow row in questionsTable.Rows)
      {
        queue.Enqueue(getQuestionById(Convert.ToInt32(row["questionID"])));
      }
      return new Test(testId, queue.ToArray(), subject, 0, 0, false, 0, DateTime.Now);
    }

    /// <summary>
    ///   to insert fully completed test
    /// </summary>
    /// <param name="candidateID">who has completed</param>
    /// <param name="test">which test is completed</param>
    public static void insertCompletedTest(int candidateID, Test test) 
    {
      if (test.IsNew)
      {
        foreach (Question q in test.Questions)
        {
          if (q is ObjectiveQuestion)
          {
            if (((ObjectiveQuestion)q).Submitted != null)
            {
              testsCandidatesAdapter.InsertSubmittedAnswer(candidateID, test.TestID,
                q.QuestionID, 1, ((ObjectiveQuestion)q).Submitted.Answer, null,
                test.TakeOrder, null);
            }
            else
            {
              testsCandidatesAdapter.InsertSubmittedAnswer(candidateID, test.TestID,
                q.QuestionID, 1, null, null, test.TakeOrder, null);
            }
          }
          else if (q is DescriptiveQuestion)
          {
            if (((DescriptiveQuestion)q).Submitted != null)
            {
              testsCandidatesAdapter.InsertSubmittedAnswer(candidateID, test.TestID,
                q.QuestionID, 2, null, ((DescriptiveQuestion)q).Submitted.Answer,
                test.TakeOrder, null);
            }
            else
            {
              testsCandidatesAdapter.InsertSubmittedAnswer(candidateID, test.TestID,
                q.QuestionID, 2, null, null, test.TakeOrder, null);
            }
          }
          else
          {
            if (((ActionBasedQuestion)q).Submitted != null)
            {
              testsCandidatesAdapter.InsertSubmittedAnswer(candidateID, test.TestID,
                q.QuestionID, 3, null, null, test.TakeOrder, 
                ((ActionBasedQuestion)q).Submitted.Answer);
            }
            else 
            {
              testsCandidatesAdapter.InsertSubmittedAnswer(candidateID, test.TestID,
                q.QuestionID, 3, null, null, test.TakeOrder, null);
            }
          }
        }        
        insertTestTime(candidateID, test);
      }
      else 
      {
        foreach (Question q in test.Questions) 
        {
          if(q is ObjectiveQuestion)
          {
            if (((ObjectiveQuestion)q).Submitted != null)
            {
              testsCandidatesAdapter.UpdateSubmittedAnswer(((ObjectiveQuestion)q).Submitted.Answer, 
                null, null, candidateID, test.TestID, q.QuestionID, test.TakeOrder);                       
            }
            else
            {
              testsCandidatesAdapter.UpdateSubmittedAnswer(null, null, null, 
                candidateID, test.TestID, q.QuestionID, test.TakeOrder);
            }
          }
          else if (q is DescriptiveQuestion)
          {
            if (((DescriptiveQuestion)q).Submitted != null)
            {
              testsCandidatesAdapter.UpdateSubmittedAnswer(null,
                ((DescriptiveQuestion)q).Submitted.Answer, null, candidateID, 
                test.TestID, q.QuestionID, test.TakeOrder);
            }
            else
            {
              testsCandidatesAdapter.UpdateSubmittedAnswer(null, null, null, 
                candidateID, test.TestID, q.QuestionID, test.TakeOrder);
            }
          }
          else
          {
            if (((ActionBasedQuestion)q).Submitted != null)
            {
              testsCandidatesAdapter.UpdateSubmittedAnswer(null, null, 
                ((ActionBasedQuestion)q).Submitted.Answer, candidateID, 
                test.TestID, q.QuestionID, test.TakeOrder);
            }
            else
            {
              testsCandidatesAdapter.UpdateSubmittedAnswer(null, null, null, 
                candidateID, test.TestID, q.QuestionID, test.TakeOrder);
            }    
          }        
        }
        deleteIncompleteTest(candidateID, test);
        updateTestTime(candidateID, test);              
      }
    }

    /// <summary>
    ///   to insert partially completed test
    /// </summary>
    /// <param name="candidateID">who has completed</param>
    /// <param name="test">which test is completed</param>
    /// <param name="questionID">from which question the test will continue</param>
    public static void insertIncompletedTest(int candidateID, Test test, int questionID)
    {
      CTVIATBankDataSet.TestsTimeDataTable table = testsTimeAdapter
        .GetTime(candidateID, test.TestID, test.TakeOrder);

      if (table.Rows.Count > 0)
      {
        testsTimeAdapter.UpdateTestTime(test.Time, test.Date, candidateID, 
          test.TestID, test.TakeOrder);
        incompleteTestsAdapter.UpdateStartQuestionID(questionID, candidateID, 
          test.TestID, test.TakeOrder);
        foreach (Question q in test.Questions)
        {
          if (q is ObjectiveQuestion)
          {
            if (((ObjectiveQuestion)q).Submitted != null)
            {
              testsCandidatesAdapter.UpdateSubmittedAnswer(((ObjectiveQuestion)q)
                .Submitted.Answer, null, null, candidateID, test.TestID, 
                q.QuestionID, test.TakeOrder);
            }
            else
            {
              testsCandidatesAdapter.UpdateSubmittedAnswer(null, null, null, 
                candidateID, test.TestID, q.QuestionID, test.TakeOrder);
            }
          }
          else if (q is DescriptiveQuestion)
          {
            if (((DescriptiveQuestion)q).Submitted != null)
            {
              testsCandidatesAdapter.UpdateSubmittedAnswer(null, 
                ((DescriptiveQuestion)q).Submitted.Answer, null, candidateID, 
                test.TestID, q.QuestionID, test.TakeOrder);
            }
            else
            {
              testsCandidatesAdapter.UpdateSubmittedAnswer(null, null, null, 
                candidateID, test.TestID, q.QuestionID, test.TakeOrder);
            }
          }
          else
          {
            if (((ActionBasedQuestion)q).Submitted != null)
            {
              testsCandidatesAdapter.UpdateSubmittedAnswer(null, null, 
                ((ActionBasedQuestion)q).Submitted.Answer, candidateID, 
                test.TestID, q.QuestionID, test.TakeOrder);
            }
            else
            {
              testsCandidatesAdapter.UpdateSubmittedAnswer(null, null, null, 
                candidateID, test.TestID, q.QuestionID, test.TakeOrder);
            }                        
          }
        }
      }
      else
      {
        insertTestTime(candidateID, test);
        incompleteTestsAdapter.InsertIncompleteTest(candidateID, test.TestID, test.TakeOrder, questionID);
        foreach (Question q in test.Questions)
        {
          if (q is ObjectiveQuestion)
          {
            if (((ObjectiveQuestion)q).Submitted != null)
            {
              testsCandidatesAdapter.InsertSubmittedAnswer(candidateID, 
                test.TestID, q.QuestionID, 1, ((ObjectiveQuestion)q).Submitted.Answer, 
                null, test.TakeOrder, null);
            }
            else
            {
              testsCandidatesAdapter.InsertSubmittedAnswer(candidateID, 
                test.TestID, q.QuestionID, 1, null, null, test.TakeOrder, null);
            }
          }
          else if (q is DescriptiveQuestion)
          {
            if (((DescriptiveQuestion)q).Submitted != null)
            {
              testsCandidatesAdapter.InsertSubmittedAnswer(candidateID, 
                test.TestID, q.QuestionID, 2, null, 
                ((DescriptiveQuestion)q).Submitted.Answer, test.TakeOrder, null);
            }
            else
            {
              testsCandidatesAdapter.InsertSubmittedAnswer(candidateID, 
                test.TestID, q.QuestionID, 2, null, null, test.TakeOrder, null);
            }
          }
          else
          {
            if (((ActionBasedQuestion)q).Submitted != null)
            {
              testsCandidatesAdapter.InsertSubmittedAnswer(candidateID, 
                test.TestID, q.QuestionID, 3, null, null, test.TakeOrder,
                ((ActionBasedQuestion)q).Submitted.Answer);
            }
            else
            {
              testsCandidatesAdapter.InsertSubmittedAnswer(candidateID, 
                test.TestID, q.QuestionID, 3, null, null, test.TakeOrder, null);
            }
          }
        }
      }
    }

    /// <summary>
    ///   delete incomplete test since it is completely finished
    /// </summary>
    /// <param name="candidateID">which candidate has completed</param>
    /// <param name="test">which test is completed</param>
    public static void deleteIncompleteTest(int candidateID, Test test)
    {
      incompleteTestsAdapter.DeleteIncompleteTest(candidateID, 
        test.TestID, test.TakeOrder);
    }

    /// <summary>
    ///   update test taken time
    /// </summary>
    /// <param name="candidateID">which candidate</param>
    /// <param name="test">which test</param>
    public static void updateTestTime(int candidateID, Test test) 
    {
      testsTimeAdapter.UpdateTestTime(test.Time, test.Date, candidateID, 
        test.TestID, test.TakeOrder);
    }

    /// <summary>
    ///   insert test taken time
    /// </summary>
    /// <param name="candidateID">which candidate</param>
    /// <param name="test">which test</param>
    public static void insertTestTime(int candidateID, Test test) 
    {
      testsTimeAdapter.InsertTestTime(candidateID, test.TestID, test.TakeOrder, 
        test.Time, test.Date);
    }

    /// <summary>
    ///   to get submitted answer of test
    /// </summary>
    /// <param name="candidateID">for which candidate</param>
    /// <param name="test">of which test</param>
    public static void getAnswersOfTest(int candidateID, Test test) 
    {
      foreach (Question q in test.Questions) 
      {
        CTVIATBankDataSet.TestsCandidatesDataTable table = 
          testsCandidatesAdapter.GetAnswer(candidateID, test.TestID, 
            q.QuestionID, test.TakeOrder);

        byte type = Convert.ToByte(table.Rows[0]["answerType"]);
        if(type == 1)
        {
          if (table.Rows[0]["objectAnswer"].ToString() == "")
          {
            ((ObjectiveQuestion)q).Submitted = null;
          }
          else 
          {
            ((ObjectiveQuestion)q).Submitted = 
            new ObjectiveAnswer(Convert.ToByte(table.Rows[0]["objectAnswer"]));
          }                    
        }
        else if (type == 2)
        {
          if (table.Rows[0]["descriptiveAnswer"].ToString() == "")
          {
            ((DescriptiveQuestion)q).Submitted = null;
          }
          else
          {
            ((DescriptiveQuestion)q).Submitted = 
            new DescriptiveAnswer(Convert.ToString(table.Rows[0]["descriptiveAnswer"]));
          }
        }
        else 
        {
          if (table.Rows[0]["actionAnswer"].ToString() == "")
          {
            ((ActionBasedQuestion)q).Submitted = null;
          }
          else
          {
            ((ActionBasedQuestion)q).Submitted = 
            new ActionBasedAnswer(Convert.ToByte(table.Rows[0]["actionAnswer"]));
          }
        }
      }
    }

    /// <summary>
    ///   to insert test take order since this is the first time this test is taken by the candidate 
    /// </summary>
    /// <param name="candidateID">which candidate</param>
    /// <param name="testID">which test</param>
    public static void insertTestTakeOrder(int candidateID, int testID) 
    {
      testsTakeOrderAdapter.InsertTakeOrder(candidateID, testID, 1);
    }

    /// <summary>
    /// to update test take order since candidate has taken this test before
    /// </summary>
    /// <param name="candidateID">which candidate</param>
    /// <param name="test">which test</param>
    public static void updateTestTakeOrder(int candidateID, Test test) 
    {
      testsTakeOrderAdapter.DeleteTakeOrder(candidateID, test.TestID);
      testsTakeOrderAdapter.InsertTakeOrder(candidateID, test.TestID, test.TakeOrder);
    }

    #endregion

    #region Question Methods

    /// <summary>
    ///  To get all questions
    /// </summary>
    /// <returns>
    ///     arraylist of question object
    /// </returns>
    public static ArrayList getAllQuestions()
    {
      CTVIATBankDataSet.QuestionsDataTable allQuestionsTable = questionsAdapter.GetAllQuestions();
      ArrayList questions = new ArrayList();
      foreach (DataRow row in allQuestionsTable)
      {
        if (row["questionType"].ToString() == "1")
        {
          String[] choices = new String[4];
          choices[0] = Convert.ToString(row["objectiveChoiceOne"]);
          choices[1] = Convert.ToString(row["objectiveChoiceTwo"]);
          choices[2] = Convert.ToString(row["objectiveChoiceThree"]);
          choices[3] = Convert.ToString(row["objectiveChoiceFour"]);
          ObjectiveQuestion temp = 
          new ObjectiveQuestion(Convert.ToString(row["question"]), choices,
            new ObjectiveAnswer(Convert.ToByte(row["objectiveAnswer"])),
            Convert.ToInt32(row["questionID"]), 
            Convert.ToInt32(row["questionSubject"])); 
          questions.Add(temp);
        }
        else if (row["questionType"].ToString() == "2")
        {
          DescriptiveQuestion temp = 
          new DescriptiveQuestion(Convert.ToString(row["question"]),
            new DescriptiveAnswer(Convert.ToString(row["descriptiveAnswer"])),
            Convert.ToInt32(row["questionID"]), 
            Convert.ToInt32(row["questionSubject"]));
          questions.Add(temp);
        }
        else 
        {
          ActionBasedQuestion temp = 
          new ActionBasedQuestion(Convert.ToString(row["question"]),
            Convert.ToInt32(row["actionType"]),
            Convert.ToString(row["actionParameterOne"]),
            Convert.ToString(row["actionParameterTwo"]),
            Convert.ToInt32(row["questionID"]),
            Convert.ToInt32(row["questionSubject"]));
          questions.Add(temp);    
          }
        }
      return questions;
    }

    /// <summary>
    ///   To get all questions by filtering with subject
    /// </summary>
    /// <param name="subject">test subject</param>
    /// <returns>
    ///   arraylist of questions with specified subjects
    /// </returns>
    public static ArrayList getAllQuestionsBySubject(int subject) 
    {
      CTVIATBankDataSet.QuestionsDataTable allQuestionsTable = questionsAdapter.GetAllQuestionsBySubject(subject);
      ArrayList questions = new ArrayList();
      foreach (DataRow row in allQuestionsTable)
      {
        if (row["questionType"].ToString() == "1")
        {
          String[] choices = new String[4];
          choices[0] = Convert.ToString(row["objectiveChoiceOne"]);
          choices[1] = Convert.ToString(row["objectiveChoiceTwo"]);
          choices[2] = Convert.ToString(row["objectiveChoiceThree"]);
          choices[3] = Convert.ToString(row["objectiveChoiceFour"]);
          ObjectiveQuestion temp = 
          new ObjectiveQuestion(Convert.ToString(row["question"]), choices, 
            new ObjectiveAnswer(Convert.ToByte(row["objectiveAnswer"])), 
            Convert.ToInt32(row["questionID"]),
            Convert.ToInt32(row["questionSubject"]));
          questions.Add(temp);
        }
        else if (row["questionType"].ToString() == "2")
        {
          DescriptiveQuestion temp = 
          new DescriptiveQuestion(Convert.ToString(row["question"]), 
            new DescriptiveAnswer(Convert.ToString(row["descriptiveAnswer"])),
            Convert.ToInt32(row["questionID"]),
            Convert.ToInt32(row["questionSubject"]));
          questions.Add(temp);
        }
        else
        {
          ActionBasedQuestion temp = 
          new ActionBasedQuestion(Convert.ToString(row["question"]), 
            Convert.ToInt32(row["actionType"]), 
            Convert.ToString(row["actionParameterOne"]),
            Convert.ToString(row["actionParameterTwo"]),
            Convert.ToInt32(row["questionID"]),
            Convert.ToInt32(row["questionSubject"]));
          questions.Add(temp);
        }
      }
      return questions;    
    }

    /// <summary>
    ///   To get all questions by filtering with type (objective, descriptive or action-based)
    /// </summary>
    /// <param name="type">question type</param>
    /// <returns>
    ///   arraylist of questions with specified type
    /// </returns>
    public static ArrayList getAllQuestionsByType(byte type) 
    {
      CTVIATBankDataSet.QuestionsDataTable allQuestionsTable = questionsAdapter.GetAllQuestionsByType(type);
      ArrayList questions = new ArrayList();
      if (type == 1)
      {
        foreach (DataRow row in allQuestionsTable)
        {
          String[] choices = new String[4];
          choices[0] = Convert.ToString(row["objectiveChoiceOne"]);
          choices[1] = Convert.ToString(row["objectiveChoiceTwo"]);
          choices[2] = Convert.ToString(row["objectiveChoiceThree"]);
          choices[3] = Convert.ToString(row["objectiveChoiceFour"]);
          ObjectiveQuestion temp = 
          new ObjectiveQuestion(Convert.ToString(row["question"]), choices,
            new ObjectiveAnswer(Convert.ToByte(row["objectiveAnswer"])),
            Convert.ToInt32(row["questionID"]),
            Convert.ToInt32(row["questionSubject"]));
          questions.Add(temp);
        }
      }
      else if (type == 2)
      {
        foreach (DataRow row in allQuestionsTable)
        {
          DescriptiveQuestion temp = 
          new DescriptiveQuestion(Convert.ToString(row["question"]),
            new DescriptiveAnswer(Convert.ToString(row["descriptiveAnswer"])),
            Convert.ToInt32(row["questionID"]),
            Convert.ToInt32(row["questionSubject"]));
          questions.Add(temp);
        }
      }
      else
      {
        foreach (DataRow row in allQuestionsTable)
        {
          ActionBasedQuestion temp = 
          new ActionBasedQuestion(Convert.ToString(row["question"]),
            Convert.ToInt32(row["actionType"]),
            Convert.ToString(row["actionParameterOne"]),
            Convert.ToString(row["actionParameterTwo"]),
            Convert.ToInt32(row["questionID"]),
            Convert.ToInt32(row["questionSubject"]));
          questions.Add(temp);
        }
      }
      return questions;
    }

    /// <summary>
    ///   To get all questions by filtering with subject and type
    /// </summary>
    /// <param name="subject">test subject</param>
    /// <param name="type">test type</param>
    /// <returns>
    ///   arraylist of questions with specified subject and type
    /// </returns>
    public static ArrayList getAllQuestionsBySubjectAndType(int subject, byte type) 
    {
      CTVIATBankDataSet.QuestionsDataTable allQuestionsTable = 
      questionsAdapter.GetAllQuestionBySubjectAndType(subject, type);
      ArrayList questions = new ArrayList();
      foreach (DataRow row in allQuestionsTable)
      {
        if (row["questionType"].ToString() == "1")
        {
          String[] choices = new String[4];
          choices[0] = Convert.ToString(row["objectiveChoiceOne"]);
          choices[1] = Convert.ToString(row["objectiveChoiceTwo"]);
          choices[2] = Convert.ToString(row["objectiveChoiceThree"]);
          choices[3] = Convert.ToString(row["objectiveChoiceFour"]);
          ObjectiveQuestion temp = 
          new ObjectiveQuestion(Convert.ToString(row["question"]), choices,
            new ObjectiveAnswer(Convert.ToByte(row["objectiveAnswer"])),
            Convert.ToInt32(row["questionID"]),
            Convert.ToInt32(row["questionSubject"]));
          questions.Add(temp);
        }
        else if (row["questionType"].ToString() == "2")
        {
          DescriptiveQuestion temp = 
          new DescriptiveQuestion(Convert.ToString(row["question"]),
            new DescriptiveAnswer(Convert.ToString(row["descriptiveAnswer"])),
            Convert.ToInt32(row["questionID"]), 
            Convert.ToInt32(row["questionSubject"]));
          questions.Add(temp);
        }
        else
        {
          ActionBasedQuestion temp = 
          new ActionBasedQuestion(Convert.ToString(row["question"]),
            Convert.ToInt32(row["actionType"]),
            Convert.ToString(row["actionParameterOne"]),
            Convert.ToString(row["actionParameterTwo"]),
            Convert.ToInt32(row["questionID"]),
            Convert.ToInt32(row["questionSubject"]));
          questions.Add(temp);
        }
      }
      return questions;
    }

    /// <summary>
    ///  question ID is unique, so question object is created by querying question ID
    /// </summary>
    /// <param name="questionId">question ID</param>
    /// <returns>
    ///     question object, can be cast to Objective, Descriptive or Action-based
    /// </returns>
    public static Question getQuestionById(int questionId)
    {
      CTVIATBankDataSet.QuestionsDataTable table = 
      questionsAdapter.GetQuestionByID(questionId);

      DataRow row = table.Rows[0];
      byte type = Convert.ToByte(row["questionType"]);
      if (type == 1)
      {
        String[] choices = new String[4];
        choices[0] = Convert.ToString(row["objectiveChoiceOne"]);
        choices[1] = Convert.ToString(row["objectiveChoiceTwo"]);
        choices[2] = Convert.ToString(row["objectiveChoiceThree"]);
        choices[3] = Convert.ToString(row["objectiveChoiceFour"]);

        return new ObjectiveQuestion(Convert.ToString(row["question"]), choices, 
          new ObjectiveAnswer(Convert.ToByte(row["objectiveAnswer"])), 
          Convert.ToInt32(row["questionID"]), 
          Convert.ToInt32(row["questionSubject"]));
      }
      else if (type == 2)
      {
        return new DescriptiveQuestion(Convert.ToString(row["question"]), 
          new DescriptiveAnswer(Convert.ToString(row["descriptiveAnswer"])), 
          Convert.ToInt32(row["questionID"]), 
          Convert.ToInt32(row["questionSubject"]));
      }
      else
      {
        return new ActionBasedQuestion(Convert.ToString(row["question"]),
          Convert.ToInt32(row["actionType"]),
          Convert.ToString(row["actionParameterOne"]),
          Convert.ToString(row["actionParameterTwo"]),
          Convert.ToInt32(row["questionID"]),
          Convert.ToInt32(row["questionSubject"]));
      }
    }

    /// <summary>
    ///   to add objective question
    /// </summary>
    /// <param name="question">objective question object to be added</param>
    public static void addObjectiveQuestion(ObjectiveQuestion question)
    {           
      questionsAdapter.InsertQuestion(1, question.QuestionText, 
        question.Choices[0], question.Choices[1], question.Choices[2], 
        question.Choices[3], question.Answer.Answer, null, 
        question.QuestionSubject, null, null, null);
    }

    /// <summary>
    ///  to add descriptive question
    /// </summary>
    /// <param name="question">descriptive question object to be added</param>
    public static void addDescriptiveQuestion(DescriptiveQuestion question)
    {
      questionsAdapter.InsertQuestion(2, question.QuestionText, null, null, 
        null, null, null, question.Answer.Answer, question.QuestionSubject,
        null, null, null);
    }

    /// <summary>
    ///  to add actipn-based question
    /// </summary>
    /// <param name="question">action-based question object to be added</param>
    public static void addActionBasedQuestion(ActionBasedQuestion question)
    {
      questionsAdapter.InsertQuestion(3, question.QuestionText, null, null, 
        null, null, null, null, question.QuestionSubject, question.Type, 
        question.Parameter1, question.Parameter2);
    }

    #endregion
  }
}
