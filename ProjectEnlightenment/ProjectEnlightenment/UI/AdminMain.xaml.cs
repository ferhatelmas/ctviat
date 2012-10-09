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

namespace ProjectEnlightenment.UI
{
  /// <summary>
  /// Interaction logic for AdminMain.xaml
  /// </summary>
  public partial class AdminMain : Window
  {
    private Admin admin; //admin which is using the window

    public AdminMain(Admin admin)
    {
      this.admin = admin;
      InitializeComponent();
      this.Title = admin.UserName + " Home";
      this.welcomeLabel.Content = "WELCOME " + admin.UserName;
    }

    /// <summary>
    ///     Window Events won't be explained since you can see their effects on the QUI
    /// </summary>
    #region Window Events
    private void signoutButton_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void userCreateButton_Click(object sender, RoutedEventArgs e)
    {
      new UserCreation(admin).Show();
      this.Close();
    }

    private void testCreateButton_Click(object sender, RoutedEventArgs e)
    {
      new TestCreation(admin).Show();
      this.Close();
    }

    private void questionCreateButton_Click(object sender, RoutedEventArgs e)
    {
      new QuestionCreation(admin).Show();
      this.Close();
    }

    private void reportCreateButton_Click(object sender, RoutedEventArgs e)
    {
      new ReportCreation(admin).Show();
      this.Close();
    }
        
    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      this.DragMove();
    }

    #endregion
  }
}
