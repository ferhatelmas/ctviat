using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.answer;

namespace Common.question
{
  public sealed class DescriptiveQuestion : Question
  {
    private DescriptiveAnswer _answer;
    private DescriptiveAnswer _submitted;

    public DescriptiveQuestion(String question, DescriptiveAnswer answer, 
      int questionID, int questionSubject) 
            : base(question, questionID, questionSubject) 
    {
      this._answer = answer;
      this._submitted = null;
    }
    
    #region Properties
    public DescriptiveAnswer Answer 
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

    public DescriptiveAnswer Submitted 
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
      return "D-" + this.QuestionText;   
    }
  }
}
