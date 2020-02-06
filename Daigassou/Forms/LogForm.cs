using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Daigassou
{
    public partial class LogForm : Form
    {
        Log.LogEvent hook;
        public LogForm()
        {
            InitializeComponent();
            hook = (string text) => {
                ListViewItem item = new ListViewItem();
                item.Text = text;
                Invoke(new Action(()=>logListView.Items.Add(text)));
            };
            Log.OverLayLogReceived += hook;
        }

        private void LogForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.OverLayLogReceived -= hook;
        }
    }
}
