//Created by Vitaly Dekhtyarev 01.02.2022.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static Utilities;

namespace CSharp_SWEng_A1
{
    public partial class Form1 : Form
    {
        string path = "";
        private string logStr = "";
        string msg = "";
        private SensorA[] sensorsA;
        private SensorD[] sensorsD;
        private object[][] rows;
        private ctrl ctrl;
        private System.ComponentModel.BackgroundWorker senBGW_filter = new BackgroundWorker();
        private System.ComponentModel.BackgroundWorker senBGW_sample = new BackgroundWorker();
        private System.ComponentModel.BackgroundWorker senBGW_log = new BackgroundWorker();
        public Form1()
        {
            InitializeComponent();
            initGrid();
            initsenBGW_sample();
            initsenBGW_log();
        }
        private void initGrid()
        {
            /*
            if(rows == null || rows.Length == 0)
            {
                senGrid0.Rows[0].Cells[0].Value = "null";
                senGrid0.Rows[0].Cells[1].Value = "null";
                senGrid0.Rows[0].Cells[2].Value = "null";
                senGrid0.Rows[0].Cells[3].Value = "null";
            }
            */
        }
        private void initsenBGW_filter()
        {
            senBGW_filter.WorkerSupportsCancellation = true;
            senBGW_filter.WorkerReportsProgress = true;
            senBGW_filter.DoWork += new DoWorkEventHandler(senBGW_filter_DoWork);
            senBGW_filter.RunWorkerCompleted += new RunWorkerCompletedEventHandler(senBGW_filter_RunWorkerCompleted);
            senBGW_filter.ProgressChanged += new ProgressChangedEventHandler(senBGW_filter_ProgressChanged);
        }
        private void initsenBGW_sample()
        {
            senBGW_sample.WorkerSupportsCancellation = true;
            senBGW_sample.WorkerReportsProgress = true;
            senBGW_sample.DoWork += new DoWorkEventHandler(senBGW_sample_DoWork);
            senBGW_sample.RunWorkerCompleted += new RunWorkerCompletedEventHandler(senBGW_sample_RunWorkerCompleted);
            senBGW_sample.ProgressChanged += new ProgressChangedEventHandler(senBGW_sample_ProgressChanged);
        }
        private void initsenBGW_log()
        {
            senBGW_log.WorkerSupportsCancellation = true;
            senBGW_log.WorkerReportsProgress = true;
            senBGW_log.DoWork += new DoWorkEventHandler(senBGW_log_DoWork);
            senBGW_log.RunWorkerCompleted += new RunWorkerCompletedEventHandler(senBGW_log_RunWorkerCompleted);
            senBGW_log.ProgressChanged += new ProgressChangedEventHandler(senBGW_log_ProgressChanged);
        }
        private void senBGW_filter_DoWork(object sender, DoWorkEventArgs e)
        {
            rep rep = new rep();
            ctrl ctrlTmp = e.Argument as ctrl;
            BackgroundWorker worker = sender as BackgroundWorker;
            double filter_interval = ctrlTmp.st/10/5;

            System.Threading.Thread.Sleep((int)(ctrlTmp.st / 10 * 9 * 1000));  //Skip time before first sampling

            while (true){
                rep.startTime = DateTime.Now;
                rep.endTimeSamp = rep.startTime.AddSeconds(ctrlTmp.st);

                if (worker.CancellationPending == true){
                    e.Cancel = true;
                    break;
                }

                if (rep.iRep == 5){
                    rep.iRep = 0;
                    /*
                    if (sensorsA != null){
                        for (int si = 0; si < sensorsA.Length; si++){
                            if (sensorsA[si] != null){
                                sensorsA[si].getFiltered();
                            }
                        }
                    }
                    */
                    System.Threading.Thread.Sleep((int)(ctrlTmp.st * 1000));
                }else{
                    if (sensorsA != null)
                    {
                        for (int si = 0; si < sensorsA.Length; si++)
                        {
                            if (sensorsA[si] != null)
                            {
                                sensorsA[si].setValueArr(rep.iRep);
                            }
                        }
                    }
                    rep.iRep++;
                    System.Threading.Thread.Sleep((int)(filter_interval * 1000));
                }
                
                if (worker.CancellationPending == true){
                    e.Cancel = true;
                    break;
                }
            }
        }
        private void senBGW_filter_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ;
        }
        private void senBGW_filter_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ;
        }
        private void senBGW_sample_DoWork(object sender, DoWorkEventArgs e)
        {
            rep rep = new rep();
            ctrl ctrlTmp = e.Argument as ctrl;
            BackgroundWorker worker = sender as BackgroundWorker;
            System.Threading.Thread.Sleep((int)(ctrlTmp.st * 1000));  //Skip time before first sampling
            while (true)
            {
                rep.startTime = DateTime.Now;
                rep.endTimeSamp = rep.startTime.AddSeconds(ctrlTmp.st);
                rep.iRep = 1;

                if(sensorsA != null)
                {
                    for (int si = 0; si < sensorsA.Length; si++)
                    {
                        if (sensorsA[si] != null)
                        {
                            //sensorsA[si].getValue();
                            sensorsA[si].getFiltered();
                        }
                    }
                }
                if (sensorsD != null)
                {
                    for (int si = 0; si < sensorsD.Length; si++)
                    {
                        if (sensorsD[si] != null)
                        {
                            sensorsD[si].getValue();
                        }
                    }
                }
                worker.ReportProgress(rep.iRep, rep);

                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }

                System.Threading.Thread.Sleep((int)(ctrlTmp.st * 1000));

                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
            }
        }
        private void senBGW_sample_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            rep repTmp = e.UserState as rep;
            stL2.Text = repTmp.endTimeSamp.ToString();
            updateRows();
            updateRowsValus();
        }
        private void senBGW_sample_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                msgBox0.Text += $"{DateTime.Now}:The measurement stopped!{Environment.NewLine}";
            }
            else if (e.Error != null)
            {
                msgBox0.Text += $"{DateTime.Now}:Error:{e.Error.Message}!{Environment.NewLine}";
            }
            else
            {
                msgBox0.Text += $"{DateTime.Now}:The measurement finished!{Environment.NewLine}";
            }
        }
        private void senBGW_log_DoWork(object sender, DoWorkEventArgs e)
        {
            rep rep = new rep();
            ctrl ctrlTmp = e.Argument as ctrl;
            BackgroundWorker worker = sender as BackgroundWorker;
            while (true)
            {
                logStr = "";
                rep.startTime = DateTime.Now;
                rep.endTimeLog = rep.startTime.AddSeconds(ctrlTmp.lt);
                
                if (sensorsA != null)
                {

                    logStr += $"{rep.startTime},";

                    for (int si = 0; si < sensorsA.Length; si++)
                    {
                        if (sensorsA[si] != null)
                        {
                            logStr += $"{sensorsA[si].Value.ToString(System.Globalization.CultureInfo.InvariantCulture)},";
                        }
                    }
                }
                if (sensorsD != null)
                {
                    if(logStr.Length == 0)
                    {
                        logStr += $"{rep.startTime},";
                    }
                    for (int si = 0; si < sensorsD.Length; si++)
                    {
                        if (sensorsD[si] != null)
                        {
                            logStr += $"{sensorsA[si].Value},";
                        }
                    }
                }
                //Remove comma in the end and add a new line
                logStr = logStr.Remove(logStr.Length - 1, 1);
                logStr += Environment.NewLine;

                worker.ReportProgress(rep.iRep, rep);

                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }

                System.Threading.Thread.Sleep((int)(ctrlTmp.lt * 1000));

                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
            }
        }
        private void senBGW_log_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            rep repTmp = e.UserState as rep;
            if(path != "")
            {
                if (bWriteFile(path, logStr) == -1)
                {
                    msgBox0.Text += $"{DateTime.Now}:Access to log file is forbidden!{Environment.NewLine}";
                }
            }
            ltL2.Text = repTmp.endTimeLog.ToString();
        }
        private void senBGW_log_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                msgBox0.Text += $"{DateTime.Now}:The logging stopped!{Environment.NewLine}";
            }
            else if (e.Error != null)
            {
                msgBox0.Text += $"{DateTime.Now}:Error:{e.Error.Message}!{Environment.NewLine}";
            }
            else
            {
                msgBox0.Text += $"{DateTime.Now}:The logging finished!{Environment.NewLine}";
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            msg = bReadFile(path);
        }
        private void senAnQuanUpd_Click(object sender, EventArgs e)
        {
            sensorsA = new SensorA[int.Parse(senAnQuanBox.Text)];
        }
        private void senDigQuanUpd_Click(object sender, EventArgs e)
        {
            sensorsD = new SensorD[int.Parse(senDigQuanBox.Text)];
        }
        private void fileNameButton0_Click(object sender, EventArgs e)
        {
            if(fileNameBox0.Text != ""){
                path = fileNameBox0.Text;
            }
        }
        private void senAddButton0_Click(object sender, EventArgs e)
        {
            bool added = false;
            bool def = false;
            bool minOk = false;
            bool maxOk = false;
            double senMin = 0;
            double senMax = 0;

            if (senAnCheck0.Checked && senDigCheck0.Checked){
                msgBox0.Text += $"{DateTime.Now}:Sensor can be of only one type.{Environment.NewLine}";
                return;
            }
            if (!senAnCheck0.Checked && !senDigCheck0.Checked){
                msgBox0.Text += $"{DateTime.Now}:Sensor type is not defined.{Environment.NewLine}";
                return;
            }
            if (senAnCheck0.Checked){
                if (sensorsA != null){
                    for(int si = 0; si < sensorsA.Length; si++){
                        if(sensorsA[si] == null){
                            if((minOk = double.TryParse(senMinBox0.Text, out senMin)) && (maxOk=double.TryParse(senMaxBox0.Text, out senMax))){
                                sensorsA[si] = new SensorA(senIdBox0.Text, senPosBox0.Text, senUnitBox0.Text, senDescBox0.Text, senMin, senMax);
                                added = true;
                                break;
                            }
                        }
                    }
                    def = true;
                }
            }else if (senDigCheck0.Checked){
                if (sensorsD != null){
                    for (int si = 0; si < sensorsD.Length; si++){
                        if (sensorsD[si] == null){
                            sensorsD[si] = new SensorD(senIdBox0.Text, senPosBox0.Text, senUnitBox0.Text, senDescBox0.Text);
                            added = true;
                            break;
                        }
                    }
                    def = true;
                }
            }
            if (added){
                msgBox0.Text += $"{DateTime.Now}:A sensor has been added.{Environment.NewLine}";
                int senANum = (sensorsA == null) ? 0 : sensCount(sensorsA);
                int senDNum = (sensorsD==null) ? 0 : sensCount(sensorsD);
                senNumLab0.Text = $"Total number of sensors: Analog: {senANum} Digital: {senDNum}.";
                this.createRows();
                this.updateGrid();

            }else{
                if (!def){
                    msgBox0.Text += $"{DateTime.Now}:No sensor has been added, because the number of sensors is not defined.{Environment.NewLine}";
                }else if (!minOk){
                    msgBox0.Text += $"{DateTime.Now}:No sensor has been added, because the minimum limit is not correct.{Environment.NewLine}";
                }else if (!maxOk){
                    msgBox0.Text += $"{DateTime.Now}:No sensor has been added, because the maximum limit is not correct.{Environment.NewLine}";
                }else{
                    msgBox0.Text += $"{DateTime.Now}:No sensor has been added, because the sensor racks are full.{Environment.NewLine}";
                }
            }
        }
        private void senAnCheck0_CheckedChanged(object sender, EventArgs e)
        {
            if (senAnCheck0.Checked){
                senDigCheck0.Enabled = false;
            }else{
                senDigCheck0.Enabled = true;
            }
        }
        private void senDigCheck0_CheckedChanged(object sender, EventArgs e)
        {
            if (senDigCheck0.Checked){
                senAnCheck0.Enabled = false;
            }else{
                senAnCheck0.Enabled = true;
            }
        }
        private void buttonBegMeas_Click(object sender, EventArgs e)
        {
            double st = 0;
            double lt = 0;

            if (stBox0.Text == "")
            {
                msgBox0.Text += $"{DateTime.Now}:Cannot start measurement: sampling time is not defined.{Environment.NewLine}";
                return;
            }
            ctrl = new ctrl();

            if(!double.TryParse(stBox0.Text, out st)){
                msgBox0.Text += $"{DateTime.Now}:Cannot start measurement: sampling time is wrong.{Environment.NewLine}";
                return;
            }else{
                ctrl.st = st;
            }
            if (!double.TryParse(ltBox0.Text, out lt)){
                msgBox0.Text += $"{DateTime.Now}:Logging time is wrong, no data will be saved.{Environment.NewLine}";
                ctrl.lt = 0;
            }
            else{
                ctrl.lt = lt;
                //If logging time is valid, it cannot be smaller than the sampling time.
                if(ctrl.lt < ctrl.st)
                {
                    ctrl.lt = ctrl.st;
                }
            }
            if (senBGW_sample.IsBusy != true)
            {
                senBGW_sample.RunWorkerAsync(ctrl);
            }
            if(ctrl.lt > 0)
            {
                if (senBGW_log.IsBusy != true)
                {
                    ctrl.path = path;
                    senBGW_log.RunWorkerAsync(ctrl);
                }
            }
        }
        private void buttonMeasStop_Click(object sender, EventArgs e)
        {
            this.senBGW_sample.CancelAsync();
            //this.senBGW_log.CancelAsync();
        }
        private void senRemButton0_Click(object sender, EventArgs e)
        {
            bool removed = false;

            if (senAnCheck0.Checked && senDigCheck0.Checked)
            {
                msgBox0.Text += $"{DateTime.Now}:Sensor can be of only one type.{Environment.NewLine}";
                return;
            }
            if (!senAnCheck0.Checked && !senDigCheck0.Checked)
            {
                msgBox0.Text += $"{DateTime.Now}:Sensor type is not defined.{Environment.NewLine}";
                return;
            }
            if (senAnCheck0.Checked)
            {
                if (sensorsA != null)
                {
                    for (int si = 0; si < sensorsA.Length; si++)
                    {
                        if (sensorsA[si] != null)
                        {
                            if (sensorsA[si].ID == senIdBox0.Text)
                            {
                                sensorsA[si].Dispose();
                                sensorsA[si] = null;
                                removed = true;
                                break;
                            }

                        }
                    }
                }
            }
            else if (senDigCheck0.Checked)
            {
                if (sensorsD != null)
                {
                    for (int si = 0; si < sensorsD.Length; si++)
                    {
                        if (sensorsD[si] != null)
                        {
                            if (sensorsD[si].ID == senIdBox0.Text)
                            {
                                sensorsD[si].Dispose();
                                sensorsD[si] = null;
                                removed = true;
                                break;
                            }
                        }
                    }
                }
            }
            createRows();
            remRow();
            updateGrid();

            if (removed)
            {
                msgBox0.Text += $"{DateTime.Now}:A sensor has been removed.{Environment.NewLine}";
                int senANum = (sensorsA == null) ? 0 : sensCount(sensorsA);
                int senDNum = (sensorsD == null) ? 0 : sensCount(sensorsD);
                senNumLab0.Text = $"Total number of sensors: Analog: {senANum} Digital: {senDNum}.";
            }
        }
        private void createRows()
        {
            int rowsLength = 0;
            bool senAdone = false;
            bool senDdone = false;
            if (sensorsA != null)
            {
                rowsLength += sensorsA.Length;
            }
            if (sensorsD != null)
            {
                rowsLength += sensorsD.Length;
            }
            rows = new object[rowsLength][];

            for(int ri = 0; ri < rows.Length; ri++)
            {
                if (sensorsA != null && !senAdone)
                {
                    for (int si = 0; si < sensorsA.Length; si++)
                    {
                        if (sensorsA[si] != null)
                        {
                            rows[ri] = new string[]{sensorsA[si].ID, sensorsA[si].SenType, sensorsA[si].TimeSample.ToString(), sensorsA[si].Value.ToString(System.Globalization.CultureInfo.InvariantCulture) };
                            ri++;
                        }
                    }
                    senAdone = true;
                }
                if (sensorsD != null && !senDdone)
                {
                    for (int si = 0; si < sensorsD.Length; si++)
                    {
                        if (sensorsD[si] != null)
                        {
                            rows[ri] = new string[] { sensorsD[si].ID, sensorsD[si].SenType, sensorsD[si].TimeSample.ToString(), sensorsD[si].Value.ToString() };
                            ri++;
                        }
                    }
                    senDdone = true;
                }
            }
        }
        private void updateRows()
        {
            bool senAdone = false;
            bool senDdone = false;

            if(rows != null)
            {
                for (int ri = 0; ri < rows.Length; ri++)
                {
                    if(rows[ri] != null)
                    {
                        if (sensorsA != null && !senAdone)
                        {
                            for (int si = 0; si < sensorsA.Length; si++)
                            {
                                if (sensorsA[si] != null)
                                {
                                    rows[ri][0] = sensorsA[si].ID;
                                    rows[ri][1] = sensorsA[si].SenType;
                                    rows[ri][2] = sensorsA[si].TimeSample.ToString();
                                    //rows[ri][3] = sensorsA[si].Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                                    rows[ri][3] = sensorsA[si].ValueFiltered.ToString(System.Globalization.CultureInfo.InvariantCulture);
                                    ri++;
                                }
                            }
                            senAdone = true;
                        }
                        if (sensorsD != null && !senDdone)
                        {
                            for (int si = 0; si < sensorsD.Length; si++)
                            {
                                if (sensorsD[si] != null)
                                {
                                    rows[ri][0] = sensorsD[si].ID;
                                    rows[ri][1] = sensorsD[si].SenType;
                                    rows[ri][2] = sensorsD[si].TimeSample.ToString();
                                    rows[ri][3] = sensorsD[si].Value.ToString();
                                    ri++;
                                }
                            }
                            senDdone = true;
                        }
                    }
                }
            }
        }
        public void updateRowsValus()
        {
            if (rows != null)
            {
                for (int ri = 0; ri < rows.Length && ri <senGrid0.Rows.Count; ri++)
                {
                    if (rows[ri] != null)
                    {
                        for (int gCi = 0; gCi < senGrid0.Columns.Count; gCi++)
                        {
                            senGrid0.Rows[ri].Cells[gCi].Value = rows[ri][gCi];
                        }
                    }
                }
            }
        }
        public void updateGrid()
        {
            int start = senGrid0.Rows.Count-1;
            if(rows != null)
            {
                for(int ri = start; ri < rows.Length; ri++)
                {
                    if (rows[ri] != null)
                    {
                        if (rows[ri].Length != 0)
                        {
                            senGrid0.Rows.Add(rows[ri]);
                        }
                    }
                }
            }
        }
        private void remRow()
        {
            for (int gI = 0; gI < senGrid0.Rows.Count; gI++)
            {
                if((string)senGrid0.Rows[gI].Cells[0].Value == senIdBox0.Text)
                {
                    if((string)senGrid0.Rows[gI].Cells[1].Value == "A" && senAnCheck0.Checked && !senDigCheck0.Checked)
                    {
                        if(senGrid0.Rows[gI] != null)
                        {
                            senGrid0.Rows.Remove(senGrid0.Rows[gI]);
                        }
                    }else if((string)senGrid0.Rows[gI].Cells[1].Value == "D" && !senAnCheck0.Checked && senDigCheck0.Checked)
                    {
                        if (senGrid0.Rows[gI] != null)
                        {
                            senGrid0.Rows.Remove(senGrid0.Rows[gI]);
                        }
                    }
                }
            }
        }
    }
}
