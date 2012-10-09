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
using System.Windows.Shapes;

using Common.user;
using Common.question;
using Common.answer;

using DAO.managers;

namespace ProjectEnlightenment.UI
{
  /// <summary>
  /// Interaction logic for QuestionCreation.xaml
  /// </summary>
  public partial class QuestionCreation : Window
  {
    private Admin admin;

    public QuestionCreation(Admin admin)
    {
      InitializeComponent();
      setWindowProperties();
      this.admin = admin;
      questionTypes.SelectedIndex = 0;
      questionSubjects.SelectedIndex = 0;
      createQuestionButton.IsEnabled = false;
      questionTypes.Focus();
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
    /// adjusts which question type grid must be enabled
    /// </summary>
    private void adjustVisibleQuestionGrid()
    {
      if (questionTypes.SelectedIndex == 0) 
      {
        objectiveGrid.IsEnabled = false;
        descriptiveGrid.IsEnabled = false;
        actionGrid.IsEnabled = false;

        resetObjectiveQuestion();
        resetDescriptiveQuestion();
        resetActionBasedQuestion();
      }
      else if (questionTypes.SelectedIndex == 1)
      {
        objectiveGrid.IsEnabled = true;
        descriptiveGrid.IsEnabled = false;
        actionGrid.IsEnabled = false;

        objectiveQuestion.Focus();

        resetDescriptiveQuestion();
        resetActionBasedQuestion();
      }
      else if (questionTypes.SelectedIndex == 2)
      {
        objectiveGrid.IsEnabled = false;
        descriptiveGrid.IsEnabled = true;
        actionGrid.IsEnabled = false;

        descriptiveQuestion.Focus();

        resetObjectiveQuestion();
        resetActionBasedQuestion();
      }
      else
      {
        objectiveGrid.IsEnabled = false;
        descriptiveGrid.IsEnabled = false;
        actionGrid.IsEnabled = true;

        questionFunctionComboBox.Focus();

        resetObjectiveQuestion();
        resetDescriptiveQuestion();
      }
    }

    /// <summary>
    /// resets functions delete all inputs that have entered before, clear GUI
    /// </summary>
    private void resetObjectiveQuestion() 
    {
      objectiveQuestion.Text = "";
      option1.Text = "";
      option2.Text = "";
      option3.Text = "";
      option4.Text = "";
      correctChoiceComboBox.SelectedIndex = 0;
    }

    private void resetDescriptiveQuestion() 
    {
      descriptiveQuestion.Text = "";
      descriptiveAnswer.Text = "";
    }

    private void resetActionBasedQuestion() 
    {
      questionFunctionComboBox.SelectedIndex = 0;
      actionName1TextBox.Text = "";
      actionName2TextBox.Text = "";
      actionText1TextBox.Text = "";
      actionText2TextBox.Text = "";
    }

    private void resetQuestionTypeSelection() 
    {
      correctChoiceComboBox.SelectedIndex = 0;
      questionSubjects.SelectedIndex = 0;
      questionTypes.SelectedIndex = 0;
      questionTypes.Focus();
    }

    /// <summary>
    ///     Window Events won't be explained since you can see their effects on the QUI
    /// </summary>
    #region Window Events
    private void questionTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      adjustVisibleQuestionGrid();
      if (questionTypes.SelectedIndex != 0 && questionSubjects.SelectedIndex != 0)
      {
        createQuestionButton.IsEnabled = true;
      }
      else 
      {
        createQuestionButton.IsEnabled = false;
      }
    }

    private void questionSubjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (questionTypes.SelectedIndex != 0 && questionSubjects.SelectedIndex != 0)
      {
        createQuestionButton.IsEnabled = true;
      }
      else 
      {
        createQuestionButton.IsEnabled = false;
      }
    }

    private void questionFunctionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      int selection = questionFunctionComboBox.SelectedIndex;

