using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.answer;

namespace Common.question
{
  public abstract class Question
  {
    protected String _question;
    protected int _questionID;
    protected int _questionSubject;

    public Question(String question, int questionID, int questionSubject) 
    {
      this._question = question;
      this._questionID = questionID;
      this._questionSubject = questionSubject;
    }

    #region Properties
    public String QuestionText 
    {
      get 
      {
        return _question;
      }
      set 
      {
        _question = value;
      }
    }

    public int QuestionID 
    {
      get 
      {
        return _questionID;
      }
      set
      {
        _questionID = value;
      }
    }

    public int QuestionSubject 
    {
      get 
      {
        return _questionSubject;
      }
      set 
      {
        _questionSubject = value;
      }
    }
    #endregion
  }
}
