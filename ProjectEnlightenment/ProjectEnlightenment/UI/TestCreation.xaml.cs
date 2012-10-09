using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Common.user;
using Common.question;
using Common.test;

using DAO.managers;

namespace ProjectEnlightenment.UI
{
  /// <summary>
  /// Interaction logic for TestCreation.xaml
  /// </summary>
  public partial class TestCreation : Window
  {
    private bool loadedFlag; //is window loaded
    private Admin admin; //admin which is using the window
    private ArrayList currentSelectedQuestions; //selected question list

    public TestCreation(Admin admin)
    {
      this.loadedFlag = false;
      InitializeComponent();
      setWindowProperties();
      this.admin = admin;
      this.questionSubjects.SelectedIndex = 0;
      this.questionTypes.SelectedIndex = 0;
      this.testSubject.SelectedIndex = 0;
      this.createTestButton.IsEnabled = false;
      this.currentSelectedQuestions = new ArrayList();
      this.questionTypes.Focus();
    }

    /// <summary>
    ///     sets window parameters
    ///     always on top and full screen
    /// </summary>
    private void setWindowProperties()
    {
      //this.Topmost = true;
      this.Width = SystemParameters.PrimaryScreenWidth;
      this.Height = SystemParameters.PrimaryScreenHeight;
      this.Top = 0;
      this.Left = 0;
    }

    /// <summary>
    ///     Window Events won't be explained since you can see their effects on the QUI
    /// </summary>
    #region Window Events
    private void questionTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      try
      {
        if (loadedFlag)
        {
          ArrayList questions = null;
          if (questionTypes.SelectedIndex != 0 && questionSubjects.SelectedIndex != 0)
          {
            questions = SQLManager.getAllQuestionsBySubjectAndType(questionSubjects
              .SelectedIndex, (byte)questionTypes.SelectedIndex);
          }
          else if (questionTypes.SelectedIndex == 0 && questionSubjects.SelectedIndex == 0)
          {
            //do nothing
          }
          else if (questionTypes.SelectedIndex == 0)
          {
            questions = SQLManager.getAllQuestionsBySubject(questionSubjects.SelectedIndex);
          }
          else
          {
            questions = SQLManager.getAllQuestionsByType((byte)questionTypes.SelectedIndex);
          }
          allQuestions.Items.Clear();
          if (questions != null)
          {
            foreach (Question q in questions)
            {
              allQuestions.Items.Add(q);
            }
          }
        }
      }
      catch (Exception ex) 
      {
        MessageBox.Show("Test fetching is failed\n" + ex.Message, 
          "Test Creation", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private void questionSubjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      try
      {
        if (loadedFlag)
        {
          ArrayList questions = null;
          if (questionTypes.SelectedIndex != 0 && questionSubjects.SelectedIndex != 0)
          {
            questions = SQLManager.getAllQuestionsBySubjectAndType(questionSubjects
              .SelectedIndex, (byte)questionTypes.SelectedIndex);
          }
          else if (questionTypes.SelectedIndex == 0 && questionSubjects.SelectedIndex == 0)
          {
            //do nothing
          }
          else if (questionTypes.SelectedIndex == 0)
          {
            questions = SQLManager.getAllQuestionsBySubject(questionSubjects.SelectedIndex);
          }
          else
          {
            questions = SQLManager.getAllQuestionsByType((byte)questionTypes.SelectedIndex);
          }

          allQuestions.Items.Clear();
          if (questions != null)
          {
            foreach (Question q in questions)
            {
                allQuestions.Items.Add(q);
            }
          }
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Test fetching is failed\n" + ex.Message, 
          "Test Creation", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private void backButton_Click(object sender, RoutedEventArgs e)
    {
      new AdminMain(admin).Show();
      this.Close();
    }

    private void createTestButton_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        Test test = new Test(-1, (Question[])currentSelectedQuestions
          .ToArray(typeof(Question)), (byte)testSubject.SelectedIndex, 0, 0, true, 0, DateTime.Now);
        SQLManager.addTest(test);
        questionTypes.SelectedIndex = 0;
        questionSubjects.SelectedIndex = 0;
        testSubject.SelectedIndex = 0;
        selectedQuestions.Items.Clear();
        currentSelectedQuestions.Clear();
        MessageBox.Show("Test Creation is successful\n", "Test Creation", 
          MessageBoxButton.OK, MessageBoxImage.Information);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Test creation is failed\n" + ex.Message, 
          "Test Creation", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private void addQuestionbutton_Click(object sender, RoutedEventArgs e)
    {
      foreach (Question q in allQuestions.SelectedItems)
      {
        if (!currentSelectedQuestions.Contains(q))
        {
          currentSelectedQuestions.Add(q);
          selectedQuestions.Items.Add(q);
        }
      }
      adjustCreateTestButton();
    }

    private void subtractQuestionButton_Click(object sender, RoutedEventArgs e)
    {
      ArrayList temp = new ArrayList();
      foreach (Question q in selectedQuestions.SelectedItems)
      {
        currentSelectedQuestions.RemoveAt(currentSelectedQuestions.IndexOf(q));
        temp.Add(q);
      }
      foreach (Question q in temp) 
      {
        selectedQuestions.Items.RemoveAt(selectedQuestions.Items.IndexOf(q));
      }
        adjustCreateTestButton();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      this.loadedFlag = true;
    }

    private void testSubject_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (loadedFlag) 
      {
        adjustCreateTestButton();
      }
    }

    private void adjustCreateTestButton()
    {
      if (currentSelectedQuestions.Count > 0 && testSubject.SelectedIndex > 0)
      {
        createTestButton.IsEnabled = true;
      }
      else
      {
        createTestButton.IsEnabled = false;
      }
    }
    #endregion Window Events
  }
}
