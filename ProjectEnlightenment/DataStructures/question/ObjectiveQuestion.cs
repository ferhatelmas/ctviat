using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.answer;

namespace Common.question
{
  public sealed class ObjectiveQuestion : Question
  {
    private String[] _choices;
    private ObjectiveAnswer _answer;
    private ObjectiveAnswer _submitted;

    public ObjectiveQuestion(String question, String[] choices, 
      ObjectiveAnswer answer, int questionID, int questionSubject) 
            : base(question, questionID, questionSubject) 
    {
      this._choices = choices;
      this._answer = answer;
      this._submitted = null;
    }

    #region Properties
    public String[] Choices 
    {
      get 
      {
        return _choices;
      }
      set 
      {
        _choices = value;
      }
    }

    public ObjectiveAnswer Answer
    {
      get
      {
        return _answer;
      }
      set
      {
        _answer = value;
      }
    }

    public ObjectiveAnswer Submitted
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
    #endregion

    public override String ToString() 
    {
      return "O-" + this.QuestionText;    
    }
  }
}
