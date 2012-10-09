using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Common.test;
using Common.question;
using Common.answer;

using DAO.managers;

namespace Automation
{
  public delegate void TestProgressHandler(object sender, TestEventArgs e);

  public sealed class TestSolver
  {
    //the caller when the timer expires
    public event TestProgressHandler TimeElapsed; 
    //timer to count the second that are passed
    private Timer timer; 

    //selected test
    private Test test;   
    //current question number which has been showing in the QUI
    private int currentQuestionNumber; 

    public TestSolver(Test test, TestProgressHandler timeElapsed) 
    {
      this.test = test;
      this.currentQuestionNumber = test.StartQuestionNo;
      timer = new Timer(new TimerCallback(timer_CallBack), 
                                          null, 
                                          Timeout.Infinite, 
                                          Timeout.Infinite);
      TimeElapsed += timeElapsed;
    }

    public Test Test 
    {
      get 
      {
        return test;
      }
      set 
      {
        test = value;
      }
    }

    #region Solver Methods
    public int PassedTime 
    {
      get 
      {
        return test.Time;
      }
      set 
      {
        test.Time = value;
      }
    }

    public void proceedToNextQuestion() 
    {
      if (isNext()) 
      {
          currentQuestionNumber++;
      }    
    }

    public void returnToPreviousQuestion() 
    {
      if (isBack()) 
      {
        currentQuestionNumber--;
      }
    }

    public bool isNext() 
    {
      if (test.Questions.Length - 1 > currentQuestionNumber) return true;
      else return false;
    }

    public bool isBack() 
    {
      if (currentQuestionNumber > 0) return true;
      else return false;
    }

    public Question getCurrentQuestion() 
    {
      return test.Questions[currentQuestionNumber];
    }

    public int getCurrentQuestionNumber() 
    {
      return currentQuestionNumber;
    }

    public void saveSubmittedObjectiveAnswer(ObjectiveAnswer answer) 
    {
      ((ObjectiveQuestion)test.Questions[currentQuestionNumber]).Submitted = answer;
    }

    public void saveSubmittedDescriptiveAnswer(DescriptiveAnswer answer) 
    {
      ((DescriptiveQuestion)test.Questions[currentQuestionNumber]).Submitted = answer;
    }

    public void saveSubmittedActionBasedAnswer(ActionBasedAnswer answer) 
    {
      ((ActionBasedQuestion)test.Questions[currentQuestionNumber]).Submitted = answer;
    }

    public void timer_CallBack(object state) 
    {
      test.Time++;

      long temp;
      String hour, min, sec;
      temp = test.Time / 3600;
      if (temp < 10) hour = "0" + Convert.ToString(temp);
      else hour = Convert.ToString(temp);
      temp = test.Time % 3600 / 60;
      if (temp < 10) min = "0" + Convert.ToString(temp);
      else min = Convert.ToString(temp);
      temp = test.Time % 60;
      if (temp < 10) sec = "0" + Convert.ToString(temp);
      else sec = Convert.ToString(temp);
      TimeElapsed(this, new TestEventArgs(hour + ":" + min + ":" + sec));
      timer.Change(1000, Timeout.Infinite);            
    }

    public void startTimer() 
    {
      timer.Change(0, Timeout.Infinite);
    }

    public void stopTimer() 
    {
      timer.Change(Timeout.Infinite, Timeout.Infinite);
      test.Date = DateTime.Now;
    }

    public void setToStart() 
    {
      currentQuestionNumber = 0;
    }
    #endregion
  }
        
  // how many seconds have passed in timer events
  public sealed class TestEventArgs : EventArgs 
  {
    private String time;
    public TestEventArgs(String time) 
    {
      this.time = time;
    }
    public String Time 
    {
      get 
      {
        return this.time;
      }
    }
  }
}
