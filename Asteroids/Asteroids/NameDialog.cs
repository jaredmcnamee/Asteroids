using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Asteroids
{
    public partial class NameDialog : Form
    {

        public string GetName { get { return _tbxName.Text; } }
        public NameDialog()
        {
            InitializeComponent();
            _btnNameOK.Click += _btnNameOK_Click;
        }

        private void _btnNameOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
