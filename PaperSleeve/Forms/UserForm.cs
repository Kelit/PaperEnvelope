using System;
using EnvelopePaper.Class;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
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

        // Объект для связи с БД
        DataConection r = new DataConection();

        //Метод нужен для обрашения к Form1 и забору данных
        public void LoadData(string Mail_, string password_)
        {
            Mail = Mail_;
            password = password_;
        }

        private void UserForm_Load(object sender, EventArgs e)
        {
            textBox1.Text =Mail;
            textBox2.Text =password;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // проверить изменились ли строки
            if ((textBox1.Text!=Mail) || (textBox2.Text != password)&&
                ((textBox1.Text !="")&&(textBox2.Text != "")))
            {
                // Находим в почтовом адресе символ @ и от него
                // вырезаем правую часть оставляя только
                // mail.ru,yandex.ru,gmail.com, потом через switch присваиваем свойствам
                // определенные номера портов
                Mail = textBox1.Text;
                int istart = Mail.IndexOf("@", StringComparison.InvariantCultureIgnoreCase);
                if (istart < 0)
                    MessageBox.Show("Вы ввели не правильный адрес");

                string ShortMail = Mail.Remove(0, istart + 1);

                switch (ShortMail)
                {
                    case "mail.ru":
                        r.Portpop = "995";
                        r.PortSmtp = "25";
                        r.Pop = "pop.mail.ru";
                        r.Smtp1 = "smtp.mail.ru";
                        break;
                    case "yandex.ru":
                        r.Portpop = "995";
                        r.PortSmtp = "465";
                        r.Pop = "pop.yandex.ru";
                        r.Smtp1 = "smtp.yandex.ru";
                        break;
                    case "gmail.com":
                        r.Portpop = "993";
                        r.PortSmtp = "465";
                        r.Pop = "pop.gmail.com";
                        r.Smtp1 = "smtp.gmail.com";
                        break;
                }
                // Если строки изменили, сохранить в БД
                r.InsertBd(textBox1.Text, textBox2.Text);
                // Добавить проверку на корпоративную почту
                //Подключить БД
                r.ConectionBD();

                //дополнить закрытием формы
            }
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
           
        }
    }
}
