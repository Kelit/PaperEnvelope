using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaperSleeve
{
    public partial class Service : Form
    {
        public Service()
        {
            InitializeComponent();
        }

        private void Service_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("Arial");
            comboBox1.Items.Add("Courier");
            comboBox1.Items.Add("Times New Roman"); 
        }
    }
}
