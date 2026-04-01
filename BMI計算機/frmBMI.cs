using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BMI計算機
{
    public partial class frmBMI : Form
    {
        private string logFile = "BMI_Records.txt";

        public frmBMI()
        {
            InitializeComponent();

            // 初始化表格欄位
            InitHistoryGrid();

            // 手動綁定按鈕事件
            this.btnCalc.Click += new System.EventHandler(this.btnCalc_Click);
            this.btnOpenLog.Click += new System.EventHandler(this.btnOpenLog_Click);
        }

        private void InitHistoryGrid()
        {
            if (dgvHistory.Columns.Count == 0)
            {
                dgvHistory.Columns.Add("Time", "時間");
                dgvHistory.Columns.Add("Name", "姓名");
                dgvHistory.Columns.Add("Height", "身高"); // 新增身高欄位
                dgvHistory.Columns.Add("Weight", "體重"); // 新增體重欄位
                dgvHistory.Columns.Add("BMI", "BMI");
                dgvHistory.Columns.Add("Status", "診斷");

                dgvHistory.DefaultCellStyle.Font = new Font("微軟正黑體", 10);
                dgvHistory.ColumnHeadersDefaultCellStyle.Font = new Font("微軟正黑體", 10, FontStyle.Bold);

                // 自動調整欄位權重，讓顯示更平均
                dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txtName.Text.Trim();
                if (string.IsNullOrEmpty(name))
                {
                    MessageBox.Show("請輸入姓名！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                double h_cm = Convert.ToDouble(txtHeight.Text);
                double weight = Convert.ToDouble(txtWeight.Text);
                double h_m = h_cm / 100;
                double bmi = weight / (h_m * h_m);

                string status = "";
                Color displayColor = Color.Black;
                bool isAbnormal = false;

                // 核心判定邏輯
                if (bmi < 18.5) { status = "體重過輕"; displayColor = Color.Blue; isAbnormal = true; }
                else if (bmi < 24) { status = "正常範圍"; displayColor = Color.Green; isAbnormal = false; }
                else if (bmi < 27) { status = "過重"; displayColor = Color.Orange; isAbnormal = true; }
                else { status = "肥胖"; displayColor = Color.Red; isAbnormal = true; }

                // 更新大框框結果
                lblResult.Text = $"{bmi:F2} ({status})";
                lblResult.ForeColor = displayColor;

                // 執行存檔與介面更新 (傳入身高與體重參數)
                SaveToLog(name, h_cm, weight, bmi, status);
                AddRecordToGrid(name, h_cm, weight, bmi, status, isAbnormal);
            }
            catch (FormatException)
            {
                MessageBox.Show("請輸入數值！", "格式錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddRecordToGrid(string name, double h, double w, double bmi, string status, bool highlight)
        {
            // 在表格中新增包含身高與體重的行
            int n = dgvHistory.Rows.Add(
                DateTime.Now.ToString("HH:mm:ss"),
                name,
                h.ToString() + " cm",
                w.ToString() + " kg",
                bmi.ToString("F2"),
                status
            );

            if (highlight)
            {
                dgvHistory.Rows[n].DefaultCellStyle.ForeColor = Color.Red;
                dgvHistory.Rows[n].DefaultCellStyle.Font = new Font("微軟正黑體", 10, FontStyle.Bold);
                dgvHistory.Rows[n].DefaultCellStyle.SelectionForeColor = Color.Red;
            }
            dgvHistory.FirstDisplayedScrollingRowIndex = n;
        }

        private void btnOpenLog_Click(object sender, EventArgs e)
        {
            if (File.Exists(logFile))
                Process.Start(new ProcessStartInfo { FileName = "notepad.exe", Arguments = Path.GetFullPath(logFile), UseShellExecute = true });
            else
                MessageBox.Show("尚無紀錄。");
        }

        private void SaveToLog(string name, double h, double w, double bmi, string status)
        {
            try
            {
                // 在 TXT 檔案中也加入身高與體重紀錄，並調整對齊
                string record = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss} | 姓名:{name,-8} | 身高:{h,5}cm | 體重:{w,5}kg | BMI:{bmi,6:F2} | {status}";
                File.AppendAllLines(logFile, new string[] { record });
            }
            catch { }
        }

        private void frmBMI_Load(object sender, EventArgs e) { }
    }
}