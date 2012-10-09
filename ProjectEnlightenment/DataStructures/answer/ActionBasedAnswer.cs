using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.answer
{
  public sealed class ActionBasedAnswer : Answer
  {
    private byte _answer;

    public ActionBasedAnswer(byte answer) : base() 
    {
      this._answer = answer;
    }

    public byte Answer 
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
