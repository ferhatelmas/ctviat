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
  /// Interaction logic for Candidate1.xaml
  /// </summary>
  public partial class Candidate1 : Window
  {
    private Candidate candidate; //which candidate is using the window
    private Subject subject;   //selected test subject
    private Freshness freshness; //selected test freshness, old(incomplete) or new(complete)
    private bool[] seen; //boolean seen array of windows to speak different messages on windows

    public Candidate1(Candidate candidate, Subject subject, Freshness freshness, bool[] seen)
    {
      this.candidate = candidate;
      this.subject = subject;
      this.freshness = freshness;
      this.seen = seen;

      InitializeComponent();
      setWindowProperties();

      checkCheckBoxes();
      this.welcomeLabel.Content = "WELCOME " + candidate.UserName;
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
    ///   adjust checkBoxes according to subject
    /// </summary>
    private void checkCheckBoxes() 
    {
      if (subject == Subject.None) 
      {
          dialogCheckBox.IsChecked = false;
          editingCheckBox.IsChecked = false;
      }
      else if (subject == Subject.Dialog) 
      {
        dialogCheckBox.IsChecked = true;
      }
      else if (subject == Subject.Editing) 
      {
        editingCheckBox.IsChecked = true;
      }
    }

    /// <summary>
    ///   Window Events won't be explained since you can see their effects on the QUI
    /// </summary>
    #region Window Events
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      Dispatcher.BeginInvoke((ThreadStart)delegate {
        if (!seen[0])
        {
          Speech.speakPurge("Test Subject Selection");
          Speech.speakNormal("Press one for dialog");
          Speech.speakNormal("Press two for editing");
          Speech.speakNormal("Press N to continue");
        }
        else
        {
          if (subject == Subject.Dialog)
          {
            Speech.speakPurge("Dialog was chosen");
            Speech.speakNormal("Press two for editing");
            Speech.speakNormal("Press N to continue");
          }
          else if (subject == Subject.Editing)
          {
            Speech.speakPurge("Editing was chosen");
            Speech.speakNormal("Press one for dialog");
            Speech.speakNormal("Press N to continue");
          }
        }
      }, DispatcherPriority.Loaded);
    }

    private void dialogCheckBox_Checked(object sender, RoutedEventArgs e)
    {
      Speech.speakPurge(null);
      subject = Subject.Dialog;
      if ((bool)editingCheckBox.IsChecked) editingCheckBox.IsChecked = false;
    }

    private void editingCheckBox_Checked(object sender, RoutedEventArgs e)
    {
      Speech.speakPurge(null);
      subject = Subject.Editing;
      if ((bool)dialogCheckBox.IsChecked) dialogCheckBox.IsChecked = false;
    }

    private void Window_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.N && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        if (subject != Subject.None)
        {
          seen[0] = true;
          Speech.speakPurge(null);

          Dispatcher.BeginInvoke((ThreadStart)delegate
          {
              new Candidate2(candidate, subject, freshness, seen).Show();
          }, DispatcherPriority.Normal);
                
          Dispatcher.BeginInvoke((ThreadStart)delegate { this.Close(); }, DispatcherPriority.Normal);
        }
        else
        {
            Speech.speakPurge("Please choose subject to continue");
        }
      }
      else if (e.Key == Key.H && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        Speech.speakPurge("Test Subject Selection");
        Speech.speakNormal("Press one for dialog");
        Speech.speakNormal("Press two for editing");
        Speech.speakNormal("Press N to continue");
      }
      else if (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        Speech.speakPurge(null);
        signoutButton_Click(null, null);
      }
      else if (e.Key == Key.D1 || e.Key == Key.NumPad1)
      {
        Speech.speakPurge(null);
        subject = Subject.Dialog;
        dialogCheckBox.IsChecked = true;
      }
      else if (e.Key == Key.D2 || e.Key == Key.NumPad2)
      {
        Speech.speakPurge(null);
        subject = Subject.Editing;
        editingCheckBox.IsChecked = true;
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
    #endregion
  }
}
