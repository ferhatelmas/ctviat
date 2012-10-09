using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.answer
{
  public sealed class DescriptiveAnswer : Answer
  {
    private String _answer;

    public DescriptiveAnswer(String answer) : base() 
    {
      this._answer = answer;
    }

    public String Answer 
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
  }
}
