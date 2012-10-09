using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.hint;
using Common.answer;

namespace Common.question
{
  public class ActionBasedQuestion : Question
  {
    private int _type;
    private ActionBasedAnswer _submitted;
    private String _parameter1;
    private String _parameter2;

    public ActionBasedQuestion(String question, int type, String parameter1, 
      String parameter2, int questionId, int questionSubject) 
            : base(question, questionId, questionSubject) 
    {
      this._type = type;
      this._submitted = null;
      this._parameter1 = parameter1;
      this._parameter2 = parameter2;
    }

    #region Properties
    public ActionBasedAnswer Submitted 
    {
      get 
      {
        return _submitted;
      }
      set 
      {
        _submitted = value;
      }
    }

    public int Type 
    {
      get 
      {
        return _type;
      }
    }

    public String Parameter1 
    {
      get 
      {
        return _parameter1;
      }
    }

    public String Parameter2
    {
      get
      {
        return _parameter2;
      }
    }
    #endregion

    public override String ToString() 
    {
      return "A-" + this.QuestionText;   
    }
  }
}
