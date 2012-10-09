using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.user
{
  public abstract class User
  {
    protected int _userID;
    protected String _userName;
    protected String _userPassword;

    #region Properties
    public int UserID 
    {
      get 
      {
        return _userID;
      }

      set 
      {
        _userID = value;
      }
    }

    public String UserName
    {
      get
      {
        return _userName;
      }

      set
      {
        _userName = value;
      }
    }

    public String UserPassword
    {
      get
      {
        return _userPassword;
      }

      set
      {
        _userPassword = value;
      }
    }
    #endregion

    public User(int userID, String userName, String userPassword)
    {
      this._userID = userID;
      this._userName = userName;
      this._userPassword = userPassword;
    }
  }
}
