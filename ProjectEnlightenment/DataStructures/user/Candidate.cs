using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.user
{
  public sealed class Candidate : User
  {        
    public Candidate(int userID, String userName, String userPassword) 
      : base(userID, userName, userPassword) 
    { }

    public override String ToString() 
    {
      return UserName;
    }            
  }
}
