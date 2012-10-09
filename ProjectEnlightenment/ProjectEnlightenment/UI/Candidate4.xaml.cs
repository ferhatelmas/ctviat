using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Threading;
using System.ComponentModel;
using System.IO;

using Common.test;
using Common.user;
using Common.question;
using Common.answer;

using Automation;

using ProjectEnlightenment.Utilities;

using DAO.managers;

namespace ProjectEnlightenment.UI
{
  /// <summary>
  /// Interaction logic for Candidate4.xaml
  /// </summary>
  public partial class Candidate4 : Window
  {
    private Candidate candidate;  //the candidate which is using the window
    private TestSolver solver;  //solver logic is implemented in this object
    private TestProgressHandler timeHandler; //the caller for the function when timer is expired
    private Freshness freshness; //test freshness

    private void InitializeTestSolver(Test test)
    {
      solver = new TestSolver(test, new TestProgressHandler(updateTime));
      timeHandler += updateTime;
    }

    private void InitializeAnswerGrids() 
    {
      objectiveAnswerGrid.Visibility = Visibility.Hidden;
      descriptiveAnswerGrid.Visibility = Visibility.Hidden;
    }

    public Candidate4(Candidate candidate, Test test, Freshness freshness)
    {
      this.candidate = candidate;
      this.freshness = freshness;

      InitializeComponent();
      InitializeAnswerGrids();
      setWindowProperties();

      InitializeTestSolver(test);
    }

    /// <summary>
    ///   sets window parameters
    ///   always on top and full screen
    /// </summary>
    private void setWindowProperties()
    {
      //this.Topmost = true;
      this.Width = SystemParameters.PrimaryScreenWidth;
      this.Height = SystemParameters.PrimaryScreenHeight;
      this.Top = 0;
      this.Left = 0;
    }

    private void updateTime(object sender, TestEventArgs e)
    {
      if (Dispatcher.CheckAccess())
      {
        timeLabel.Content = e.Time;
      }
      else
      {
        Dispatcher.BeginInvoke(timeHandler, new Object[] { this, e });
      }
    }

    /// <summary>
    /// determines question type and shows the question
    /// </summary>
    private void showQuestion() 
    {
      Question question = solver.getCurrentQuestion();
      if (question is ObjectiveQuestion)
      {
        showObjectiveQuestion((ObjectiveQuestion)question);
      }
      else if (question is DescriptiveQuestion)
      {
        showDescriptiveQuestion((DescriptiveQuestion)question);
      }
      else
      {
        showActionBasedQuestion((ActionBasedQuestion)question);
        InitializeAnswerGrids();
      }
    }

    /// <summary>
    ///  show the objective question by setting window controls and their visibility
    /// </summary>
    /// <param name="q">question to be shown</param>
    private void showObjectiveQuestion(ObjectiveQuestion q)
    {
      questionLabel.Text = q.QuestionText;
      option0Text.Text = q.Choices[0];
      option1Text.Text = q.Choices[1];
      option2Text.Text = q.Choices[2];
      option3Text.Text = q.Choices[3];

      if (q.Submitted != null)
      {
        if (q.Submitted.Answer == 1)
        {
          option0.IsChecked = true;
        }
        else if (q.Submitted.Answer == 2)
        {
          option1.IsChecked = true;
        }
        else if (q.Submitted.Answer == 3)
        {
          option2.IsChecked = true;
        }
        else if (q.Submitted.Answer == 4)
        {
          option3.IsChecked = true;
        }
      }
      else
      {
        option0.IsChecked = false;
        option1.IsChecked = false;
        option2.IsChecked = false;
        option3.IsChecked = false;
      }

      objectiveAnswerGrid.Visibility = Visibility.Visible;
      descriptiveAnswerGrid.Visibility = Visibility.Hidden;

      Speech.speakPurge("Objective Question");
      Speech.speakNormal(q.QuestionText);

      Speech.speakNormal("option one");
      Speech.speakNormal(q.Choices[0]);
      Speech.speakNormal("option two");
      Speech.speakNormal(q.Choices[1]);
      Speech.speakNormal("option three");
      Speech.speakNormal(q.Choices[2]);
      Speech.speakNormal("option four");
      Speech.speakNormal(q.Choices[3]);
    }

