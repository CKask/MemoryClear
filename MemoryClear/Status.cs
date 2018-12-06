using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemoryClear
{
    public partial class Status : Form
    {
        private Point mPoint;
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        public double? Percent;

        private const int VM_NCLBUTTONDOWN = 0XA1;//定义鼠标左键按下
        private const int HTCAPTION = 2;



        private string MemoryString = "内存占用";
        private string ClearMemoryString = "内存清理中";

        private bool ChangeFlag;

        FormMain frm;



        public Status(FormMain frmP)
        {
            frm = frmP;
           
            InitializeComponent();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Status_ShowMemoryPercent(object sender, EventArgs e)
        {
            Model model = (Model)sender;

            Percent = model.Percent;
           

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if(Percent !=null)
            {
                lblMessage.Text = ((int)Percent).ToString() + "%";
            }
        }
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mPoint = new Point(e.X, e.Y);
                //为当前应用程序释放鼠标捕获
                ReleaseCapture();
                //发送消息 让系统误以为在标题栏上按下鼠标
                SendMessage((IntPtr)this.Handle, VM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
            else
            {
                contextMenuStripMouseRight.Show(this, e.Location);
            }

        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Location.X + e.X - mPoint.X, this.Location.Y + e.Y - mPoint.Y);
            }
        }

        private void lblMessage_MouseDoubleClick(object sender, MouseEventArgs e)
        { 
            ChangeFlag = true;
           
            lblMessage.Visible = false;
          
            Task clearTask = Task.Factory.StartNew(new Action(new ClearMemory().Clear));
         
            clearTask.Wait();
            
            if (clearTask.IsCompleted)
            {
                lblMessage.Visible = true;
             

            }
            ChangeFlag = false;
       
        }

        private void lblMessage_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStripMouseRight.Show(this, e.Location);
            }

        }
        private void lblMessage_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Location.X + e.X - mPoint.X, this.Location.Y + e.Y - mPoint.Y);
            }
        }

        private void 显示主界面ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frm != null)
            {
                frm.ShowMemoryPercent -= Status_ShowMemoryPercent;
                frm.Show();
            }
            this.Close();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dispose();

            if (frm != null)
            {
                frm.ShowMemoryPercent -= Status_ShowMemoryPercent;
                frm.Dispose();
                frm.Close();
            }
            this.Close();
            Application.Exit();
        }

        private void Status_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Status_Load(object sender, EventArgs e)
        {
            this.Top = Screen.PrimaryScreen.Bounds.Height - 150;
            this.Left = Screen.PrimaryScreen.Bounds.Width - 80;
            this.Width = 60;
            this.Height = 60;
            this.BackColor = this.TransparencyKey;          //设置当前窗体的背景色为透明
            lblMessage.Visible = true;
            //this.Height -=7;
            //this.Width -= 60;
        
        }

        private void Status_Resize(object sender, EventArgs e)
        {

        }

      
    }
}

