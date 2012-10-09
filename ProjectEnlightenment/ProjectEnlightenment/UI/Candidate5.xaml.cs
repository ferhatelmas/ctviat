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
using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;

using Common.user;
using Common.question;

using Automation;

namespace ProjectEnlightenment.UI
{
  /// <summary>
  /// Interaction logic for Candidate5.xaml
  /// </summary>
  public partial class Candidate5 : Window
  {
    private Candidate candidate; //candidate which is using this window
    private TestSolver solver;   //test solve logic, holds test and submitted answers 

    public Candidate5(Candidate candidate, TestSolver solver)
    {
      this.candidate = candidate;
      this.solver = solver;
      this.solver.setToStart();
      InitializeComponent();
      setWindowProperties();
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
    /// determines question type and shows the question, correct and submitted answer
    /// </summary>
    private void showQuestionAndAnswer() 
    {
      Question question = solver.getCurrentQuestion();
      if (question is ObjectiveQuestion) 
      {
        showObjectiveQuestionAndAnswer((ObjectiveQuestion)question);
      }
      else if (question is DescriptiveQuestion)
      {
        showDescriptiveQuestionAndAnswer((DescriptiveQuestion)question);
      }
      else 
      {
        showActionBasedQuestionAndAnswer((ActionBasedQuestion)question);
      }
    }

    /// <summary>
    /// shows objective question, correct and submitted answer, adjust visibility
    /// </summary>
    /// <param name="q">objective question to be shown</param>
    private void showObjectiveQuestionAndAnswer(ObjectiveQuestion q)
    {
      questionLabel.Text = q.QuestionText;
      correctAnswerLabel.Text = q.Choices[q.Answer.Answer-1];
      Speech.speakPurge("Objective Question");
      Speech.speakNormal(q.QuestionText);
      
      Speech.speakNormal("Correct Answer");
      Speech.speakNormal(q.Choices[q.Answer.Answer-1]);

      if (q.Submitted != null)
      {
        submittedAnswerLabel.Text = q.Choices[q.Submitted.Answer-1];
        Speech.speakNormal("Your answer");
        Speech.speakNormal(q.Choices[q.Submitted.Answer-1]);
      }
      else 
      {
        submittedAnswerLabel.Text = "";
        Speech.speakNormal("You didn't answer");
      }
    }

    /// <summary>
    /// shows descriptive question, correct and submitted answer, adjust visibility
    /// </summary>
    /// <param name="q">descriptive question to be shown</param>
    private void showDescriptiveQuestionAndAnswer(DescriptiveQuestion q) 
    {
      questionLabel.Text = q.QuestionText;
      correctAnswerLabel.Text = q.Answer.Answer;

      Speech.speakPurge( "Descriptive Question");
      Speech.speakNormal(q.QuestionText);

      Speech.speakNormal("Correct Answer");
      Speech.speakNormal(q.Answer.Answer);

      if (q.Submitted != null)
      {
        submittedAnswerLabel.Text = q.Submitted.Answer;
        Speech.speakNormal("Your answer");
        Speech.speakNormal(q.Submitted.Answer);
      }
      else
      {
        submittedAnswerLabel.Text = "";
        Speech.speakNormal("You didn't answer");
      }
    }

    private void showActionBasedQuestionAndAnswer(ActionBasedQuestion q) 
    {
      questionLabel.Text = q.QuestionText;
      correctAnswerLabel.Text = "Correct Answer isn't given for action based questions";

      Speech.speakPurge("Action Based Question");
      Speech.speakNormal(q.QuestionText);

      if (q.Submitted != null)
      {
        submittedAnswerLabel.Text = "You answered correctly";
        Speech.speakNormal("You answered correctly");
      }
      else 
      {
        submittedAnswerLabel.Text = "You didn't answer or answered wrong";
        Speech.speakNormal("You didn't answer or answered wrong");
      }
    }

    /// <summary>
    ///     Window Events won't be explained since you can see their effects on the QUI
    /// </summary>
    #region Window Events
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      long time = solver.PassedTime;
      String message = "You have completed the test in ";

      if (time / 3600 > 0)
      {
        message = time / 3600 + " hours";
      }
      if (time % 3600 / 60 > 0)
      {
        message = message + time % 3600 / 60 + " minutes";
      }
      message = message + time % 60 + " seconds";

      Speech.speakPurgeSync(message);
      Speech.speakPurgeSync("Now answers will be shown");
      showQuestionAndAnswer();
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
      else if (e.Key == Key.R && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        if (solver.getCurrentQuestion() is ObjectiveQuestion)
        {
          ObjectiveQuestion q = (ObjectiveQuestion)solver.getCurrentQuestion();

          Speech.speakPurge("Objective Question");
          Speech.speakNormal(q.QuestionText);

          Speech.speakNormal("Correct Answer");
          Speech.speakNormal(q.Choices[q.Answer.Answer-1]);
          if (q.Submitted != null)
          {
            Speech.speakNormal("Your Answer");
            Speech.speakNormal(q.Choices[q.Submitted.Answer-1]);
          }
          else 
          {
            Speech.speakNormal("You didn't answer");
          }
        }
        else if (solver.getCurrentQuestion() is DescriptiveQuestion)
        {
          DescriptiveQuestion q = (DescriptiveQuestion)solver.getCurrentQuestion();

          Speech.speakPurge("Descriptive Question");
          Speech.speakNormal(q.QuestionText);

          Speech.speakNormal("Correct Answer");
          Speech.speakNormal(q.QuestionText);
          if (q.Submitted != null)
          {
            Speech.speakNormal("Your Answer");
            Speech.speakNormal(q.Submitted.Answer);
          }
          else
          {
            Speech.speakNormal("You didn't answer");
          }
        }
        else
        {
          ///action-based here
        }
      }
      else if (e.Key == Key.F && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) 
      {
        Speech.speakPurge(null);
        finishButton_Click(null, null);
      }
      else if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
          Speech.changeVoice();
      }
    }

    private void nextButton_Click(object sender, RoutedEventArgs e)
    {
      if (solver.isNext())
      {
        Speech.speakPurge(null);
        solver.proceedToNextQuestion();
        showQuestionAndAnswer();
      }
      else
      {
        Speech.speakPurge("This is the last question");
      }
    }

    private void backButton_Click(object sender, RoutedEventArgs e)
    {
      if (solver.isBack())
      {
        Speech.speakPurge(null);
        solver.returnToPreviousQuestion();
        showQuestionAndAnswer();
      }
      else
      {
        Speech.speakPurge("This is the first question");
      }
    }

    private void finishButton_Click(object sender, RoutedEventArgs e)
    {
      Speech.speakPurge(null);
      Dispatcher.BeginInvoke((ThreadStart)delegate
      {
          new Candidate6(candidate).Show();
      }, DispatcherPriority.Normal);

      Dispatcher.BeginInvoke((ThreadStart)delegate { this.Close(); },
          DispatcherPriority.Normal);
    }
    #endregion
  }
}
