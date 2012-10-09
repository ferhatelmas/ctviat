using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;

namespace DAO.managers
{
  public sealed class ConnectionManager
  {
    public static void configureConnection(String targetdir, String conn)
    {
      Configuration c = ConfigurationManager.OpenExeConfiguration(targetdir);
      c.ConnectionStrings.ConnectionStrings.Clear();
      c.ConnectionStrings.ConnectionStrings
        .Add(new ConnectionStringSettings("DAO.Properties.Settings.CTVIATBankConnectionString", conn));
      c.Save();
    }
  }
}
