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
using Util.Event;
using Newtonsoft.Json;

//多线程参考
//https://docs.microsoft.com/zh-cn/previous-versions/visualstudio/visual-studio-2010/ms171728(v=vs.100)?redirectedfrom=MSDN

namespace Compare
{
    public partial class Form1 : Form
    {
        delegate void SetTextCallback(string text);
        BackgroundWorker backgroundWorker1;
        public Form1()
        {
            UserDefault.Init();
            InitializeComponent();
            EventDispatcher.RegisterEvent(this, 1001, this.CompareFileCallback);
            InitDirs();
        }

        void InitDirs()
        {
            this.textBox1.Text = UserDefault.GetValue("oldpath", "").ToString();
            this.textBox2.Text = UserDefault.GetValue("newpath", "").ToString();
            this.textBox3.Text = UserDefault.GetValue("destpath", "").ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = dialog.SelectedPath;
                UserDefault.SetValue("oldpath", this.textBox1.Text);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.textBox2.Text = dialog.SelectedPath;
                UserDefault.SetValue("newpath", this.textBox2.Text);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.textBox3.Text = dialog.SelectedPath;
                UserDefault.SetValue("destpath", this.textBox3.Text);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //方法1
            //var compareThread = new System.Threading.Thread(CmpareFunc);
            //compareThread.Start();

            //方法2
            CompareBackground();
        }

        void CmpareFunc()
        {
            var path1 = this.textBox1.Text;
            var path2 = this.textBox2.Text;
            var path3 = this.textBox3.Text;
            var isDir = Directory.Exists(path1) && Directory.Exists(path2) && Directory.Exists(path3);
            if (!isDir)
            {
                MessageBox.Show("错误的目录！");
                return;
            }
            var dirName = Path.GetFileName(path2);
            var path = path3 + "//" + dirName;
            if (Directory.Exists(path))
            {
                if (MessageBox.Show("存储位置存在相同名称的目录 " + dirName + ", 要覆盖吗？", "警告", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    return;
                }
                else
                {
                    CompareUtil.DeleteDir(path);
                    Directory.CreateDirectory(path);
                }
            }

            CompareUtil.CompareDirectory(path1, path2, path);
            this.SetText("结束");
        }

        public void CompareFileCallback(object filePath)
        {
            //方法1
            //this.SetText(filePath.ToString());

            //方法2
            this.backgroundWorker1.ReportProgress(0, filePath);
        }

        public void SetText(string text)
        {
            if (this.label4.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.label4.Text = text;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(
            object sender,
            RunWorkerCompletedEventArgs e)
        {
            this.label4.Text = "结束";
        }

        void backgroundWorker1_ProcessChanged(object sender, ProgressChangedEventArgs e)
        {
            this.label4.Text = e.UserState.ToString();
        }

        void BackgroundCompareFun(object sender,
            DoWorkEventArgs e)
        {
            this.CmpareFunc();
        }

        void CompareBackground()
        {
            this.backgroundWorker1 = new BackgroundWorker();
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProcessChanged);
            this.backgroundWorker1.DoWork += new DoWorkEventHandler(BackgroundCompareFun);
            this.backgroundWorker1.RunWorkerAsync();
        }
    }
}