    /// <summary>
    ///  show the descriptive question by setting window controls and their visibility
    /// </summary>
    /// <param name="q">question to be shown</param>
    private void showDescriptiveQuestion(DescriptiveQuestion q) 
    {
      questionLabel.Text = q.QuestionText;

      if (q.Submitted != null)
      {
        descriptionTextBox.Text = q.Submitted.Answer;
      }
      else
      {
        descriptionTextBox.Text = "";
      }

      objectiveAnswerGrid.Visibility = Visibility.Hidden;
      descriptiveAnswerGrid.Visibility = Visibility.Visible;
      descriptionTextBox.Focus();

      Speech.speakPurge("Descriptive Question");
      Speech.speakNormal(q.QuestionText);
    }

    /// <summary>
    ///  show the action-based question by setting window controls and their visibility
    /// </summary>
    /// <param name="q">question to be shown</param>
    private void showActionBasedQuestion(ActionBasedQuestion q) 
    {
      bool flag = false;
      questionLabel.Text = q.QuestionText;
      if (q.Submitted != null)
      {
        flag = true;
      }
      else 
      {
        prepareEnvironment(q);
      }

      objectiveAnswerGrid.Visibility = Visibility.Hidden;
      descriptiveAnswerGrid.Visibility = Visibility.Hidden;

      Speech.speakPurge("Action based question");
      if (flag)
      {
        Speech.speakNormal("You have already solved this question correctly");
      }
      else 
      {
        Speech.speakNormal(q.QuestionText);
      }
    }

    /// <summary>
    /// saves submitted answer of the question
    /// </summary>
    private void saveQuestion() 
    {
      Question question = solver.getCurrentQuestion();
      if (question is ObjectiveQuestion)
      {
        if ((bool)option0.IsChecked) 
        {
          solver.saveSubmittedObjectiveAnswer(new ObjectiveAnswer(1));
        }
        else if ((bool)option1.IsChecked) 
        {
          solver.saveSubmittedObjectiveAnswer(new ObjectiveAnswer(2));
        }
        else if ((bool)option2.IsChecked)
        {
          solver.saveSubmittedObjectiveAnswer(new ObjectiveAnswer(3));
        }
        else if ((bool)option3.IsChecked)
        {
          solver.saveSubmittedObjectiveAnswer(new ObjectiveAnswer(4));
        }
        else 
        {
          solver.saveSubmittedObjectiveAnswer(null);
        }    
      }
      else if (question is DescriptiveQuestion)
      {
        if (descriptionTextBox.Text == "")
        {
          solver.saveSubmittedDescriptiveAnswer(null);
        }
        else 
        {
          solver.saveSubmittedDescriptiveAnswer(new DescriptiveAnswer(descriptionTextBox.Text));
        }
      }
      else
      {
        ActionBasedQuestion q = (ActionBasedQuestion)question;

        if (q.Type == 1) 
        {
          if (isDirectoryExists(q.Parameter1))
          {
            solver.saveSubmittedActionBasedAnswer(new ActionBasedAnswer(1));
          }
          else 
          {
            solver.saveSubmittedActionBasedAnswer(null);
          } 
        }
        else if (q.Type == 2) 
        {
          if (isFileExists(q.Parameter1))
          {
            solver.saveSubmittedActionBasedAnswer(new ActionBasedAnswer(1));
          }
          else 
          {
            solver.saveSubmittedActionBasedAnswer(null);
          }
        }
        else if (q.Type == 3)
        {
          if (!isDirectoryExists(q.Parameter1))
          {
            solver.saveSubmittedActionBasedAnswer(new ActionBasedAnswer(1));
          }
          else 
          {
            solver.saveSubmittedActionBasedAnswer(null);
          }
        }
        else if (q.Type == 4)
        {
          if (!isFileExists(q.Parameter1))
          {
            solver.saveSubmittedActionBasedAnswer(new ActionBasedAnswer(1));
          }
          else
          {
            solver.saveSubmittedActionBasedAnswer(null);
          }
        }
        else if (q.Type == 5)
        {
          if (isDirectoryExists(q.Parameter2))
          {
            solver.saveSubmittedActionBasedAnswer(new ActionBasedAnswer(1));
          }
          else
          {
            solver.saveSubmittedActionBasedAnswer(null);
          }
        }
        else if (q.Type == 6)
        {
          if (isFileExists(q.Parameter2))
          {
            solver.saveSubmittedActionBasedAnswer(new ActionBasedAnswer(1));
          }
          else
          {
            solver.saveSubmittedActionBasedAnswer(null);
          }
        }
        else if (q.Type == 7)
        {
          if (checkText(q.Parameter1))
          {
            solver.saveSubmittedActionBasedAnswer(new ActionBasedAnswer(1));
          }
          else 
          {
            solver.saveSubmittedActionBasedAnswer(null);
          }
        }
        else if (q.Type == 8)
        {
          if (checkText(q.Parameter2))
          {
            solver.saveSubmittedActionBasedAnswer(new ActionBasedAnswer(1));
          }
          else 
          {
            solver.saveSubmittedActionBasedAnswer(null);
          }
        }

        clearEnvironment(q);
      }
    }

