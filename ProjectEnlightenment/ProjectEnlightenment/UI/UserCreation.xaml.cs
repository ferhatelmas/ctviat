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

using DAO.managers;

using ProjectEnlightenment.Utilities;

namespace ProjectEnlightenment.UI
{
  /// <summary>
  /// Interaction logic for UserCreation.xaml
  /// </summary>
  public partial class UserCreation : Window
  {
    //One of the admin windows so to know which admin is using the window
    private Admin admin;

    public UserCreation(Admin admin)
    {
      InitializeComponent();
      setWindowProperties();
      this.admin = admin;
      this.createUserButton.IsEnabled = false;
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
    private void createUserButton_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (password.Password != repassword.Password)
        {
          MessageBox.Show("Passwords don't match", "User Creation Error",
            MessageBoxButton.OK, MessageBoxImage.Error);
          password.Focus();
        }
        else
        {
          if (SQLManager.getUserByUserName(userName.Text) != null)
          {
            MessageBox.Show("There is already an user with this username\nPlease choose another name",
              "User Creation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            userName.Focus();
          }
          else
          {
            if ((bool)isAdminCheckBox.IsChecked)
            {
              //type must one for admin
              SQLManager.addUser(userName.Text, Security.getMd5Hash(password.Password), 1);
            }
            else
            {
              //type must be zero for candidate
              SQLManager.addUser(userName.Text, Security.getMd5Hash(password.Password), 0);
              MessageBox.Show("User Creation is successful\n", "User Creation", 
                MessageBoxButton.OK, MessageBoxImage.Information);
            }
            userName.Text = "";
            password.Password = "";
            repassword.Password = "";
            userName.Focus();
          }
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("User Creation is failed\n" + ex.Message,"User Creation", 
          MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private void backButton_Click(object sender, RoutedEventArgs e)
    {
      new AdminMain(admin).Show();
      this.Close();
    }

    private void password_PasswordChanged(object sender, RoutedEventArgs e)
    {
      if (!String.IsNullOrEmpty(password.Password) &&
          !String.IsNullOrEmpty(userName.Text) &&
          !String.IsNullOrEmpty(repassword.Password))
      {
        this.createUserButton.IsEnabled = true;
      }
      else 
      {
        this.createUserButton.IsEnabled = false;
      }
    }

    private void userName_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (!String.IsNullOrEmpty(password.Password) &&
          !String.IsNullOrEmpty(userName.Text) &&
          !String.IsNullOrEmpty(repassword.Password))
      {
        this.createUserButton.IsEnabled = true;
      }
      else
      {
        this.createUserButton.IsEnabled = false;
      }
    }

    private void repassword_PasswordChanged(object sender, RoutedEventArgs e)
    {
      if (!String.IsNullOrEmpty(password.Password) &&
          !String.IsNullOrEmpty(userName.Text) &&
          !String.IsNullOrEmpty(repassword.Password))
      {
        this.createUserButton.IsEnabled = true;
      }
      else
      {
        this.createUserButton.IsEnabled = false;
      }
    }
    #endregion
  }
}
