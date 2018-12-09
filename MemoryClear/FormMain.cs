using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using Microsoft.Win32;
using System.Configuration;
using System.Xml;

namespace MemoryClear
{
    public partial class FormMain : System.Windows.Forms.Form
    {
        /// <summary>
        /// 内存占比
        /// </summary>
        private int MemoryValue = 80;
        /// <summary>
        /// 间隔时间
        /// </summary>
        private double MemoryMinute = 5;

        private double m_PhysicalMemory;
        public int iActulaWidth { get; set; }
        public int iActulaHeight { get; set; }

      

        public event EventHandler ShowMemoryPercent;
        /// <summary>
        /// 状态窗体
        /// </summary>
        StatusPercent frm;



        bool flag = true;

        #region 可用内存 
        ///  
        /// 获取可用内存 
        ///  
        public double MemoryAvailable
        {
            get; set;

        }
        #endregion

        #region 物理内存 
        ///  
        /// 获取物理内存 
        ///  
        public double PhysicalMemory
        {
            get
            {
                return m_PhysicalMemory;
            }
        }
        public double Percent { get; set; }
        #endregion

        public FormMain()
        {
            InitializeComponent();
            //设置timer可用
            Timer.Enabled = true;
            TimerShow.Enabled = true;
            Timer.Stop();
            //获得物理内存 
            ManagementClass cimobject1 = new ManagementClass("Win32_PhysicalMemory");
            ManagementObjectCollection moc1 = cimobject1.GetInstances();
            foreach (ManagementObject mo1 in moc1)
            {
                m_PhysicalMemory += ((Math.Round(Int64.Parse(mo1.Properties["Capacity"].Value.ToString()) / 1024 / 1024 / 1024.0, 1)));
            }
            moc1.Dispose();
            cimobject1.Dispose();

            MemoryValue=Convert.ToInt32(ConfigurationManager.AppSettings["MemoryValue"]);
            MemoryMinute= Convert.ToDouble(ConfigurationManager.AppSettings["MemoryMinute"]);
        }


        /// <summary>
        /// form加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            txtValue.Text = MemoryValue.ToString();
            txtM.Text = MemoryMinute.ToString();
            btnStop.Enabled = false;
            iActulaWidth = Screen.PrimaryScreen.Bounds.Width;

