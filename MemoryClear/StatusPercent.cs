using MemoryClear.Properties;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemoryClear
{
    public partial class StatusPercent : Form
    {
        FormMain frm;
        private Point mPoint;
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        private bool _isAutoCLear;
        /// <summary>
        /// 是否在自动清理中
        /// </summary>
        public bool IsAutoClear
        {
            get
            {
                return _isAutoCLear;
            }
            set
            {
                _isAutoCLear = value;
                if(value)
                {
                    pictureBox1.Image = Resources.timer;
                    contextMenuStripRight.Items[1].Enabled = false;
                    contextMenuStripRight.Items[2].Enabled = true;
                }
                else
                {
                    pictureBox1.Image = Resources.timer1;
                    contextMenuStripRight.Items[1].Enabled = true;
                    contextMenuStripRight.Items[2].Enabled = false;
                }
             

            }
        }

        private const int VM_NCLBUTTONDOWN = 0XA1;//定义鼠标左键按下
        private const int HTCAPTION = 2;
        private double? _percent;
        public double? Percent
        {
            get {
                return _percent;
            }
            set 
            {
                _percent = value;
                lblMessage.Text = ((int)value).ToString() + "%";
            }
        }

        /// <summary>
        /// 默认带参构造函数
        /// </summary>
        /// <param name="fms"></param>
        public StatusPercent(FormMain fms)
        {
            frm = fms;
           
            InitializeComponent();
        }
      
        private void StatusPercent_Load(object sender, EventArgs e)
        {
            this.Top = Screen.PrimaryScreen.Bounds.Height - 150;
            this.Left = Screen.PrimaryScreen.Bounds.Width - 80;
            this.Width = 60;
            this.Height = 25;
        }

        private void 显示主界面ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frm != null)
            { 
                frm.Show();
            }
            this.Hide();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dispose();

            if (frm != null)
            {
               
                frm.Dispose();
                frm.Close();
            }
            this.Close();
            Application.Exit();
        }

        private void panel_MouseDown(object sender, MouseEventArgs e)
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
                contextMenuStripRight.Show(this, e.Location);
            }
        }

        private void panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Location.X + e.X - mPoint.X, this.Location.Y + e.Y - mPoint.Y);
            }
        }

        private void lblMessage_MouseDown(object sender, MouseEventArgs e)
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
                contextMenuStripRight.Show(this, e.Location);
            }
        }

        private void lblMessage_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Location.X + e.X - mPoint.X, this.Location.Y + e.Y - mPoint.Y);
            }
        }

        private void lblMessage_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Clear();
        }
        private void Clear()
        {
            
            Task clearTask = Task.Factory.StartNew(new Action(new ClearMemory().Clear));

            //clearTask.Wait();

            //if (clearTask.IsCompleted)
            //{
            //    //动态的
            //    if(IsAutoClear)
            //    {
            //        pictureBox1.Image = Resources.timer;
            //    }
            //    else
            //    {
            //        pictureBox1.Image = Resources.timer1;
            //    }
               
            //}
          
        }

        private void 开始ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(frm!=null)
            {
                frm.Start();
            }
        }

        private void 停止ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frm != null)
            {
                frm.Stop();
            }
        }
    }
}