    #region Action Based Environment Functions
    /// <summary>
    /// Their names are self explanatory
    /// </summary>
    /// <param name="q"></param>
    private void prepareEnvironment(ActionBasedQuestion q) 
    {
      try
      {
        if (q.Type == 1)
        {
          deleteDirectory(q.Parameter1);
        }
        else if (q.Type == 2)
        {
          deleteFile(q.Parameter1);
        }
        else if (q.Type == 3)
        {
          createDirectory(q.Parameter1);
        }
        else if (q.Type == 4)
        {
          createFile(q.Parameter1);
        }
        else if (q.Type == 5)
        {
          createDirectory(q.Parameter1);
        }
        else if (q.Type == 6)
        {
          createFile(q.Parameter1);
        }
        else if (q.Type == 7)
        {
          createFile("enlightenment_test.txt");
        }
          else if (q.Type == 8)
          {
            createFile("enlightenment_test.txt");
            writeToFile("enlightenment_test.txt", q.Parameter1);
          }
        }
      catch 
      {
          Speech.speakPurgeSync("An error occurred. You will begin from start.");

          Dispatcher.BeginInvoke((ThreadStart)delegate
          {
              new Candidate1(candidate, Subject.None, Freshness.None, Screen.getEmptySeen()).Show();
          }, DispatcherPriority.Normal);

          Dispatcher.BeginInvoke((ThreadStart)delegate { this.Close(); },
              DispatcherPriority.Normal);
      }
    }

    private void clearEnvironment(ActionBasedQuestion q) 
    {
      try
      {
        if (q.Type == 1)
        {
          deleteDirectory(q.Parameter1);
        }
        else if (q.Type == 2)
        {
          deleteFile(q.Parameter1);
        }
        else if (q.Type == 3)
        {
          deleteDirectory(q.Parameter1);
        }
        else if (q.Type == 4)
        {
          deleteFile(q.Parameter1);
        }
        else if (q.Type == 5)
        {
          deleteDirectory(q.Parameter1);
          deleteDirectory(q.Parameter2);
        }
        else if (q.Type == 6)
        {
          deleteFile(q.Parameter1);
          deleteFile(q.Parameter2);
        }
        else if (q.Type == 7)
        {
          deleteFile("enlightenment_test.txt");
        }
        else if (q.Type == 8)
        {
          deleteFile("enlightenment_test.txt");
        }
      }
      catch 
      {           
        Speech.speakPurgeSync("An error occurred. You will begin from start.");

        Dispatcher.BeginInvoke((ThreadStart)delegate
        {
            new Candidate1(candidate, Subject.None, Freshness.None, Screen.getEmptySeen()).Show();
        }, DispatcherPriority.Normal);

        Dispatcher.BeginInvoke((ThreadStart)delegate { this.Close(); },
            DispatcherPriority.Normal);
    
      }
    }

    private void createDirectory(String dirName) 
    {
      String desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
      DirectoryInfo d = new DirectoryInfo(desktop);
      bool isExists = false;
      foreach (DirectoryInfo df in d.GetDirectories())
      {
        if (df.Name == dirName)
        {
          isExists = true;
          break;
        }
      }

      if (!isExists)
      {
        new DirectoryInfo(@Path.Combine(desktop, dirName)).Create();
      }
    }

    private void createFile(String fileName) 
    {
      String desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
      DirectoryInfo d = new DirectoryInfo(desktop);
      bool isExists = false;

      foreach (FileInfo f in d.GetFiles())
      {
        if (f.Name == fileName)
        {
          isExists = true;
          break;
        }
      }

      if (!isExists)
      {
        FileStream fs = new FileInfo(@Path.Combine(desktop, fileName)).Create();
        fs.Close();
      }
    }

    private bool isDirectoryExists(String dirName) 
    {
      String desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
      DirectoryInfo d = new DirectoryInfo(desktop);

      foreach (DirectoryInfo df in d.GetDirectories())
      {
        if (df.Name == dirName)
        {
          return true;
        }
      }
      return false;
    }

    private bool isFileExists(String fileName) 
    {
      String desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
      DirectoryInfo d = new DirectoryInfo(desktop);

      foreach (FileInfo f in d.GetFiles())
      {
        if (f.Name == fileName)
        {
          return true;
        }
      }
      return false;
    }

