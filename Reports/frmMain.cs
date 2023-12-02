using AdapterOPH;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Models;
using NLog;


using Reports.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Reports
{
    public partial class FrmMain : XtraForm
    {
        public FrmMain()
        {
            InitializeComponent();
            this.Load += FrmMain_Load;
            this.FormClosing += FrmMain_FormClosing;

            timer1.Tick += Timer1_Tick;
            gViewMain.FocusedRowChanged += GViewMain_FocusedRowChanged;
            gViewMain.DoubleClick += GwMain_DoubleClick;
            barButtonNew.ItemClick += BarButtonNew_ItemClick;
            barButtonEdit.ItemClick += GwMain_DoubleClick;

            barButtonCopy.ItemClick += OnCopyRowClick;
            barButtonDelete.ItemClick += OnDeleteRowClickAsync;

            barButtonGenerate.ItemClick += OnGenerateRowClick;

            barButtonTimerStart.ItemClick += BarButtonTimerStart_ItemClick;
            barButtonTimerStop.ItemClick += BarButtonTimerStop_ItemClick;

            barButtonSaveOptions.ItemClick += BarButtonSaveOptions_ItemClick;
            barButtonSetting.ItemClick += BarButtonSetting_ItemClick;

            btnEditDestinationInfo.ButtonClick += BtnEditDestinationInfo_ButtonClick;
            lookUpTimeFormat.EditValueChanged += LookUpTimeFormat_EditValueChanged;
            lookUpSample.EditValueChanged += LookUpSample_EditValueChanged;

            btnAddPoint.Click += BtnAddPoint_Click;
            btnRemovePoint.Click += BtnRemovePoint_Click;
            btnUpPoint.Click += BtnUpPoint_Click;
            btnDownPoint.Click += BtnDownPoint_Click;
            btnImportPoints.Click += BtnImportPoints_Click;
            btnExportPoints.Click += BtnExportPoints_Click;

            btnGetDesc.Click += BtnGetDesc_Click;
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;

        }

        private void BarButtonSetting_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var frmSettings = new FormSettings();
            frmSettings.ShowDialog();
        }

        private const string Version = "Менеджер отчетов 3.0";
        private const int Panel2Width = 518;
        private int _cycleSet = 5;
        private int _cycleTimer = 0;
        private readonly string _fileName = Application.StartupPath + "\\Grid_Layout.xml";
        private enum State
        {
            View,
            Edit,
            New
        }
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly BindingSource _bindingSourceMain = new BindingSource();
        private List<int> _deletedHistpoints = new List<int>();
        private List<HistPoint> _importedHistpoints = new List<HistPoint>();
        private readonly BindingSource _bindingSourceDetail = new BindingSource();
        //static CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();
        //CancellationToken _token = _cancelTokenSource.Token;

        private async void DataBinding(bool reset = false)
        {
            var reports = await ReportDefinition.GetAll(DataConnection());
            _bindingSourceMain.DataSource = reports.OrderBy(o => o.reportdefinitionid).ToList();
            gridConrolMain.DataSource = _bindingSourceMain;
            if (reset) return;
            txtReportName.DataBindings.Add("EditValue", _bindingSourceMain, "reportname");
            cmbUnit.DataBindings.Add("EditValue", _bindingSourceMain, "unit");
            btnEditDestinationInfo.DataBindings.Add("EditValue", _bindingSourceMain, "destinationinfo");
            deNextEvent.DataBindings.Add("EditValue", _bindingSourceMain, "nextevent", true);
            chkEnable.DataBindings.Add("EditValue", _bindingSourceMain, "enable", true);
            tsOffSet.DataBindings.Add("EditValue", _bindingSourceMain, "shift", true);
            chZip.DataBindings.Add("EditValue", _bindingSourceMain, "arhive", true);


            LookUpEdit_DataBinding(lkpReportType, Dictionares.ReportType, "reporttypeid");
            LookUpEdit_DataBinding(lkpReportDest, Dictionares.DestinationType, "reportdestid");
            LookUpEdit_DataBinding(lookUpTimeFormat, Dictionares.TimeType, "timeformatid");
            LookUpEdit_DataBinding(lookUpSample, Dictionares.TimeType, "sampletimeformatid");

            chCmbHeader.Properties.EditValueType = EditValueTypeCollection.CSV;
            chCmbHeader.DataBindings.Add(new Binding("EditValue", _bindingSourceMain, "header2"));
            chCmbHeader.Properties.DataSource = Dictionares.Headers;
            chCmbHeader.Properties.ValueMember = "Value";
            chCmbHeader.Properties.DisplayMember = "Description";
            chCmbHeader.Properties.DropDownRows = Dictionares.Headers.Count;
            chCmbHeader.Properties.PopupFormMinSize = new Size(10, 10);
            chCmbHeader.Properties.PopupWidthMode = PopupWidthMode.ContentWidth;
            
            
            chCmbHeader.SetEditValue(((ReportDefinition)_bindingSourceMain.Current).header2);
            var id = (int)gViewMain.GetFocusedRowCellValue("reportdefinitionid");
            Select_Detail(id, State.View);
            timer1.Interval = 1000;
            Timer_Start();
        }
        private async void FrmMain_Load(object sender, EventArgs e)
        {
            InitgViewMain();
            InitgViewDetail();
            this.Text = Version;
            var h = (int)Settings.Default["Heigth"];
            var w = (int)Settings.Default["Width"];


            this.ClientSize = new Size(w, h);
            Screen myScreen = Screen.FromControl(this);
            Rectangle area = myScreen.WorkingArea;

            this.Top = (area.Height - this.Height) / 2;
            this.Left = (area.Width - this.Width) / 2;
            this.StartPosition = FormStartPosition.CenterScreen;
            _cycleSet = (int)Settings.Default["Period"];
            barSpinCycle.EditValue = _cycleSet;

            DataBinding();

            gridConrolMain.ForceInitialize();

            barStaticInfo.Caption = "";
            barStaticTime.Caption = "";

            gViewMain.PopupMenuShowing += gridViewMain_PopupMenuShowing;
            splitContainerControl1.FixedPanel = SplitFixedPanel.Panel2;

            splitContainerControl1.Panel2.MinSize = Panel2Width;
            splitContainerControl1.Panel2.Size = new Size(Panel2Width, splitContainerControl1.Panel2.Size.Height);
            barButtonTimerStart.Enabled = false;
            barProgress.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;

        }

        private void LookUpEdit_DataBinding(LookUpEdit sender, IReadOnlyCollection<Item> items, string dataMember = null)
        {
            sender.Properties.DataSource = items;
            sender.Properties.DropDownRows = items.Count;
            sender.Properties.DisplayMember = "Value";
            sender.Properties.ValueMember = "Id";
            if (dataMember != null)
            {
                sender.DataBindings.Add(new Binding("EditValue", _bindingSourceMain, dataMember));
            }
            sender.Properties.PopulateColumns();
            sender.Properties.Columns[0].Visible = false;
            sender.Properties.ShowHeader = false;
            sender.Properties.ShowFooter = false;
            sender.Properties.PopupFormMinSize = new Size(10, 10);
            sender.Properties.PopupWidthMode = PopupWidthMode.ContentWidth;
        }
        private void GViewMain_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0 || e.PrevFocusedRowHandle < 0) return;
            int id = 0;
            var rowText = gViewMain.GetRowCellDisplayText(e.FocusedRowHandle, "reportdefinitionid");
            id = int.Parse(rowText);
            Select_Detail(id, State.View);
        }
        private void GwMain_DoubleClick(object sender, EventArgs e)
        {
            StateSwitch();
        }
        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            var dialog = XtraMessageBox.Show("Закрыть программу?", "Менеджер отчетов", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            switch (dialog)
            {
                case DialogResult.Yes:
                    Application.ExitThread();
                    break;
                case DialogResult.No:
                    e.Cancel = true;
                    return;
                default:
                    return;
            }
        }
        private void BarButtonExit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Application.ExitThread();
        }
        #region Detail
        private async void Select_Detail(int id, State state)
        {
            var report = (ReportDefinition)_bindingSourceMain.Current;
            var points = await ReportDefinition.GetHistPoints(DataConnection(), report.reportdefinitionid);
            report.HistPoints = report.reportdefinitionid > 0
                ? points
                : new List<HistPoint>();
            _bindingSourceDetail.DataSource = report.HistPoints;
            gridControlDetail.DataSource = _bindingSourceDetail;
            gViewDetail.FocusedRowHandle = 0;
            lblTotalPoints.Text = $@"Кол-во: {report.HistPoints.Count}";
            if (state == State.Edit || state == State.New)
            {
                StateSwitch();
            }
            GetTimeFormat(report);
            
        }
        private void GetTimeFormat(ReportDefinition report)
        {
            switch (report.sampletimeformatid)
            {
                case 1:
                    spinSampleHours.EditValue = report.sampletimeperiodinfo.Split(':')[0];
                    spinSampleMins.EditValue = report.sampletimeperiodinfo.Split(':')[1];
                    spinSampleSecs.EditValue = report.sampletimeperiodinfo.Split(':')[2];
                    break;
                case 6:
                    cmbFractionOfSec.EditValue = report.sampletimeperiodinfo;
                    break;
                default:
                    spinSampleHours.EditValue = report.sampletimeperiodinfo.Split(':')[0];
                    break;
            }
            switch (report.timeformatid)
            {
                case 1:
                    spinTimeHours.EditValue = report.timeperiodinfo.Split(':')[0];
                    spinTimeMinutes.EditValue = report.timeperiodinfo.Split(':')[1];
                    spinTimeSecs.EditValue = report.timeperiodinfo.Split(':')[2];
                    break;
                default:
                    spinTimeHours.EditValue = report.timeperiodinfo.Split(':')[0];
                    break;
            }

        }
        private void StateSwitch()
        {

            gridConrolMain.Enabled = !gridConrolMain.Enabled;

            grpGeneral.Enabled = !grpGeneral.Enabled;
            gridControlDetail.Enabled = true;
            gViewDetail.OptionsBehavior.Editable = !gViewDetail.OptionsBehavior.Editable;

            barButtonEdit.Enabled = !barButtonEdit.Enabled;
            btnSave.Enabled = !btnSave.Enabled;
            btnCancel.Enabled = !btnCancel.Enabled;
            btnGetDesc.Enabled = !btnGetDesc.Enabled;
            btnAddPoint.Enabled = !btnAddPoint.Enabled;
            btnRemovePoint.Enabled = !btnRemovePoint.Enabled;
            btnUpPoint.Enabled = !btnUpPoint.Enabled;
            btnDownPoint.Enabled = !btnDownPoint.Enabled;
            btnImportPoints.Enabled = !btnImportPoints.Enabled;
            btnExportPoints.Enabled = !btnExportPoints.Enabled;
            lblTotalPoints.Enabled = !lblTotalPoints.Enabled;

        }
        private async void BtnCancel_Click(object sender, EventArgs e)
        {
            var report = (ReportDefinition)_bindingSourceMain.Current;
            var dialogResult = DialogResult.No;
            var reportDb = await ReportDefinition.GetById(DataConnection(), report.reportdefinitionid);
            var changesReport = (reportDb != null && !report.Equals(reportDb));
            var changesHistPoints = Task.Run(HistPointCompare).Result;


            if (changesReport || changesHistPoints.Count > 0)
            {
                dialogResult = MessageBox.Show("Save changes?", "Report Manager", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }

            StateSwitch();
            if (dialogResult == DialogResult.Yes)
            {
                BtnSave_Click(sender, e);
                return;
            }

            DataBinding(true);
            GetTimeFormat(report);
            Select_Detail(report.reportdefinitionid, State.View);
        }

        public async Task<List<HistPoint>> HistPointCompare()
        {
            var report = (ReportDefinition)_bindingSourceMain.Current;
            var gridList = (List<HistPoint>)_bindingSourceDetail.List;
            var listDb = await ReportDefinition.GetHistPoints(DataConnection(), report.reportdefinitionid);
            var result = gridList.Except(listDb).ToList();
            return result;
        }
        private async void BtnSave_Click(object sender, EventArgs e)
        {
            var report = (ReportDefinition)_bindingSourceMain.Current;
            var id = report.reportdefinitionid;
            switch (report.timeformatid)
            {
                case 1:
                    string[] info = { spinTimeHours.EditValue.ToString(), spinTimeMinutes.EditValue.ToString(), spinTimeSecs.EditValue.ToString() };
                    report.timeperiodinfo = string.Join(":", info);
                    break;
                default:
                    report.timeperiodinfo = spinTimeHours.EditValue.ToString();
                    break;
            }

            switch (report.sampletimeformatid)
            {
                case 1:
                    string[] info = { spinSampleHours.EditValue.ToString(), spinSampleMins.EditValue.ToString(), spinSampleSecs.EditValue.ToString() };
                    report.sampletimeperiodinfo = string.Join(":", info);
                    break;
                case 6:
                    report.sampletimeperiodinfo = cmbFractionOfSec.EditValue.ToString();
                    break;
                default:
                    report.sampletimeperiodinfo = spinSampleHours.EditValue.ToString();
                    break;
            }

            if (report.HistPoints == null)
            {
                report.HistPoints = await ReportDefinition.GetHistPoints(DataConnection(), id);
            }

            if (report.HistPoints != null)
            {
                lblTotalPoints.Text = $@"Кол-во: {report.HistPoints.Count}";
            }

            StateSwitch();

            report.header2 = chCmbHeader.EditValue.ToString();

            if (id == 0)
            {
                var reportDefId = await ReportDefinition.Insert(DataConnection(), report);
                if (report.HistPoints != null)
                    for (var i = 0; i < report.HistPoints.Count; i++)
                    {
                        report.HistPoints[i].pointposn = i + 1;
                        report.HistPoints[i].reportdefinitionid = reportDefId;
                        await HistPoint.Insert(DataConnection(), report.HistPoints[i]);
                    }

                return;
            }




            var changes = Task.Run(HistPointCompare).Result;

            if (changes.Count > 0)
            {
                foreach (var p in changes)
                {
                    if (p.reportdefinitionid == 0)
                    {
                        p.reportdefinitionid = report.reportdefinitionid;
                        var i = await HistPoint.Insert(DataConnection(), p);

                    }
                    await HistPoint.Update(DataConnection(), p);
                }
            }

            if (!report.Equals(await ReportDefinition.GetById(DataConnection(), report.reportdefinitionid)))
            {
                await ReportDefinition.Update(DataConnection(), report);
            }


            if (_deletedHistpoints.Count > 0)
            {
                var delHistPoints = new List<int>(_deletedHistpoints);
                foreach (var p in delHistPoints)
                {
                    await HistPoint.Delete(DataConnection(), p);
                    _deletedHistpoints.Remove(p);
                }
            }


        }
        private void BtnGetDesc_Click(object sender, EventArgs e)
        {
            var report = (ReportDefinition)_bindingSourceMain.Current;
            if (report.reporttypeid != 1) return;
            var date = DateTime.Now;
            var target = CreateTarget(report, date, date).Result;
            target.GetAttr();
            gViewDetail.RefreshData();
        }
        private void BtnEditDestinationInfo_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            string file = btnEditDestinationInfo.EditValue.ToString();
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = @"Excel Workbook(*.xlsx)|*.xlsx|Excel binary Workbook(*.xlsb)|*.xlsb|CSV files(*.csv)|*.csv|All files(*.*)|*.*"
            };

            if (!string.IsNullOrEmpty(file))
            {
                saveFileDialog.FileName = Path.GetFileName(file);
                saveFileDialog.InitialDirectory = Path.GetDirectoryName(file);
            }
            switch (Path.GetExtension(file))
            {
                case ".xlsx":
                    saveFileDialog.FilterIndex = 1;
                    break;
                case ".xlsb":
                    saveFileDialog.FilterIndex = 2;
                    break;
                case ".csv":
                    saveFileDialog.FilterIndex = 3;
                    break;
                default:
                    saveFileDialog.FilterIndex = 4;
                    break;
            }


            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                btnEditDestinationInfo.Text = saveFileDialog.FileName;
            }
        }
        private void LookUpTimeFormat_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit lookUpEdit = (LookUpEdit)sender;
            if (lookUpEdit.EditValue is DBNull) return;
            int timeType = Convert.ToInt32(lookUpEdit.EditValue);

            switch (timeType)
            {
                case 1:
                    spinTimeMinutes.Visible = true;
                    spinTimeSecs.Visible = true;
                    break;
                default:
                    spinTimeMinutes.Visible = false;
                    spinTimeSecs.Visible = false;

                    break;
            }



        }
        private void LookUpSample_EditValueChanged(object sender, EventArgs e)
        {
            var lookUpEdit = sender as LookUpEdit;
            if (lookUpEdit?.EditValue is DBNull) return;
            var timeType = Convert.ToInt32(lookUpEdit.EditValue);
            var arr = VisibilitySampleIntervals(timeType);
            spinSampleHours.Visible = arr[0];
            spinSampleMins.Visible = arr[1];
            spinSampleSecs.Visible = arr[2];
            cmbFractionOfSec.Visible = arr[3];

            switch (timeType)
            {
                case 6:
                    cmbFractionOfSec.SelectedIndex = 0;
                    break;
                default:
                    break;
            }
        }
        public static bool[] VisibilitySampleIntervals(int timeType)
        {
            var showFracasOfSec = true;
            var showHours = true;
            var showMinutes = true;
            var showSecs = true;
            switch (timeType)
            {
                case 1:
                    showFracasOfSec = false;
                    break;
                case 6:
                    showHours = false;
                    showMinutes = false;
                    showSecs = false;
                    break;
                default:
                    showMinutes = false;
                    showSecs = false;
                    showFracasOfSec = false;
                    break;
            }
            var array = new bool[] { showHours, showMinutes, showSecs, showFracasOfSec };
            return array;
        }
        #endregion
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (_cycleSet >= _cycleTimer)
            {
                barStaticTime.Caption = $"Цикл: {(_cycleSet - _cycleTimer)} сек";
                _cycleTimer++;
                return;
            }
            _cycleTimer = 0;
            Timer_Stop(true);
            TaskPerform();
        }
        private async void TaskPerform()
        {
            var now = DateTime.Now;
            var list = ((List<ReportDefinition>)_bindingSourceMain.List).Where(w =>
                w.enable == true && w.nextevent < now).ToList();
            if (!list.Any())
            {
                Timer_Start();
                return;
            }
            foreach (var autoReport in list.Where(autoReport => autoReport.nextevent != null))
            {
                while (autoReport.nextevent < DateTime.Now)
                {
                    var end = autoReport.nextevent.Value;
                    var start = HelpersAdapter.DateCalc(autoReport.nextevent.Value, autoReport.timeperiodinfo, autoReport.timeformatid, true);
                    var target = await Task.Run(() => CreateTarget(autoReport, start, end));
                    SwitchWhenGenerate();
                    var result = await Task.Run(() => target.Generate());
                    SwitchWhenGenerate(true);
                    if (!result)
                    {
                        break;
                    }
                    autoReport.nextevent = HelpersAdapter.DateCalc(autoReport.nextevent.Value, autoReport.timeperiodinfo, autoReport.timeformatid);
                    autoReport.lastused = DateTime.Now;
                    await ReportDefinition.Update(DataConnection(), autoReport);
                    // this.Invoke(new Action(() => gViewMain.RefreshData()));
                }
            }
            Timer_Start();
        }
        private async Task<IReport> CreateTarget(ReportDefinition report, DateTime start, DateTime end)
        {
            var id = report.reportdefinitionid;

            report.HistPoints = await ReportDefinition.GetHistPoints(DataConnection(), report.reportdefinitionid);
            report.Historians = await Historian.GetAll(DataConnection());

            switch (report.reporttypeid)
            {
                //case 1: //simple
                //   // return new SimpleReport(report, hist);
                //    break;
                case 2: //operator events
                    break;
                case 3: //alarms
                    break;
                case 4: //RAW
                    var r = new RawReport(report, start, end);
                    r.ReportChanged += Report_ReportChanged;
                    return r;
            }
            var s = new SimpleReport(report, start, end);
            s.ReportChanged += Report_ReportChanged;
            return s;
        }
        private void Report_ReportChanged(object sender, ReportEventArgs e)
        {
            var report = (IReport)sender;
            BeginInvoke(new Action(() =>
            {
                barProgress.EditValue = e.Value;
            }));
            barStaticInfo.Caption = $"{report.Report.reportname}: {report.Start.ToString()} по {report.End.ToString()}: {e.Message}";

        }
        private void BarButtonNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gViewMain.AddNewRow();

            Select_Detail(GridControl.NewItemRowHandle, State.New);

        }
        private async void OnCopyRowClick(object sender, EventArgs e)
        {
            var report = (ReportDefinition)_bindingSourceMain.Current;

            var id = await ReportDefinition.Insert(DataConnection(), report);
            var newReport = await ReportDefinition.GetById(DataConnection(), id);
            newReport.reportname += " - Копия";
            await ReportDefinition.Update(DataConnection(), newReport);

            var points = _bindingSourceDetail.List;
            foreach (HistPoint item in points)
            {
                item.reportdefinitionid = newReport.reportdefinitionid;
                await HistPoint.Insert(DataConnection(), item);
            }
            DataBinding(true);

        }

        private void SwitchWhenGenerate(bool enabled = false)
        {
            barButtonGenerate.Enabled = enabled;

        }
        private async void OnGenerateRowClick(object sender, EventArgs eventArgs)
        {
            using (var frmInput = new FormInputDates())
            {

                Timer_Stop(true);
                var date = DateTime.Now;
                var report = (ReportDefinition)_bindingSourceMain.Current;
                frmInput.DateEnd = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                frmInput.DateStart = frmInput.DateEnd.Subtract(TimeSpan.Parse(report.timeperiodinfo));
                if (frmInput.ShowDialog() != DialogResult.OK) return;
                var target = await Task.Run(() => CreateTarget(report, frmInput.DateStart, frmInput.DateEnd));
                target.MultiSheets = frmInput.SingleFile;
                SwitchWhenGenerate();
                var result = await Task.Run(() => target.Generate());
                SwitchWhenGenerate(true);
                if (!result) return;
                report.lastused = DateTime.Now;
                await ReportDefinition.Update(DataConnection(), report);
            }
            this.Invoke(new Action(() => gViewMain.RefreshData()));
            Timer_Start();
        }
        private async void OnDeleteRowClickAsync(object sender, EventArgs e)
        {

            var handles = gViewMain.GetSelectedRows();
            var text = "Удаляем: ";
            foreach (var item in handles)
            {
                text += gViewMain.GetRowCellValue(item, "reportname");
                if (item != handles.Last())
                {
                    text += ", ";
                }
            }
            text += "?";
            var dialogResult = XtraMessageBox.Show(text, "Подтверждение удаления", MessageBoxButtons.YesNo);
            if (dialogResult != DialogResult.Yes) return;
            foreach (var item in handles)
            {
                var name = gViewMain.GetRowCellValue(item, "reportname");
                _log.Info($"Отчет {name} удален!");
                var id = (int)gViewMain.GetRowCellValue(item, "reportdefinitionid");
                await ReportDefinition.Delete(DataConnection(), id);
                var points = await ReportDefinition.GetHistPoints(DataConnection(), id);
                foreach (var point in points.Select(s => s.histpointid))
                {
                    await HistPoint.Delete(DataConnection(), point);
                }

            }
            gViewMain.DeleteSelectedRows();

        }
        private void BarButtonTimerStart_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (timer1.Enabled) return;
            Timer_Start();
            barStaticInfo.Caption = string.Empty;
            barStaticInfo.ItemAppearance.Normal.Options.UseBackColor = false;
        }
        private void BarButtonTimerStop_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!timer1.Enabled) return;
            Timer_Stop(false);
            barStaticInfo.Caption = "Таймер остановлен!!!";
            barStaticInfo.ItemAppearance.Normal.BackColor = Color.Tomato;
            barStaticInfo.ItemAppearance.Normal.Options.UseBackColor = true;
        }
        private void Timer_Start()
        {
            timer1.Start();
            barButtonTimerStart.Enabled = false;
            barButtonTimerStop.Enabled = true;
        }
        private void Timer_Stop(bool disable = false)
        {
            timer1.Stop();
            barButtonTimerStop.Enabled = false;
            barButtonTimerStart.Enabled = !disable;
        }
        private void BtnDownPoint_Click(object sender, EventArgs e)
        {
            var view = gViewDetail;
            view.GridControl.Focus();
            var index = view.FocusedRowHandle;
            if (index >= view.DataRowCount - 1) return;

            var row1 = (HistPoint)view.GetRow(index);
            var row2 = (HistPoint)view.GetRow(index + 1);
            var val1 = row1.pointposn;
            var val2 = row2.pointposn;
            row1.pointposn = val2;
            row2.pointposn = val1;
            view.RefreshData();
            view.FocusedRowHandle = index + 1;
        }
        private void BtnUpPoint_Click(object sender, EventArgs e)
        {
            var view = gViewDetail;
            view.GridControl.Focus();
            var index = view.FocusedRowHandle;
            if (index <= 0) return;

            var row1 = (HistPoint)view.GetRow(index);
            var row2 = (HistPoint)view.GetRow(index - 1);
            var val1 = row1.pointposn;
            var val2 = row2.pointposn;
            row1.pointposn = val2;
            row2.pointposn = val1;
            view.RefreshData();
            view.FocusedRowHandle = index - 1;
        }
        private void BtnRemovePoint_Click(object sender, EventArgs e)
        {
            var handle = gViewDetail.FocusedRowHandle;
            var id = (int)gViewDetail.GetRowCellValue(handle, "histpointid");
            _deletedHistpoints.Add(id);
            gViewDetail.DeleteRow(handle);
        }
        private void BtnAddPoint_Click(object sender, EventArgs e)
        {
            var report = (ReportDefinition)_bindingSourceMain.Current;
            if (report.HistPoints == null) report.HistPoints = new List<HistPoint>();
            _bindingSourceDetail.DataSource = report.HistPoints;
            gViewDetail.AddNewRow();
            gViewDetail.SetRowCellValue(GridControl.NewItemRowHandle, gViewDetail.Columns["format"], "0.00");
            gViewDetail.SetRowCellValue(GridControl.NewItemRowHandle, gViewDetail.Columns["integconst"], "1.0");
            gViewDetail.SetRowCellValue(GridControl.NewItemRowHandle, gViewDetail.Columns["reportdefinitionid"], report.reportdefinitionid);

            if (!report.HistPoints.Any())
            {
                gViewDetail.SetRowCellValue(gViewDetail.FocusedRowHandle, gViewDetail.Columns["pointposn"], 1);
            }
            else
                gViewDetail.SetRowCellValue(gViewDetail.FocusedRowHandle, gViewDetail.Columns["pointposn"], report.HistPoints.Max(m => m.pointposn) + 1);

        }
        private void BtnImportPoints_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            var report = (ReportDefinition)_bindingSourceMain.Current;
            var pointposn = 1;
            if (report.HistPoints != null && report.HistPoints.Count > 0) pointposn = report.HistPoints.Max(m => m.pointposn) + 1;
            if (dialog.ShowDialog() != DialogResult.OK) return;
            var dialogPoints = MessageBox.Show("Заменить точки новыми?", "Импорт точек", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            var file = File.ReadAllLines(dialog.FileName);
            if (dialogPoints == DialogResult.Yes)
            {
                if (report.HistPoints != null) _deletedHistpoints = new List<int>(report.HistPoints.Select(s => s.histpointid));
                report.HistPoints.Clear();
                pointposn = 1;
            }

            foreach (var row in file)
            {
                var s = row.Split(';');
                var p = new HistPoint
                {
                    pointname = s[0]
                };
                if (s.Length > 1)
                {
                    p.format = s[1];
                }
                if (s.Length > 2)
                {
                    p.description = s[2];
                }
                p.pointposn = pointposn;
                pointposn++;
                report.HistPoints?.Add(p);
            }



            gViewDetail.RefreshData();
            barStaticInfo.Caption = $"Imported from {dialog.FileName}";
        }
        private void BtnExportPoints_Click(object sender, EventArgs e)
        {
            var report = (ReportDefinition)_bindingSourceMain.Current;
            if (report.HistPoints == null) return;
            var list = report.HistPoints.Select(p => string.Join(";", p.pointname,
                    p.format, p.description))
                .ToList();
            var dir = Path.GetDirectoryName(report.destinationinfo);
            var filename = Path.GetFileNameWithoutExtension(report.destinationinfo);
            if (dir == null) return;
            var combine = Path.Combine(dir, "Points_" + filename + ".txt");
            var dialog = new SaveFileDialog() { FileName = combine };

            if (dialog.ShowDialog() != DialogResult.OK) return;
            File.WriteAllLines(dialog.FileName, list);
            barStaticInfo.Caption = $@"Exported {dialog.FileName}";

        }
        private void BarButtonSaveOptions_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridConrolMain.MainView.SaveLayoutToXml(_fileName);

                _cycleSet = int.Parse(barSpinCycle.EditValue.ToString());
                _cycleTimer = 0;
                Settings.Default["Period"] = _cycleSet;
                Settings.Default["Heigth"] = this.ClientSize.Height;
                Settings.Default["Width"] = this.ClientSize.Width;

                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Ошибка, что-то не так с настройками.", DefaultBoolean.True);
            }
        }
        #region Контексное меню
        private void gridViewMain_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            var view = sender as GridView;
            if (e.MenuType != GridMenuType.Row) return;
            var rowHandle = e.HitInfo.RowHandle;
            e.Menu.Items.Clear();
            e.Menu.Items.Add(CreateSubMenuRows("Генерировать отчет", rowHandle, OnGenerateRowClick));
            e.Menu.Items.Add(CreateSubMenuRows("Открыть в проводнике ...", rowHandle, OnExplorerRowClick));
            e.Menu.Items.Add(CreateSubMenuRows("Копировать отчет", rowHandle, OnCopyRowClick));
            e.Menu.Items.Add(CreateSubMenuRows("Удалить отчет", rowHandle, OnDeleteRowClickAsync));
        }
        private static DXMenuItem CreateSubMenuRows(string caption, int rowHandle, EventHandler eventHandler)
        {
            var menuItem = new DXMenuItem(caption, eventHandler)
            {
                Tag = rowHandle
            };
            return menuItem;
        }
        void OnExplorerRowClick(object sender, EventArgs e)
        {
            var report = (ReportDefinition)_bindingSourceMain.Current;
            Process.Start("explorer.exe", @Path.GetDirectoryName(report.destinationinfo));
        }
        #endregion
        private void InitgViewMain()
        {
            gViewMain.OptionsBehavior.AutoPopulateColumns = false;
            gViewMain.OptionsBehavior.Editable = false;
            gViewMain.OptionsView.ShowIndicator = false;
            gViewMain.OptionsSelection.EnableAppearanceFocusedCell = false;
            gViewMain.FocusRectStyle = DrawFocusRectStyle.RowFocus;
            gViewMain.OptionsDetail.EnableMasterViewMode = false;
            gViewMain.OptionsSelection.MultiSelect = true;
            gViewMain.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;

            gViewMain.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn()
            {
                Name = "reportdefinitionid",
                FieldName = "reportdefinitionid",
                Caption = "ID",
                Visible = false,
            });
            gViewMain.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn()
            {
                Name = "unit",
                FieldName = "unit",
                Caption = "Блок",
                Visible = true,
                Width = 30,


            });
            gViewMain.Columns["unit"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gViewMain.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn()
            {
                Name = "ReportName",
                FieldName = "reportname",
                Caption = "Название отчета",
                Visible = true,
                Width = 120,
            });

            RepositoryItemLookUpEdit lookUpEditReportDestId = new RepositoryItemLookUpEdit()
            {
                DisplayMember = "DestName",
                ValueMember = "ReportDestID",
            };
            gViewMain.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn()
            {
                Name = "ReportDestID",
                FieldName = "reportdestid",
                Caption = "Тип",
                Visible = false,
                Width = 30,
                ColumnEdit = new RepositoryItemLookUpEdit()
                {
                    DataSource = Dictionares.DestinationType,
                    ShowHeader = false,
                    ShowFooter = false,
                    PopupFormMinSize = new Size(1, 1),
                    DropDownRows = Dictionares.DestinationType.Count
                }
            });
            gViewMain.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn()
            {
                Name = "DestinationInfo",
                FieldName = "destinationinfo",
                Caption = "Расположение файла",
                Visible = true,
                Width = 150

            });

            gViewMain.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn()
            {
                Name = "ReportTypeID",
                FieldName = "reporttypeid",
                Caption = "Вид отчета",
                Visible = true,
                Width = 40,
                ColumnEdit = new RepositoryItemLookUpEdit()
                {
                    DataSource = Dictionares.ReportType,
                    ValueMember = "Id",
                    DisplayMember = "Value"
                }
            });


            gViewMain.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn()
            {
                Name = "NextEvent",
                FieldName = "nextevent",
                Caption = "Следущая дата",
                Visible = true,
            });

            gViewMain.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn()
            {
                Name = "enable",
                FieldName = "enable",
                Caption = "Авто",
                Visible = true,
                Width = 30,
                ColumnEdit = new RepositoryItemCheckEdit()
            });


            gViewMain.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn()
            {
                Name = "LastUsed",
                FieldName = "lastused",
                Caption = "Последняя дата форм.",
                Visible = true,
            });
        }
        private void InitgViewDetail()
        {
            gViewDetail.OptionsBehavior.AutoPopulateColumns = false;


            gViewDetail.OptionsBehavior.Editable = false;
            gViewDetail.OptionsView.ShowIndicator = false;
            gViewDetail.OptionsSelection.EnableAppearanceFocusedCell = false;
            gViewDetail.FocusRectStyle = DrawFocusRectStyle.RowFocus;
            gViewDetail.OptionsBehavior.EditingMode = GridEditingMode.Inplace;
            gViewDetail.OptionsBehavior.EditorShowMode = EditorShowMode.Click;
            gViewDetail.OptionsEditForm.EditFormColumnCount = 1;
            gViewDetail.OptionsEditForm.PopupEditFormWidth = 300;


            #region !!!!!!!!!!!!!!!!!!Columns!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! 

            DevExpress.XtraGrid.Columns.GridColumn colPoinPosn = new DevExpress.XtraGrid.Columns.GridColumn()
            {
                Name = "pointposn",
                FieldName = "pointposn",
                Caption = "№ п/п",
                Visible = true,
                Width = 20,
            };
            colPoinPosn.OptionsColumn.ImmediateUpdateRowPosition = DevExpress.Utils.DefaultBoolean.True;
            colPoinPosn.OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.False;

            gViewDetail.Columns.Add(colPoinPosn);
            gViewDetail.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
             new DevExpress.XtraGrid.Columns.GridColumnSortInfo(colPoinPosn, DevExpress.Data.ColumnSortOrder.Ascending)});


            gViewDetail.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn()
            {
                Name = "histpointid",
                FieldName = "histpointid",
                Caption = "ID"

            });



            gViewDetail.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn()
            {
                Name = "pointname",
                FieldName = "pointname",
                Caption = "Код сигнала",
                Visible = true,

            });

            gViewDetail.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn()
            {
                Name = "description",
                FieldName = "description",
                Caption = "Описание сигнала",
                Visible = true,

            });
            gViewDetail.Columns["description"].OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.False;

            gViewDetail.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn()
            {
                Name = "format",
                FieldName = "format",
                Caption = "Формат",
                Visible = true,
                Width = 20
            });


            gViewDetail.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn()
            {
                Name = "proctype",
                FieldName = "proctype",
                Caption = "Тип выбоки",
                Visible = false,
                Width = 30,
                ColumnEdit = new RepositoryItemLookUpEdit()
                {
                    DataSource = Dictionares.Filters,
                    DisplayMember = "Value",
                    ValueMember = "Id",
                    PopupFormMinSize = new Size(1, 1),
                    DropDownRows = Dictionares.Filters.Count
                }
            });

            gViewDetail.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn()
            {
                Name = "integconst",
                FieldName = "integconst",
                Caption = "Интегр.",
                Visible = false,
                Width = 20

            });
            RepositoryItemComboBox boxBitNumber = new RepositoryItemComboBox();
            for (int i = 0; i < 32; i++)
            {
                boxBitNumber.Items.Add(i);
            }

            gViewDetail.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn()
            {
                Name = "bitnumber",
                FieldName = "bitnumber",
                Caption = "Бит",
                Visible = false,
                Width = 10,
                ColumnEdit = boxBitNumber

            });
            #endregion
        }

        public static IDbConnection DataConnection()
        {
            var host = (string)Settings.Default["Host"];
            var port = (int)Settings.Default["Port"];
            var database = (string)Settings.Default["Database"];
            var user = (string)Settings.Default["User"];
            var pass = (string)Settings.Default["Password"];
            return ConnectDb.GetConnection(host, port, database, user, pass);
        }
    }
}
