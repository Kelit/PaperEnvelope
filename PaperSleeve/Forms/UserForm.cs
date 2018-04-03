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
    public partial class UserForm : Form
    {
        public UserForm()
        {
            InitializeComponent();
        }
        // Глобальные переменные из Метода Load Form1
        string Mail;
        string password;


        // Метод нужен для обрашения к Form1 и забору данных  
        public void LoadData(string Mail_,string password_)
        {
            Mail = Mail_;
            password = password_;
        }

        private void UserForm_Load(object sender, EventArgs e)
        {
            textBox1.Text = Mail;
            textBox2.Text = password;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // проверить изменились ли строки, если нет то просто выйти
            // Если строки изменили, сохранить в БД
        }
    }
}
