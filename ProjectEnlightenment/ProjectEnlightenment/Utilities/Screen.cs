using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ProjectEnlightenment.Utilities
{
  public sealed class Screen
  {
    //used by login and candidates when subject, freshness and test selection sequence starts
    public static bool[] getEmptySeen() 
    {
      bool[] seen = new bool[3];

      seen[0] = false;
      seen[1] = false;
      seen[2] = false;

      return seen;
    }
  }
}