            iActulaHeight = Screen.PrimaryScreen.Bounds.Height;
            try
            {
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                object obj = rk2.GetValue("MemoryClear");
                if (obj != null)
                {
                    cboxStartBySelf.Checked = true;
                }
                else
                {
                    cboxStartBySelf.Checked = false;
                }
                rk2.Close();
                rk.Close();
            }
            catch (Exception)
            {
                cboxStartBySelf.Checked = false;
            }
            frm = new StatusPercent(this);
           
           
           
        }
        /// <summary>
        /// 菜单开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 开始ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Start();
            this.Hide();
        }
        /// <summary>
        /// 菜单推出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dispose();
            this.Close();
            if (frm != null)
            {
                ShowMemoryPercent = null;
                frm.Close();
            }
            Application.Exit();

        }
        /// <summary>
        /// 开始按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            Start();
            this.Hide();
           
        }
        /// <summary>
        /// 任务图标的双击显示主窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            ShowMemoryPercent = null;
            if (frm != null)
            {
               
                frm.Hide();
            }

        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            if (frm == null)
            {
                frm = new StatusPercent(this);
              
            }
            frm.Show();
            e.Cancel = true;
        }

        /// <summary>
        /// 开始
        /// </summary>
        public void Start()
        {
            SaveConfig("MemoryValue", MemoryValue.ToString());
            SaveConfig("MemoryMinute", MemoryMinute.ToString());

            txtM.Enabled = false;
            txtValue.Enabled = false;
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            contextMenuStrip1.Items[0].Enabled = false;
            //设置timer
            Timer.Interval = (int)(MemoryMinute * 60 * 1000);
            Timer.Start();
            if(frm==null)
            {
                
                frm = new StatusPercent(this);
               
            }
            frm.IsAutoClear = true;
            frm.Show();
        }
        /// <summary>
        /// 定时清理内存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
          Task t=Task.Factory.StartNew(() =>
            {
                if (Percent > MemoryValue)
                {
                    ClearMemory cm = new ClearMemory();
                    cm.Clear();
                    MemoryAvailable = GetAvailableMemory();
                    Percent = Math.Round(((PhysicalMemory - MemoryAvailable) / PhysicalMemory) * 100, 2);
                    //  notifyIcon.ShowBalloonTip(500, "Clear", $"现在内存占有率{Percent}%", ToolTipIcon.Info);
                }
            });
           
           
        }
        /// <summary>
        /// 停止按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStop_Click(object sender, EventArgs e)
        {
            Stop();
        }
        public void Stop()
        {
            txtM.Enabled = true;
            txtValue.Enabled = true;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            Timer.Stop();
            if (frm == null)
            {
                frm = new StatusPercent(this);

            }
            frm.Hide();
            frm.IsAutoClear = false;
            contextMenuStrip1.Items[0].Enabled = true;
        }

        /// <summary>
        /// 定时显示内存占用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerShow_Tick(object sender, EventArgs e)
        {
           Task t= Task.Factory.StartNew(() =>
            {
                MemoryAvailable = GetAvailableMemory();
                lblMemory.Invoke(new Action(() =>
                {
                    Percent = Math.Round(((PhysicalMemory - MemoryAvailable) / PhysicalMemory) * 100, 2);
                    lblMemory.Text = Percent.ToString();
                }));
            }); 
            frm.Percent = Percent;
        }
        /// <summary>
        /// 获取可用内存
        /// </summary>
        /// <returns></returns>
        private double GetAvailableMemory()
        {
            double availablebytes = 0;
            ManagementClass cimobject2 = new ManagementClass("Win32_PerfFormattedData_PerfOS_Memory");
            ManagementObjectCollection moc2 = cimobject2.GetInstances();
            foreach (ManagementObject mo2 in moc2)
            {
                availablebytes += ((Math.Round(Int64.Parse(mo2.Properties["AvailableMBytes"].Value.ToString()) / 1024.0, 1)));
            }
            moc2.Dispose();
            cimobject2.Dispose();
            return availablebytes;
        }

        private void txtValue_Validated(object sender, EventArgs e)
        {
            try
            {
                int temp = int.Parse(txtValue.Text.Trim());
                if (temp < 30 || temp > 100)
                {
                    MessageBox.Show("请输入30-100的数字");
                    txtValue.Focus();
                    return;
                }
                MemoryValue = temp;
            }
            catch (Exception)
            {
                MessageBox.Show("请输入数字");
                txtValue.Focus();
                return;

            }
        }

        private void txtM_Validated(object sender, EventArgs e)
        {
            try
            {
                double temp = double.Parse(txtM.Text.Trim());
                if (temp < 0 || temp > 100)
                {
                    MessageBox.Show("请输入0-100的数字");
                    txtM.Focus();
                    return;
                }
                MemoryMinute = temp;
            }
            catch (Exception)
            {
                MessageBox.Show("请输入数字");
                txtM.Focus();
                return;

            }
        }
        /// <summary>
        /// 自启动的checkbox验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (flag)
            {
                flag = false;
                try
                {
                    if (cboxStartBySelf.Checked) //设置开机自启动  
                    {

                        string path = Application.ExecutablePath;
                        RegistryKey rk = Registry.LocalMachine;
                        RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                        rk2.SetValue("MemoryClear", path);
                        rk2.Close();
                        rk.Close();
                    }
                    else //取消开机自启动  
                    {

                        string path = Application.ExecutablePath;
                        RegistryKey rk = Registry.LocalMachine;
                        RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                        rk2.DeleteValue("MemoryClear", false);
                        rk2.Close();
                        rk.Close();
                    }
                }
                catch (Exception)
                {

                    MessageBox.Show("需要管理员权限");
                    cboxStartBySelf.Checked = false;
                }
                flag = true;
            }


        }

        /// <summary>
        /// 保存到配置文件
        /// </summary>
        /// <param name="strKey"></param>
        /// <param name="ConnenctionString"></param>
        private void SaveConfig(string strKey, string ConnenctionString)
        {
            XmlDocument doc = new XmlDocument();
            //获得配置文件的全路径  
            string strFileName = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            // string  strFileName= AppDomain.CurrentDomain.BaseDirectory + "\\exe.config";  
            doc.Load(strFileName);
            //找出名称为“add”的所有元素  
            XmlNodeList nodes = doc.GetElementsByTagName("add");
            for (int i = 0; i < nodes.Count; i++)
            {
                //获得将当前元素的key属性  
                XmlAttribute att = nodes[i].Attributes["key"];
                //根据元素的第一个属性来判断当前的元素是不是目标元素  
                if (att.Value == strKey)
                {
                    //对目标元素中的第二个属性赋值  
                    att = nodes[i].Attributes["value"];
                    att.Value = ConnenctionString;
                    break;
                }
            }
            //保存上面的修改  
            doc.Save(strFileName);
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            if (cboxStartBySelf.Checked)
            {
                this.Hide();
                Start();
               
            }
        }
    }
}
