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
  /// Interaction logic for Candidate6.xaml
  /// </summary>
  public partial class Candidate6 : Window
  {
    private Candidate candidate; //candidate which is using the window

    public Candidate6(Candidate candidate)
    {
      this.candidate = candidate;

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
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      Speech.speakPurge("Press P to practice again");
      Speech.speakNormal("Press S to sign out");
    }

    private void Window_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) 
      {
        Speech.speakPurge(null);
        signoutButton_Click(null, null);
      }
      else if (e.Key == Key.P && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) 
      {
        Speech.speakPurge(null);
        practiceButton_Click(null, null);
      }
      else if (e.Key == Key.H && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        Speech.speakPurge("Press P to practice again");
        Speech.speakNormal("Press S to sign out");
      }
      else if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        Speech.changeVoice();
      }
    }

    private void practiceButton_Click(object sender, RoutedEventArgs e)
    {
      Speech.speakPurge(null);
      
      Dispatcher.BeginInvoke((ThreadStart)delegate
      {
          new Candidate1(candidate, Subject.None, Freshness.None, Screen.getEmptySeen()).Show();
      }, DispatcherPriority.Normal);

      Dispatcher.BeginInvoke((ThreadStart)delegate { this.Close(); },
          DispatcherPriority.Normal);
    }

    private void signoutButton_Click(object sender, RoutedEventArgs e)
    {
      Speech.speakPurgeSync("Bye " + candidate.UserName);
      this.Close();
    }
    #endregion
  }
}
