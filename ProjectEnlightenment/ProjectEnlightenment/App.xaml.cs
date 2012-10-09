using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ProjectEnlightenment.UI;

namespace ProjectEnlightenment
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    protected override void OnStartup(StartupEventArgs e)
    {          
      //add event handlers for textbox
      EventManager.RegisterClassHandler(typeof(TextBox), TextBox.PreviewMouseLeftButtonDownEvent,
          new MouseButtonEventHandler(SelectivelyIgnoreMouseButton));
      EventManager.RegisterClassHandler(typeof(TextBox), TextBox.GotKeyboardFocusEvent,
          new RoutedEventHandler(SelectAllText));
      EventManager.RegisterClassHandler(typeof(TextBox), TextBox.MouseDoubleClickEvent,
          new RoutedEventHandler(SelectAllText));
            
      //add event handlers for passwordbox
      EventManager.RegisterClassHandler(typeof(PasswordBox), PasswordBox.PreviewMouseLeftButtonDownEvent,
          new MouseButtonEventHandler(SelectivelyIgnoreMouseButton));
      EventManager.RegisterClassHandler(typeof(PasswordBox), PasswordBox.GotKeyboardFocusEvent,
          new RoutedEventHandler(SelectAllText));
      EventManager.RegisterClassHandler(typeof(PasswordBox), PasswordBox.MouseDoubleClickEvent,
          new RoutedEventHandler(SelectAllText));
      
      //call base startup code
      base.OnStartup(e);
    }

    void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
    {
      //find textbox and password boxes
      DependencyObject parent = e.OriginalSource as UIElement;
      while (parent != null && !(parent is TextBox) && !(parent is PasswordBox))
          parent = VisualTreeHelper.GetParent(parent);

      //got focus
      if (parent != null)
      {
        if (parent is TextBox) 
        {
          var textBox = (TextBox)parent;
          if (!textBox.IsKeyboardFocusWithin) 
          {
            textBox.Focus();
            e.Handled = true;
          }
        }
        else if (parent is PasswordBox) 
        {
          var passwordBox= (PasswordBox)parent;
          if (!passwordBox.IsKeyboardFocusWithin)
          {
            passwordBox.Focus();
            e.Handled = true;
          }
        }
      }
    }

    void SelectAllText(object sender, RoutedEventArgs e)
    {
      //select all
      if (e.OriginalSource is TextBox) 
      {
        var textBox = e.OriginalSource as TextBox;
        if (textBox != null) textBox.SelectAll();
      }
      else if (e.OriginalSource is PasswordBox) 
      {
        var passwordBox = e.OriginalSource as PasswordBox;
        if (passwordBox != null) passwordBox.SelectAll();
      }
    }
  }
}
