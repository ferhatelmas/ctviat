using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Threading;

using DAO.managers;
using Common.user;
using ProjectEnlightenment.Utilities;

namespace ProjectEnlightenment.UI
{
  /// <summary>
  /// Interaction logic for Login.xaml
  /// </summary>
  public partial class Login : Window
  {
    Candidate candidate = null;

    public Login()
    {
      this.candidate = null;
      InitializeComponent();
      InitializeBackgroundImage();
      this.username.Focus();
    }

    private void InitializeBackgroundImage()
    {
        backgroundImage.Source = 
        new BitmapImage(new Uri("/ProjectEnlightenment;component/Resources/enlightenment.jpg", 
          UriKind.Relative));
    }

    /// <summary>
    ///     Window Events won't be explained since you can see their effects on the QUI
    /// </summary>
    #region Window Events
    private void submit_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        User user = SQLManager.authenticateUser(username.Text, 
          Security.getMd5Hash(password.Password));

        if (user == null)
        {
          MessageBox.Show("There is not any user with these credentials", 
            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
          username.Focus();
          username.SelectAll();
        }
        else if (user is Candidate)
        {
          mainGrid.Children.Remove(loginGrid);
          mainGrid.RowDefinitions.Clear();

          candidate = (Candidate)user;

          Dispatcher.BeginInvoke((ThreadStart)delegate{
            Speech.speakPurge("Welcome " + candidate.UserName);
            Speech.speakNormal("Press M to listen user manual");
            Speech.speakNormal("Press N to skip or interrupt while listening");
            Speech.speakNormal("Press H to listen this message again"); 
          }, DispatcherPriority.Normal);
        }
        else
        {
            Dispatcher.BeginInvoke((ThreadStart)delegate
            {
                new AdminMain((Admin)user).Show();
            }, DispatcherPriority.Normal);
            Dispatcher.BeginInvoke((ThreadStart)delegate { this.Close(); }, DispatcherPriority.Normal);
        }
      }
      catch (Exception ex)
      {
          MessageBox.Show("Login is failed\n" + ex.Message, 
              "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private void Window_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter) 
      {
        submit_Click(null, null);
      }
      else if (e.Key == Key.H && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        Speech.adjustRate(0);
        Speech.speakPurge("Press M to listen user manual");
        Speech.speakNormal("Press N to skip or interrupt while listening");
      }
      else if (e.Key == Key.M && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        Speech.adjustRate(-1);
        Speech.speakPurge("In the Test Subject Selection Screen " +
        "Press 1 for choosing dialog " +
        "Press 2 for choosing editing " +
        "Press Ctrl + N to continue next window " +
        "Press Ctrl + H for listening audio help " +
        "Press Ctrl + S to sign out " +
        "Press Ctrl + C to change voice " +
        "In the Test Freshness Selection Screen " +
        "Press 1 for choosing new test " +
        "Press 2 for choosing old test " +
        "Press Ctrl + N to continue next window " +
        "Press Ctrl + H for listening audio help " +
        "Press Ctrl + S to sign out " +
        "Press Ctrl + B to return the previous window " +
        "Press Ctrl + C to change voice " +
        "In Test Selection Screen " +
        "Press up or down to navigate in the test list " +
        "Press Ctrl + H for listening the audio help " +
        "Press Ctrl + N to start the selected test " +
        "Press Ctrl + B to return the previous window " +
        "Press Ctrl + S to sign out " +
        "Press Ctrl + C to change voice " +
        "In Test Solving Screen " +
        "Press Ctrl + N to move next question " +
        "Press Ctrl + B to return the previous question " +
        "Press Ctrl + F to finish solving " +
        "Press Ctrl + D to discontinue solving " +
        "Press Ctrl +T to listen the elapsed time " +
        "Press Ctrl +R to listen the question " +
        "Press Ctrl + H to listen the audio help " +
        "For objective questions " +
        "Press 1 for option 1 " +
        "Press 2 for option 2 " +
        "Press 3 for option 3 " +
        "Press 4 for option 4 " +
        "Press 5 to unselect the options " +
        "Press Ctrl + C to change voice " +
        "In Test Answer Listening Screen " +
        "Press Ctrl + N to move next question " +
        "Press Ctrl + B to return the previous question " +
        "Press Ctrl + F to finish listening the answers " +
        "Press Ctrl +R to listen the answer " +
        "Press Ctrl + H to listen the audio help " +
        "Press Ctrl + C to change voice " +
        "In the last Screen " +
        "Press Ctrl + P to practice again " +
        "Press Ctrl + S to sign out " +
        "Press Ctrl + H to listen the audio help" +
        "Press Ctrl + C to change voice ");
      }
      else if (e.Key == Key.N && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        Speech.adjustRate(0);
        Speech.speakPurge(null);
        Dispatcher.BeginInvoke((ThreadStart)delegate
        {
            new Candidate1(candidate, Subject.None, Freshness.None, Screen.getEmptySeen()).Show();
        }, DispatcherPriority.Normal);
        Dispatcher.BeginInvoke((ThreadStart)delegate { this.Close(); }, DispatcherPriority.Normal);
      }
      else if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
          Speech.changeVoice();
      }
    }        
    #endregion
  }
}
