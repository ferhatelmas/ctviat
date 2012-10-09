using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.user
{
  public sealed class Admin : User
  {
    public Admin(int userID, String userName, String userPassword) 
      : base(userID, userName, userPassword) 
    { }
  }
}
