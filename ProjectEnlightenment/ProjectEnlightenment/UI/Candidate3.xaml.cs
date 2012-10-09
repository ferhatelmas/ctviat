using System;
using System.Collections.Generic;
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
using System.Windows.Threading;
using System.Threading;
using System.ComponentModel;

using Common.user;
using Common.test;
using ProjectEnlightenment.Utilities;
using DAO.managers;

namespace ProjectEnlightenment.UI
{
  /// <summary>
  /// Interaction logic for Candidate3.xaml
  /// </summary>
  public partial class Candidate3 : Window
  {
    private Candidate candidate; //which candidate is using window
    private Subject subject;    //test subject
    private Freshness freshness; //test freshness
    private bool[] seen; //window seen array

    private Test test; //selected test

    public Candidate3(Candidate candidate, Subject subject, Freshness freshness, bool[] seen)
    {
      this.candidate = candidate;
      this.subject = subject;
      this.freshness = freshness;
      this.seen = seen;

      InitializeComponent();
      setWindowProperties();
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

    /// <summary>
    ///     Window Events won't be explained since you can see their effects on the QUI
    /// </summary>
    #region Window Events
    private void backButton_Click(object sender, RoutedEventArgs e)
    {
      Speech.speakPurge(null);
      seen[2] = true;

      Dispatcher.BeginInvoke((ThreadStart)delegate
      {
          new Candidate2(candidate, subject, freshness, seen).Show();
      }, DispatcherPriority.Normal);

      Dispatcher.BeginInvoke((ThreadStart)delegate { this.Close(); }, DispatcherPriority.Normal);
    }

    private void startButton_Click(object sender, RoutedEventArgs e)
    {            
      Speech.speakPurge(null);
      seen[2] = true;

      if (test.IsNew)
      {
        test.TakeOrder++;
      }
      
      Dispatcher.BeginInvoke((ThreadStart)delegate
      {
          new Candidate4(candidate, test, freshness).Show();
      }, DispatcherPriority.Normal);

      Dispatcher.BeginInvoke((ThreadStart)delegate { this.Close(); }, DispatcherPriority.Normal);
    }

    private void signoutButton_Click(object sender, RoutedEventArgs e)
    {
      Speech.speakPurgeSync("Bye " + candidate.UserName);
      this.Close();
    }

    private void Window_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.N && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        Speech.speakPurge(null);
        startButton_Click(null, null);
      }
      else if (e.Key == Key.Up)
      {
        if (testsListView.SelectedIndex > 0)
        {
          testsListView.SelectedIndex--;
        }
        else 
        {
          Speech.speakPurge("top of the test list");
        }
      }
      else if (e.Key == Key.Down) 
      {
        if (testsListView.SelectedIndex < testsListView.Items.Count - 1)
        {
          testsListView.SelectedIndex++;
        }
        else
        {
          Speech.speakPurge("bottom of the test list");
        }
      }
      else if (e.Key == Key.B && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        Speech.speakPurge(null);
        backButton_Click(null, null);
      }
      else if (e.Key == Key.H && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        Speech.speakPurge("Please use arrows to select the test");
        Speech.speakNormal("Press N to continue");
      }
      else if (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        Speech.speakPurge(null);
        signoutButton_Click(null, null);
      }
      else if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        Speech.changeVoice();
      }         
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        ArrayList tests = null;
        if (freshness == Freshness.Old)
        {
          if (subject == Subject.Dialog)
          {
            tests = SQLManager.getAllIncompleteTestsBySubject(candidate.UserID, 1);
          }
          else
          {
            tests = SQLManager.getAllIncompleteTestsBySubject(candidate.UserID, 2);
          }
        }
        else
        {
          if (subject == Subject.Dialog)
          {
            tests = SQLManager.getAllTestsBySubject(candidate.UserID, 1);
          }
          else
          {
            tests = SQLManager.getAllTestsBySubject(candidate.UserID, 2);
          }
        }

        if (tests.Count == 0)
        {
          Speech.speakPurge("There are no tests");
          Speech.speakNormal("Press B to go back and change selections");
        }
        else
        {
          foreach (Test t in tests)
          {
              testsListView.Items.Add(t);
          }

          Dispatcher.BeginInvoke((ThreadStart)delegate
          {
              testsListView.SelectedIndex = 0;
          }, DispatcherPriority.Loaded);
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

    private void testsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      test = (Test)testsListView.SelectedItem;
      Speech.speakPurge(test.ToString());
    }
    #endregion
  }
}
