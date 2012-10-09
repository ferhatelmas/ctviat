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
using System.Windows.Threading;
using System.Threading;
using System.ComponentModel;

using Common.user;
using ProjectEnlightenment.Utilities;

namespace ProjectEnlightenment.UI
{
  /// <summary>
  ///   Interaction logic for Candidate2.xaml
  /// </summary>
  public partial class Candidate2 : Window
  {
    private Candidate candidate; //which candidate is using
    private Subject subject;  //test subject
    private Freshness freshness; //test freshness
    private bool[] seen; //window seen array

    public Candidate2(Candidate candidate, Subject subject, Freshness freshness, bool[] seen)
    {
      this.candidate = candidate;
      this.subject = subject;
      this.freshness = freshness;
      this.seen = seen;

      InitializeComponent();
      setWindowProperties();

      checkCheckBoxes();
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
    ///   adjust checkboxes according to test freshness
    /// </summary>
    private void checkCheckBoxes()
    {
      if (freshness == Freshness.None)
      {
        newCheckBox.IsChecked = false;
        oldCheckBox.IsChecked = false;
      }
      else if (freshness == Freshness.New)
      {
        newCheckBox.IsChecked = true;
      }
      else if (freshness == Freshness.Old)
      {
        oldCheckBox.IsChecked = true;
      }
    }

    /// <summary>
    ///   Window Events won't be explained since you can see their effects on the QUI
    /// </summary>
    #region Window Events
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      Dispatcher.BeginInvoke((ThreadStart)delegate
      {
        if (!seen[1])
        {
          Speech.speakPurge("Test Freshness Selection");
          Speech.speakNormal("Press one for new test");
          Speech.speakNormal("or Press two for old test");
          Speech.speakNormal("Then Press N to continue");
        }
        else 
        {
          if (freshness == Freshness.New)
          {
            Speech.speakPurge("New was chosen");
            Speech.speakNormal("Press two for old");
            Speech.speakNormal("Press N to continue");
          }
          else if (freshness == Freshness.Old)
          {
            Speech.speakPurge("Old was chosen");
            Speech.speakNormal("Press one for new");
            Speech.speakNormal("Press N to continue");
          }
          else 
          {
            Speech.speakPurge("No selection");
            Speech.speakNormal("Press one for new");
            Speech.speakNormal("Press two for old");
          }
        }
      }, DispatcherPriority.Loaded);
    }

    private void newCheckBox_Checked(object sender, RoutedEventArgs e)
    {
      Speech.speakPurge(null);
      freshness = Freshness.New;
      if ((bool)oldCheckBox.IsChecked) oldCheckBox.IsChecked = false;
    }

    private void oldCheckBox_Checked(object sender, RoutedEventArgs e)
    {
      Speech.speakPurge(null);
      freshness = Freshness.Old;
      if ((bool)newCheckBox.IsChecked) newCheckBox.IsChecked = false;
    }

    private void Window_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.N && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        if (freshness != Freshness.None)
        {
          seen[1] = true;
          Speech.speakPurge(null);
          Dispatcher.BeginInvoke((ThreadStart)delegate
          {
              new Candidate3(candidate, subject, freshness, seen).Show();
          }, DispatcherPriority.Normal);
                    
                   
          Dispatcher.BeginInvoke((ThreadStart)delegate { this.Close(); }, 
            DispatcherPriority.Normal);
        }
        else
        {
          Speech.speakPurge("Please choose freshness to continue");
        }
      }
      else if (e.Key == Key.B && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) 
      {
        Speech.speakPurge(null);
        backButton_Click(null, null);
      }
      else if (e.Key == Key.H && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        Speech.speakPurge("Test Freshness Selection");
        Speech.speakNormal("Press one for new test");
        Speech.speakNormal("or Press two for old test");
        Speech.speakNormal("Then Press N to continue");
      }
      else if (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        Speech.speakPurge(null);
        signoutButton_Click(null, null);
      }
      else if (e.Key == Key.D1 || e.Key == Key.NumPad1)
      {
        Speech.speakPurge(null);
        freshness = Freshness.New;
        newCheckBox.IsChecked = true;
      }
      else if (e.Key == Key.D2 || e.Key == Key.NumPad2)
      {
        Speech.speakPurge(null);
        freshness = Freshness.Old;
        oldCheckBox.IsChecked = true;
      }
      else if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        Speech.changeVoice();
      }
    }

    private void signoutButton_Click(object sender, RoutedEventArgs e)
    {
      Speech.speakPurgeSync("Bye " + candidate.UserName);
      this.Close();
    }

    private void backButton_Click(object sender, RoutedEventArgs e)
    {
      Speech.speakPurge(null);
      seen[1] = true;

      Dispatcher.BeginInvoke((ThreadStart)delegate
      {
          new Candidate1(candidate, subject, freshness, seen).Show();
      }, DispatcherPriority.Normal);

      Dispatcher.BeginInvoke((ThreadStart)delegate { this.Close(); }, 
        DispatcherPriority.Normal);
    }
    #endregion
  }
}
