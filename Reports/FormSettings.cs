using System;
using Reports.Properties;

namespace Reports
{
    public partial class FormSettings : DevExpress.XtraEditors.XtraForm
    {
        public FormSettings()
        {
            InitializeComponent();
            this.Load += FormSettings_Load;
            buttonOK.Click += ButtonOK_Click;
            buttonCancel.Click += ButtonCancel_Click;
            textEditHost.Properties.UseAdvancedMode = DevExpress.Utils.DefaultBoolean.True;
            textEditHost.Properties.AdvancedModeOptions.Label = "Host";
            textEditPort.Properties.UseAdvancedMode = DevExpress.Utils.DefaultBoolean.True;
            textEditPort.Properties.AdvancedModeOptions.Label = "Port";
            textEditDatabase.Properties.UseAdvancedMode = DevExpress.Utils.DefaultBoolean.True;
            textEditDatabase.Properties.AdvancedModeOptions.Label = "Database";
            textEditUser.Properties.UseAdvancedMode = DevExpress.Utils.DefaultBoolean.True;
            textEditUser.Properties.AdvancedModeOptions.Label = "Username";
            textEditPassword.Properties.UseAdvancedMode = DevExpress.Utils.DefaultBoolean.True;
            textEditPassword.Properties.AdvancedModeOptions.Label = "Password";
        }

        private void ButtonCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
        private void FormSettings_Load(object sender, System.EventArgs e)
        {
            textEditHost.Text = (string)Settings.Default["Host"];
            textEditPort.Text = Settings.Default["Port"].ToString();
            textEditUser.Text = (string)Settings.Default["User"];
            textEditPassword.Text = (string)Settings.Default["Password"];
            textEditDatabase.Text = (string)Settings.Default["Database"];
        }
        private void ButtonOK_Click(object sender, System.EventArgs e)
        {
            Settings.Default["Host"] = textEditHost.Text;
            Settings.Default["Port"] = Convert.ToInt32(textEditPort.Text);
            Settings.Default["User"]=textEditUser.Text;
            Settings.Default["Password"] = textEditPassword.Text;
            Settings.Default["Database"] = textEditDatabase.Text;
            this.Close();
        }

      
    }
}