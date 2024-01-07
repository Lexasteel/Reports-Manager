using System;
using System.Data;
using System.Drawing.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Models;
using Reports.Properties;

namespace Reports
{
    public partial class FormSettings : DevExpress.XtraEditors.XtraForm
    {
        public FormSettings(IDbConnection connection)
        {
            InitializeComponent();
            this.Load += FormSettings_LoadAsync;
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
            _connection = connection;
            gridControlHistorians.EmbeddedNavigator.ButtonClick += EmbeddedNavigator_ButtonClick;
            buttonOK.Click += ButtonOK_Click1;
        }

        private void ButtonOK_Click1(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void EmbeddedNavigator_ButtonClick(object sender, DevExpress.XtraEditors.NavigatorButtonClickEventArgs e)
        {

        }

        private readonly IDbConnection _connection;
        private readonly BindingSource _bindingSourceHistorian = new BindingSource();
        private void ButtonCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
        private async void FormSettings_LoadAsync(object sender, System.EventArgs e)
        {
            textEditHost.Text = (string)Settings.Default["Host"];
            textEditPort.Text = Settings.Default["Port"].ToString();
            textEditUser.Text = (string)Settings.Default["User"];
            textEditPassword.Text = (string)Settings.Default["Password"];
            textEditDatabase.Text = (string)Settings.Default["Database"];
            var historians = await Historian.GetAll(_connection);
            _bindingSourceHistorian.DataSource = historians;
            gridControlHistorians.DataSource = _bindingSourceHistorian;
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