    private void deleteDirectory(String dirName) 
    {
      String desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
      DirectoryInfo d = new DirectoryInfo(desktop);

      foreach (DirectoryInfo df in d.GetDirectories())
      {
        if (df.Name == dirName)
        {
          df.Delete();
          break;
        }
      }
    }

    private void deleteFile(String fileName) 
    {
      String desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
      DirectoryInfo d = new DirectoryInfo(desktop);

      foreach (FileInfo f in d.GetFiles())
      {
        if (f.Name == fileName)
        {
          f.Delete();
          break;
        }
      }    
    }

    private void writeToFile(String fileName, String text) 
    {
      String desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

      StreamWriter sw = new StreamWriter(@Path.Combine(desktop, fileName));
      sw.Write(text);
      sw.Close();
    }

    private bool checkText(String text) 
    {
      String desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
      FileInfo fi = new FileInfo(@Path.Combine(desktop, "enlightenment_test.txt"));

      if (!fi.Exists)
      {
        return false;
      }
      else 
      {
        StreamReader sr = fi.OpenText();

        if (sr.ReadToEnd() == text)
        {
          sr.Close();
          return true;
        }
        else 
        {
          sr.Close();
          return false;
        }
      }
    }
    #endregion

    /// <summary>
    ///   Window Events won't be explained since you can see their effects on the QUI
    /// </summary>
    #region Window Events
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      Speech.speakPurge("Test is started");
      showQuestion();
      solver.startTimer();
    }

