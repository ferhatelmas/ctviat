using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections;
using Microsoft.Win32;
using System.IO;

using Common.user;
using Common.test;
using Common.question;
using DAO.managers;

using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ProjectEnlightenment.UI
{
  /// <summary>
  /// Interaction logic for ReportCreation.xaml
  /// </summary>
  public partial class ReportCreation : Window
  {
    private Admin admin; //admin which is using the window
    private bool focusFlag; //focus is on search text box

    public ReportCreation(Admin admin)
    {
      InitializeComponent();
      this.admin = admin;
      setWindowProperties();
      createButton.IsEnabled = false;
      searchTextBox.Focus();
      focusFlag = true;
    }

    /// <summary>
    ///     sets window parameters
    ///     always on top and full screen
    /// </summary>
    private void setWindowProperties()
    {
      //this.TopMost = true;
      this.Width = SystemParameters.PrimaryScreenWidth;
      this.Height = SystemParameters.PrimaryScreenHeight;
      this.Top = 0;
      this.Left = 0;
    }

    /// <summary>
    ///     Window Events won't be explained since you can see their effects on the QUI
    /// </summary>
    #region Window Events
    private void searchButton_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        usersListView.Items.Clear();
        ArrayList users = SQLManager.getAllCandidates(searchTextBox.Text);
        foreach (Candidate c in users)
        {
          usersListView.Items.Add(c);
        }
        searchTextBox.Focus();
      }
      catch (Exception ex)
      {
        MessageBox.Show("User fetching is failed\n" + ex.Message, 
          "Report Creation", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private void backButton_Click(object sender, RoutedEventArgs e)
    {
      new AdminMain(admin).Show();
      this.Close();
    }

    private void createButton_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        Candidate c = (Candidate)usersListView.SelectedItem;
        System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();

        if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
          Document doc = new Document(PageSize.A4, 72, 72, 72, 72);
          PdfWriter.GetInstance(doc, new FileStream(System.IO.Path.Combine(fbd.SelectedPath,
            c.UserName + "_CTVIAT_Report.pdf"), FileMode.Create));
          doc.Open();

          Paragraph p1 = new Paragraph(c.UserName);
          p1.Font.Size = 24;
          p1.Font.SetStyle(Font.ITALIC);
          doc.Add(p1);

          ArrayList tests = SQLManager.getAllTestsOfCandidate(c.UserID);
          if (tests.Count == 0)
          {
            doc.Add(new Paragraph("Candidate didn't solve any tests"));
          }
          else
          {
            int i = 1;
            foreach (Test t in tests)
            {
              String testInfo = "TEST " + i + "\nTEST ID: " + t.TestID + 
              "\nTEST SUBJECT: " + t.Subject + "\nTEST DATE: " + t.Date + 
              "\nTEST TAKE ORDER: " + t.TakeOrder + "\nTEST TIME: ";

              int time = t.Time;
              if (time / 3600 > 0)
              {
                if (time / 3600 == 1) testInfo = testInfo + time / 3600 + " hour";
                else testInfo = testInfo + time / 3600 + " hours";
              }
              if (time % 3600 / 60 > 0)
              {
                if (time % 3600 / 60 == 1) testInfo = testInfo + time % 3600 / 60 + " minute";
                else testInfo = testInfo + time % 3600 / 60 + " minutes";
              }
              if (time % 60 == 1)
              {
                testInfo = testInfo + time % 60 + " second\n";
              }
              else
              {
                testInfo = testInfo + time % 60 + " seconds\n";
              }

              if (t.IsNew)
              {
                testInfo = testInfo + "TEST COMPLETENESS: COMPLETED\n";
              }
              else
              {
                testInfo = testInfo + "TEST COMPLETENESS: INCOMPLETED\nTEST START QUESTION NO: "
                  + t.StartQuestionNo + "\n";
              }

              Paragraph p = new Paragraph(testInfo);
              p.Font.SetStyle(Font.BOLD);
              doc.Add(p);

              foreach (Question q in t.Questions)
              {
                String questionInfo = "QUESTION\n" + q.QuestionText + "\nCANDIDATE ANSWER\n";

                if (q is ObjectiveQuestion)
                {
                  ObjectiveQuestion oq = (ObjectiveQuestion)q;
                  if (oq.Submitted != null)
                  {
                    questionInfo = questionInfo + oq.Choices[oq.Submitted.Answer - 1] + "\n";
                  }
                  else
                  {
                    questionInfo = questionInfo + "Candidate didn't answer the question\n";
                  }
                }
                else if (q is DescriptiveQuestion)
                {
                  DescriptiveQuestion dq = (DescriptiveQuestion)q;
                  if (dq.Submitted != null)
                  {
                    questionInfo = questionInfo + dq.Submitted.Answer;
                  }
                  else
                  {
                    questionInfo = questionInfo + "Candidate didn't answer the question\n";
                  }
                }
                else
                {
                  ActionBasedQuestion aq = (ActionBasedQuestion)q;
                  if (aq.Submitted != null)
                  {
                    questionInfo = questionInfo + "Candidate answered the question correctly";
                  }
                  else
                  {
                    questionInfo = questionInfo + "Candidate didn't answer the question\n";
                  }
                }

                Paragraph questionParagraph = new Paragraph(questionInfo);
                doc.Add(questionParagraph);
              }
              i++;
            }
          }
          doc.Close();
          MessageBox.Show("Report Creation is successful\n", "Report Creation", 
            MessageBoxButton.OK, MessageBoxImage.Information);
        }
      }
      catch (Exception ex) 
      {
        MessageBox.Show("Report Creation is failed\n" + ex.Message, 
          "Report Creation", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private void usersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (usersListView.SelectedIndex == -1)
      {
        createButton.IsEnabled = false;
      }
      else
      {
        createButton.IsEnabled = true;
      }
    }

    private void searchTextBox_GotFocus(object sender, RoutedEventArgs e)
    {
      focusFlag = true;
    }

    private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
      if (focusFlag && e.Key == Key.Enter) 
      {
        searchButton_Click(null, null);
      }
    }
    #endregion
  }
}
