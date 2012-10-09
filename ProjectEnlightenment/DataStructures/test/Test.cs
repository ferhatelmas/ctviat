using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.question;

namespace Common.test
{
  public sealed class Test
  {
    private int _testID;
    private Question[] _questions;
    private int _subject;
    private int _takeOrder;
    private int _time;
    private bool _isNew;
    private int _startQuestionNo;
    private DateTime _date;

    #region Properties
    public int TestID 
    {
      get 
      {
        return _testID;
      }
      set 
      {
        _testID = value;
      }
    }

    public int Subject
    {
      get
      {
        return _subject;
      }
      set
      {
        _subject = value;
      }
    }
      
    public Question[] Questions 
    {
      get 
      {
        return _questions;
      }
      set 
      {
        _questions = value;
      }
    }

    public int TakeOrder
    {
      get
      {
        return _takeOrder;
      }
      set
      {
        _takeOrder = value;
      }
    }

    public int Time
    {
      get
      {
        return _time;
      }
      set
      {
        _time = value;
      }
    }

    public int StartQuestionNo
    {
      get
      {
        return _startQuestionNo;
      }
      set
      {
        _startQuestionNo = value;
      }
    }

    public bool IsNew
    {
      get
      {
        return _isNew;
      }
      set
      {
        _isNew = value;
      }
    }

    public DateTime Date 
    {
      get 
      {
        return _date;
      }
      set 
      {
        _date = value;
      }
    }
    #endregion

    public Test(int testID, Question[] questions, int subject, int takeOrder, 
      int time, bool isNew, int startQuestionNo, DateTime date) 
    {
      this._testID = testID;
      this._questions = questions;
      this._subject = subject;
      this._takeOrder = takeOrder;
      this._time = time;
      this._isNew = isNew;
      this._startQuestionNo = startQuestionNo;
      this._date = date;
    }

    public override string ToString()
    {
      return "Test " + TestID + " composed of " + _questions.Length + " questions";
    }
  }
}