    private void Window_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.N && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        Speech.speakPurge(null);
        nextButton_Click(null, null);
      }
      else if (e.Key == Key.B && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        Speech.speakPurge(null);
        backButton_Click(null, null);
      }
      else if (e.Key == Key.T && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        Speech.speakPurge(null);
        speakTime();
      }
      else if (e.Key == Key.D && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        Speech.speakPurge(null);
        discontinueButton_Click(null, null);
      }
      else if (e.Key == Key.F && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        Speech.speakPurge(null);
        finishButton_Click(null, null);
      }
      else if (e.Key == Key.H && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        Speech.speakPurge("Press N to go next question");
        Speech.speakNormal("Press B to return previous question");
        Speech.speakNormal("Press T to learn how much time passed");
        Speech.speakNormal("Press D to leave and continue later");
        Speech.speakNormal("Press F to finish solving");
      }
      else if (e.Key == Key.R && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        if (solver.getCurrentQuestion() is ObjectiveQuestion)
        {
          ObjectiveQuestion q = (ObjectiveQuestion)solver.getCurrentQuestion();
            
          Speech.speakPurge("Objective Question");
          Speech.speakNormal(q.QuestionText);

          Speech.speakNormal("option one");
          Speech.speakNormal(q.Choices[0]);
          Speech.speakNormal("option two");
          Speech.speakNormal(q.Choices[1]);
          Speech.speakNormal("option three");
          Speech.speakNormal(q.Choices[2]);
          Speech.speakNormal("option four");
          Speech.speakNormal(q.Choices[3]);
        }
        else if (solver.getCurrentQuestion() is DescriptiveQuestion)
        {
          DescriptiveQuestion q = (DescriptiveQuestion)solver.getCurrentQuestion();

          Speech.speakPurge("Descriptive Question");
          Speech.speakNormal(q.QuestionText);
        }
        else 
        {
          ActionBasedQuestion q = (ActionBasedQuestion)solver.getCurrentQuestion();

          Speech.speakPurge("Action Based Question");
          Speech.speakNormal(q.QuestionText);
        }
      }
      else if (e.Key == Key.D1 || e.Key == Key.NumPad1)
      {
        Speech.speakPurge(null);
        option0.IsChecked = true;
      }
      else if (e.Key == Key.D2 || e.Key == Key.NumPad2)
      {
        Speech.speakPurge(null);
        option1.IsChecked = true;
      }
      else if (e.Key == Key.D3 || e.Key == Key.NumPad3)
      {
        Speech.speakPurge(null);
        option2.IsChecked = true;
      }
      else if (e.Key == Key.D4 || e.Key == Key.NumPad4)
      {
        Speech.speakPurge(null);
        option3.IsChecked = true;
      }
      else if (e.Key == Key.D5 || e.Key == Key.NumPad5) 
      {
        Speech.speakPurge(null);
        option0.IsChecked = false;
        option1.IsChecked = false;
        option2.IsChecked = false;
        option3.IsChecked = false;
      }
      else if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        Speech.changeVoice();
      }
    }

    private void option0_Checked(object sender, RoutedEventArgs e)
    {
      Speech.speakPurge(null);
      option1.IsChecked = false;
      option2.IsChecked = false;
      option3.IsChecked = false;
    }

    private void option1_Checked(object sender, RoutedEventArgs e)
    {
      Speech.speakPurge(null);
      option0.IsChecked = false;
      option2.IsChecked = false;
      option3.IsChecked = false;
    }

    private void option2_Checked(object sender, RoutedEventArgs e)
    {
      Speech.speakPurge(null);
      option0.IsChecked = false;
      option1.IsChecked = false;
      option3.IsChecked = false;
    }

    private void option3_Checked(object sender, RoutedEventArgs e)
    {
      Speech.speakPurge(null);           
      option0.IsChecked = false;
      option1.IsChecked = false;
      option2.IsChecked = false;
    }

    private void speakTime()
    {
      long time = solver.PassedTime;
      String message = null;

      if (time / 3600 > 0)
      {
        message = time / 3600 + " hours";
      }

      if (time % 3600 / 60 > 0)
      {
        message = message + time % 3600 / 60 + " minutes";
      }

      message = message + time % 60 + " seconds is passed";

      Speech.speakPurge(message);        
    }

    private void backButton_Click(object sender, RoutedEventArgs e)
    { 
      if (solver.isBack())
      {
        Speech.speakPurge(null);
        saveQuestion();
        solver.returnToPreviousQuestion();
        showQuestion();
      }
      else
      {
        Speech.speakPurge("This question is the first question");
      }
    }

    private void nextButton_Click(object sender, RoutedEventArgs e)
    {
      if (solver.isNext())
      {
        Speech.speakPurge(null);
        saveQuestion();
        solver.proceedToNextQuestion();
        showQuestion();
      }
      else
      {
        Speech.speakPurge("This question is the last question");
      }
    }

    private void discontinueButton_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        Speech.speakPurge(null);
        saveQuestion();
        solver.stopTimer();
        if (solver.Test.IsNew)
        {
          if (solver.Test.TakeOrder == 1)
          {
              SQLManager.insertTestTakeOrder(candidate.UserID, solver.Test.TestID);
          }
          else
          {
              SQLManager.updateTestTakeOrder(candidate.UserID, solver.Test);
          }
        }
        SQLManager.insertIncompletedTest(candidate.UserID, solver.Test, 
          solver.getCurrentQuestionNumber());

        Dispatcher.BeginInvoke((ThreadStart)delegate
        {
            new Candidate6(candidate).Show();
        }, DispatcherPriority.Normal);

        Dispatcher.BeginInvoke((ThreadStart)delegate { this.Close(); },
            DispatcherPriority.Normal);
      }
      catch 
      {
        Speech.speakPurgeSync("An error occurred. You will begin from start.");

        Dispatcher.BeginInvoke((ThreadStart)delegate
        {
            new Candidate1(candidate, Subject.None, Freshness.None, Screen.getEmptySeen()).Show();
        }, DispatcherPriority.Normal);

        Dispatcher.BeginInvoke((ThreadStart)delegate { this.Close(); },
            DispatcherPriority.Normal);
      }
    }

    private void finishButton_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        Speech.speakPurge(null);
        saveQuestion();
        solver.stopTimer();
        if (solver.Test.IsNew)
        {
          if (solver.Test.TakeOrder == 1)
          {
            SQLManager.insertTestTakeOrder(candidate.UserID, solver.Test.TestID);
          }
          else
          {
            SQLManager.updateTestTakeOrder(candidate.UserID, solver.Test);
          }
        }
        SQLManager.insertCompletedTest(candidate.UserID, solver.Test);

        Dispatcher.BeginInvoke((ThreadStart)delegate
        {
            new Candidate5(candidate, solver).Show();
        }, DispatcherPriority.Normal);

        Dispatcher.BeginInvoke((ThreadStart)delegate { this.Close(); },
            DispatcherPriority.Normal);
      } 
      catch 
      {
        Speech.speakPurgeSync("An error occurred. You will begin from start.");

        Dispatcher.BeginInvoke((ThreadStart)delegate
        {
            new Candidate1(candidate, Subject.None, Freshness.None, Screen.getEmptySeen()).Show();
        }, DispatcherPriority.Normal);

        Dispatcher.BeginInvoke((ThreadStart)delegate { this.Close(); },
            DispatcherPriority.Normal);
      }
    }
    #endregion
  }
}
