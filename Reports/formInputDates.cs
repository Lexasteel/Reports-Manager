using System;

namespace Reports
{
    public partial class formInputDates : DevExpress.XtraEditors.XtraForm
    {
        public formInputDates()
        {
            InitializeComponent();

        }

        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public bool SingleFile { get; set; }
        private void btnOK_Click(object sender, EventArgs e)
        {
            DateStart = DateEditStart.DateTime;
            DateEnd = DateEditEnd.DateTime;
            SingleFile = chkSingle.Checked;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmInputDates_Load(object sender, EventArgs e)
        {
            if (System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToLower() == "web-kutes\\user")
            {
                DateEditStart.DateTime = new DateTime(2020, 1, 1, 0, 0, 0);
                DateEditEnd.DateTime = new DateTime(2020, 2, 1, 0, 0, 0);
            }
            else
            {
                DateEditStart.EditValue = DateStart;
                DateEditEnd.EditValue = DateEnd;
            }
        }
    }
}