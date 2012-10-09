using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;

namespace ProjectEnlightenment
{
  partial class Installer
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
          components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      components = new System.ComponentModel.Container();
    }

    #endregion

    public override void Install(IDictionary stateSaver)
    {
        base.Install(stateSaver);
        String t = Context.Parameters["targetdir"];
        String param1 = Context.Parameters["Param1"];
        Configuration c = ConfigurationManager
        .OpenExeConfiguration(String.Format("{0}ProjectEnlightenment.exe", t));
        //c.ConnectionStrings.ConnectionStrings.Clear();
        c.ConnectionStrings
          .ConnectionStrings["DAO.Properties.Settings.CTVIATBankConnectionString"]
          .ConnectionString = param1;
        c.Save(ConfigurationSaveMode.Modified);
    }
  }
}
    