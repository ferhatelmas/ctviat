using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.hint
{
  public sealed class Hint
  {
    private int _hintID;

    public int HintID 
    {
      get 
      {
        return _hintID;
      }
      set 
      {
        _hintID = value;
      }
    }

    public Hint(int hintID) 
    {
        this._hintID = hintID;
    }
  }
}
