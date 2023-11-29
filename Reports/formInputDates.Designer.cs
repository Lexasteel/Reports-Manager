namespace Reports
{
    partial class formInputDates
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formInputDates));
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.dateTimeChartRangeControlClient1 = new DevExpress.XtraEditors.DateTimeChartRangeControlClient();
            this.DateEditStart = new DevExpress.XtraEditors.DateEdit();
            this.DateEditEnd = new DevExpress.XtraEditors.DateEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.chkSingle = new DevExpress.XtraEditors.CheckEdit();
            ((System.ComponentModel.ISupportInitialize)(this.dateTimeChartRangeControlClient1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DateEditStart.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DateEditStart.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DateEditEnd.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DateEditEnd.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSingle.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(26, 149);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(101, 30);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "ОК";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(180, 149);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(101, 30);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // DateEditStart
            // 
            this.DateEditStart.EditValue = new System.DateTime(2020, 1, 9, 21, 58, 41, 0);
            this.DateEditStart.Location = new System.Drawing.Point(105, 24);
            this.DateEditStart.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.DateEditStart.Name = "DateEditStart";
            this.DateEditStart.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DateEditStart.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DateEditStart.Properties.DisplayFormat.FormatString = "dd.MM.yyyy HH:mm:ss";
            this.DateEditStart.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.DateEditStart.Properties.EditFormat.FormatString = "dd.MM.yy HH:mm:ss";
            this.DateEditStart.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.DateEditStart.Properties.MaskSettings.Set("mask", "dd.MM.yyyy HH:mm:ss");
            this.DateEditStart.Size = new System.Drawing.Size(176, 22);
            this.DateEditStart.TabIndex = 0;
            // 
            // DateEditEnd
            // 
            this.DateEditEnd.EditValue = null;
            this.DateEditEnd.Location = new System.Drawing.Point(105, 65);
            this.DateEditEnd.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.DateEditEnd.Name = "DateEditEnd";
            this.DateEditEnd.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DateEditEnd.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DateEditEnd.Properties.DisplayFormat.FormatString = "dd.MM.yyyy HH:mm:ss";
            this.DateEditEnd.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.DateEditEnd.Properties.EditFormat.FormatString = "dd.MM.yy HH:mm:ss";
            this.DateEditEnd.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.DateEditEnd.Properties.MaskSettings.Set("mask", "dd.MM.yyyy HH:mm:ss");
            this.DateEditEnd.Size = new System.Drawing.Size(176, 22);
            this.DateEditEnd.TabIndex = 1;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(26, 26);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(53, 16);
            this.labelControl1.TabIndex = 4;
            this.labelControl1.Text = "Начало:";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(26, 68);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(76, 16);
            this.labelControl2.TabIndex = 5;
            this.labelControl2.Text = "Окончание:";
            // 
            // chkSingle
            // 
            this.chkSingle.Location = new System.Drawing.Point(65, 109);
            this.chkSingle.Margin = new System.Windows.Forms.Padding(4);
            this.chkSingle.Name = "chkSingle";
            this.chkSingle.Properties.Caption = "Формировать один файл";
            this.chkSingle.Size = new System.Drawing.Size(186, 20);
            this.chkSingle.TabIndex = 2;
            this.chkSingle.Visible = false;
            // 
            // formInputDates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(325, 196);
            this.Controls.Add(this.chkSingle);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.DateEditEnd);
            this.Controls.Add(this.DateEditStart);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.IconOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("formInputDates.IconOptions.SvgImage")));
            this.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formInputDates";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Период отчета";
            this.Load += new System.EventHandler(this.frmInputDates_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dateTimeChartRangeControlClient1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DateEditStart.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DateEditStart.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DateEditEnd.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DateEditEnd.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSingle.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.DateTimeChartRangeControlClient dateTimeChartRangeControlClient1;
        private DevExpress.XtraEditors.DateEdit DateEditStart;
        private DevExpress.XtraEditors.DateEdit DateEditEnd;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.CheckEdit chkSingle;
    }
}