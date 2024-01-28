
namespace Reports
{
    partial class FormSettings
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSettings));
            this.textEditHost = new DevExpress.XtraEditors.TextEdit();
            this.textEditUser = new DevExpress.XtraEditors.TextEdit();
            this.textEditPassword = new DevExpress.XtraEditors.TextEdit();
            this.textEditPort = new DevExpress.XtraEditors.TextEdit();
            this.buttonOK = new DevExpress.XtraEditors.SimpleButton();
            this.buttonCancel = new DevExpress.XtraEditors.SimpleButton();
            this.textEditDatabase = new DevExpress.XtraEditors.TextEdit();
            this.gridControlHistorians = new DevExpress.XtraGrid.GridControl();
            this.gridViewHistorians = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.textEditHost.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditUser.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditPort.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditDatabase.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlHistorians)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewHistorians)).BeginInit();
            this.SuspendLayout();
            // 
            // textEditHost
            // 
            this.textEditHost.EditValue = "";
            this.textEditHost.Location = new System.Drawing.Point(12, 12);
            this.textEditHost.Name = "textEditHost";
            this.textEditHost.Size = new System.Drawing.Size(100, 20);
            this.textEditHost.TabIndex = 0;
            // 
            // textEditUser
            // 
            this.textEditUser.EditValue = "";
            this.textEditUser.Location = new System.Drawing.Point(12, 55);
            this.textEditUser.Name = "textEditUser";
            this.textEditUser.Size = new System.Drawing.Size(100, 20);
            this.textEditUser.TabIndex = 1;
            // 
            // textEditPassword
            // 
            this.textEditPassword.EditValue = "";
            this.textEditPassword.Location = new System.Drawing.Point(118, 55);
            this.textEditPassword.Name = "textEditPassword";
            this.textEditPassword.Size = new System.Drawing.Size(100, 20);
            this.textEditPassword.TabIndex = 2;
            // 
            // textEditPort
            // 
            this.textEditPort.EditValue = "";
            this.textEditPort.Location = new System.Drawing.Point(118, 12);
            this.textEditPort.Name = "textEditPort";
            this.textEditPort.Size = new System.Drawing.Size(100, 20);
            this.textEditPort.TabIndex = 3;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(262, 322);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(342, 322);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            // 
            // textEditDatabase
            // 
            this.textEditDatabase.EditValue = "";
            this.textEditDatabase.Location = new System.Drawing.Point(224, 12);
            this.textEditDatabase.Name = "textEditDatabase";
            this.textEditDatabase.Size = new System.Drawing.Size(100, 20);
            this.textEditDatabase.TabIndex = 6;
            // 
            // gridControlHistorians
            // 
            this.gridControlHistorians.EmbeddedNavigator.TextStringFormat = "Historian {0} of {1}";
            this.gridControlHistorians.Location = new System.Drawing.Point(13, 102);
            this.gridControlHistorians.MainView = this.gridViewHistorians;
            this.gridControlHistorians.Name = "gridControlHistorians";
            this.gridControlHistorians.Size = new System.Drawing.Size(400, 200);
            this.gridControlHistorians.TabIndex = 7;
            this.gridControlHistorians.UseEmbeddedNavigator = true;
            this.gridControlHistorians.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewHistorians});
            // 
            // gridViewHistorians
            // 
            this.gridViewHistorians.GridControl = this.gridControlHistorians;
            this.gridViewHistorians.Name = "gridViewHistorians";
            this.gridViewHistorians.OptionsView.ShowGroupPanel = false;
            // 
            // FormSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 354);
            this.Controls.Add(this.gridControlHistorians);
            this.Controls.Add(this.textEditDatabase);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textEditPort);
            this.Controls.Add(this.textEditPassword);
            this.Controls.Add(this.textEditUser);
            this.Controls.Add(this.textEditHost);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HelpButton = true;
            
            this.Name = "FormSettings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            ((System.ComponentModel.ISupportInitialize)(this.textEditHost.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditUser.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditPort.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditDatabase.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlHistorians)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewHistorians)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.TextEdit textEditHost;
        private DevExpress.XtraEditors.TextEdit textEditUser;
        private DevExpress.XtraEditors.TextEdit textEditPassword;
        private DevExpress.XtraEditors.TextEdit textEditPort;
        private DevExpress.XtraEditors.SimpleButton buttonOK;
        private DevExpress.XtraEditors.SimpleButton buttonCancel;
        private DevExpress.XtraEditors.TextEdit textEditDatabase;
        private DevExpress.XtraGrid.GridControl gridControlHistorians;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewHistorians;
    }
}