      if (selection < 1) 
      {
        actionName1Grid.IsEnabled = false;
        actionName2Grid.IsEnabled = false;
        actionText1Grid.IsEnabled = false;
        actionText2Grid.IsEnabled = false;
      }
      else if (selection == 1 || selection == 2 || selection == 3 || selection == 4) 
      {
        actionName1Grid.IsEnabled = true;
        actionName2Grid.IsEnabled = false;
        actionText1Grid.IsEnabled = false;
        actionText2Grid.IsEnabled = false;
      }
      else if (selection == 5 || selection == 6)
      {
        actionName1Grid.IsEnabled = true;
        actionName2Grid.IsEnabled = true;
        actionText1Grid.IsEnabled = false;
        actionText2Grid.IsEnabled = false;
      }
      else if (selection == 7)
      {
        actionName1Grid.IsEnabled = false;
        actionName2Grid.IsEnabled = false;
        actionText1Grid.IsEnabled = true;
        actionText2Grid.IsEnabled = false;
      }
      else if (selection == 8)
      {
        actionName1Grid.IsEnabled = false;
        actionName2Grid.IsEnabled = false;
        actionText1Grid.IsEnabled = true;
        actionText2Grid.IsEnabled = true;
      }
    }

    private void backButton_Click(object sender, RoutedEventArgs e)
    {
      new AdminMain(admin).Show();
      this.Close();
    }

    private void createQuestionButton_Click(object sender, RoutedEventArgs e)
    {
      if (questionTypes.SelectedIndex == 1) 
      {
        if (String.IsNullOrEmpty(objectiveQuestion.Text)) 
        {
          MessageBox.Show("Question Text cannot be empty\nPlease enter a question text", 
            "Question Creation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else if (String.IsNullOrEmpty(option1.Text) || 
                 String.IsNullOrEmpty(option2.Text) || 
                 String.IsNullOrEmpty(option3.Text) || 
                 String.IsNullOrEmpty(option4.Text)) 
        {
          MessageBox.Show("Question Option cannot be empty\nPlease enter options of question",
            "Question Creation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else if (correctChoiceComboBox.SelectedIndex == 0)
        {
          MessageBox.Show("Question Answer isn't selected\nPlease select correct choice",
           "Question Creation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else 
        {
          try
          {
            SQLManager.addObjectiveQuestion(new ObjectiveQuestion(objectiveQuestion.Text, 
              new String[4]{option1.Text, option2.Text, option3.Text, option4.Text},
              new ObjectiveAnswer((byte)correctChoiceComboBox.SelectedIndex), -1, 
              questionSubjects.SelectedIndex));
            resetObjectiveQuestion();
            resetQuestionTypeSelection();
            MessageBox.Show("Question creation is successful\n", "Question Creation", 
              MessageBoxButton.OK, MessageBoxImage.Information);
          }
          catch (Exception ex) 
          {
              MessageBox.Show("Question creation is failed\n" + ex.Message, "Question Creation", MessageBoxButton.OK, MessageBoxImage.Error);
          }
        }
      }
      else if (questionTypes.SelectedIndex == 2)
      {
        if (String.IsNullOrEmpty(descriptiveQuestion.Text)) 
        {
          MessageBox.Show("Question Text cannot be empty\nPlease enter a question text",
            "Question Creation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else if (String.IsNullOrEmpty(descriptiveAnswer.Text))
        {
          MessageBox.Show("Question Answer cannot be empty\nPlease enter a question answer",
            "Question Creation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else 
        {
          try
          {
            SQLManager.addDescriptiveQuestion(new DescriptiveQuestion(descriptiveQuestion.Text, 
              new DescriptiveAnswer(descriptiveAnswer.Text), -1, 
              questionSubjects.SelectedIndex));
            resetDescriptiveQuestion();
            resetQuestionTypeSelection();
            MessageBox.Show("Question creation is successful\n", "Question Creation", 
              MessageBoxButton.OK, MessageBoxImage.Information);
          }
          catch (Exception ex) 
          {
            MessageBox.Show("Question creation is failed\n" + ex.Message, 
              "Question Creation", MessageBoxButton.OK, MessageBoxImage.Error);
          }
        }
      }
      else 
      {
        int selection = questionFunctionComboBox.SelectedIndex;
        if (selection < 1) 
        {
          MessageBox.Show("Please choose a valid question function\n", 
            "Question Creation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else if ((selection == 1 ||
                selection == 2 ||
                selection == 3 ||
                selection == 4) && !checkDirectoryOrFileName(actionName1TextBox.Text)) 
        {
          MessageBox.Show("Diretory or File name isn't valid\nPlease enter a valid name\n",
              "Question Creation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else if ((selection == 5 || selection == 6) && 
                (!checkDirectoryOrFileName(actionName1TextBox.Text) ||
                !checkDirectoryOrFileName(actionName1TextBox.Text))) 
        {
          MessageBox.Show("Diretory or File names aren't valid\nPlease enter valid names\n",
            "Question Creation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else if(selection == 7 && String.IsNullOrEmpty(actionText1TextBox.Text))
        {
          MessageBox.Show("Entered Text cannot be empty\nPlease enter text", 
            "Question Creation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else if (selection == 8 && String.IsNullOrEmpty(actionText2TextBox.Text))
        {
          MessageBox.Show("Entered Texts cannot be empty\nPlease enter text",
            "Question Creation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else 
        {
          String questionText = null;
          String parameter1 = null;
          String parameter2 = null;

          if (selection == 1) 
          {
            parameter1 = actionName1TextBox.Text;
            questionText = "Create a directory with name " + parameter1;
          }
          else if (selection == 2) 
          {
            parameter1 = actionName1TextBox.Text;
            questionText = "Create a text file with name " + parameter1;
          }
          else if (selection == 3)
          {
            parameter1 = actionName1TextBox.Text;
            questionText = "Delete the directory with name " + parameter1;
          }
          else if (selection == 4)
          {
            parameter1 = actionName1TextBox.Text;
            questionText = "Delete the text file with name " + parameter1;
          }
          else if (selection == 5)
          {
            parameter1 = actionName1TextBox.Text;
            parameter2 = actionName2TextBox.Text;
            questionText = "Change the directory name from " + parameter1 + " to " + parameter2;
          }
          else if (selection == 6)
          {
            parameter1 = actionName1TextBox.Text;
            parameter2 = actionName2TextBox.Text;
            questionText = "Change the text file name from " + parameter1 + " to " + parameter2;
          }
          else if (selection == 7)
          {
            parameter1 = actionText1TextBox.Text;
            questionText = "Write " + parameter1 + " into text file with name enlightenment_test.txt"; 
          }
          else if (selection == 8)
          {
            parameter1 = actionText1TextBox.Text;
            parameter2 = actionText2TextBox.Text;
            questionText = "Change " + parameter1 + " to " + parameter2 + 
              " in text file with name enlightenment_test.txt";
          }
          try
          {
            SQLManager.addActionBasedQuestion(new ActionBasedQuestion(
                questionText, selection, parameter1, parameter2, -1, questionSubjects.SelectedIndex));
            resetActionBasedQuestion();
            resetQuestionTypeSelection();
            MessageBox.Show("Question creation is successful\n", "Question Creation", 
              MessageBoxButton.OK, MessageBoxImage.Information);
          }
          catch (Exception ex)
          {
            MessageBox.Show("Question creation is failed\n" + ex.Message, 
              "Question Creation", MessageBoxButton.OK, MessageBoxImage.Error);
          }
        }
      }
    }

    private bool checkDirectoryOrFileName(String name) 
    {
      return !((String.IsNullOrEmpty(name) || name.Contains("<") ||
        name.Contains(">") || name.Contains(":") || name.Contains("\"") ||
        name.Contains("/") || name.Contains("\\") || name.Contains("|") ||
        name.Contains("?") || name.Contains("*")))    
    }
    #endregion      
  }
}
