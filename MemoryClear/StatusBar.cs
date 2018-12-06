using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemoryClear
{
    public partial class StatusBar : Form
    {
        FormMain frm;
        public StatusBar(FormMain frmMain)
        {
            frm = frmMain;
            InitializeComponent();
        }

        private void StatusBar_Load(object sender, EventArgs e)
        {
            this.Top = Screen.PrimaryScreen.Bounds.Height - 150;
            this.Left = Screen.PrimaryScreen.Bounds.Width - 80;
            this.Width = 60;
            this.Height = 60;
        }
    }
}
