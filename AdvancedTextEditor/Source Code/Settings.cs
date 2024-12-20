using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdvancedTextEditor
{
    public partial class Settings : Form
    {
        private Form1 _mainForm;

        public Settings(Form1 mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
        }

        public void ChangeBackgroundColor(Color newColor)
        {
            this.BackColor = newColor;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Settings_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
