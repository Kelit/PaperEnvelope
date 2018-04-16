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
        // Глобальные переменные из Метода Load Form1
        string Mail;
        string password;

        // Объект для связи с БД
        DataConection r;


        public UserForm()
        {
            InitializeComponent();
        }

       

        //Метод нужен для обрашения к Form1 и забору данных
        public void LoadData(object con)
        {
            r = (DataConection)con;
            Mail = r.Mail;
            password = r.password;
            listBox1.Items.Add("MAIL = " + r.Mail);
            listBox1.Items.Add("PASWWORD = " + r.password);
            listBox1.Items.Add("POP = " + r.Pop);
            listBox1.Items.Add("PORT POP = " + r.Portpop);
            listBox1.Items.Add("SMTP = " + r.Smtp1);
            listBox1.Items.Add("PORT SMTP = " + r.PortSmtp);
        }

        private void UserForm_Load(object sender, EventArgs e)
        {
            textBox1.Text =Mail;
            textBox2.Text =password;
            //pictureBox1.ImageLocation =
            
            //button1.Visible = false;


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
                password = textBox2.Text;
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
                
                r.Mail = Mail;
                r.password = password;
                // Добавить проверку на корпоративную почту
                // Если нету подходящей почты из  switch (ShortMail)


                // Если строки изменили, сохранить в БД
                r.InsertBd(r);
                
                //Подключить БД
                r.ConectionBD();
                //дополнить закрытием формы, пока так 
                //поиск нормальной реализации
                this.Close();
            }

            //дополнить закрытием формы, пока так 
            //поиск нормальной реализации
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
           
        }

        // Удаление пользователя 
        private void button2_Click(object sender, EventArgs e)
        {
            r.Delet();
            r.Mail = null;
            r.password = null;
            r.Pop = null;
            r.Portpop = null;
            r.PortSmtp = null;
            r.Smtp1 = null;
            if (MessageBox.Show("Данные о пользователе удалены",
                "ВНИМАНИЕ",
                MessageBoxButtons.OK,
                MessageBoxIcon.Question) == DialogResult.OK)
            {
                this.Close();
            }
            button1.Visible = true;
        }
    }
}
