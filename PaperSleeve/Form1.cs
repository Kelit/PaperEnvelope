using EnvelopePaper.Class;
using OpenPop.Pop3;
using PaperSleeve;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaperSleeve
{
    public partial class Form1 : Form
    {
        // Объект для связи с БД
        DataConection Con = new DataConection();

        public Form1()
        {
            InitializeComponent();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Вы уверены что хотите выйти ?",
                "Предупреждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(" Очистить ?",
                "Предупреждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                textBox1.Clear();
            }
        }

        private void пользовательToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Форма пользоветеля
            UserForm FormU = new UserForm();
            // Проверка есть ли данные в БД
            if ((Con.Mail != null) && (Con.password != null) && (Con.Smtp1 != null)
                && (Con.PortSmtp != null) && (Con.Portpop != null))
            {
                FormU.LoadData(Con);
                FormU.Show();
            }
            else
            {
                // Открытие формы, если нет поля будут пустыми
                MessageBox.Show(" Данные по пользователю отсутствуют",
                "Внимание",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
                FormU.LoadData(Con);
                FormU.Show();
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Con.ConectionBD();
            // Если в БД нет записей вывести сообщения об отсутствие данных
            // Вывести форму для ввода данных о пользователях
            if ((Con.Mail == null) && (Con.password == null) && (Con.Smtp1 == null)
               && (Con.PortSmtp == null) && (Con.Portpop == null))
            {
                // Форма "Пользователь"
                UserForm FormU = new UserForm();
                Debug.Print("Disconnected");
                MessageBox.Show(" Данные по пользователю отсутствуют",
                "Внимание",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
                FormU.LoadData(Con);
                FormU.ShowDialog();
            }
            
        }


        private  void button1_Click(object sender, EventArgs e)
        {
            // временное решение
           // Con.ConectionBD();
            // Свойство PortSmtp хранит тип string для нормальной работы
            // необходим int, временной решение
            int ps = Convert.ToInt32(Con.PortSmtp);

            try
            {
               using (SmtpClient Smtp = new SmtpClient(Con.Smtp1,ps))
               { 
                Smtp.Credentials = new NetworkCredential(Con.Mail, Con.password);
                MailMessage Message = new MailMessage();
                Message.From = new MailAddress(Con.Mail);
                Message.To.Add(new MailAddress(textBox3.Text));
                Smtp.EnableSsl = true;
                Message.Subject = textBox2.Text;
                Message.Body = textBox1.Text;
                Smtp.Send(Message);

                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                }
            }
            catch(Exception exx)
            {
                MessageBox.Show(exx.Message);
            }
        }

        // приём сообщений, реализовано через OpenPop
        private void button3_Click(object sender, EventArgs e)
        {
            int pp = Convert.ToInt32(Con.Portpop);

            var client = new OpenPop.Pop3.Pop3Client();
            client.Connect(Con.Pop, pp, true);
            client.Authenticate(Con.Mail, Con.password, OpenPop.Pop3.AuthenticationMethod.UsernameAndPassword);
            int countMessage = client.GetMessageCount();
            var count = client.GetMessageCount();

            var  message = client.GetMessage(count);
            listBox1.Items.Add(message.Headers.Subject);// заголовок
            listBox1.Items.Add(message.Headers.From);//от кого
            listBox1.Items.Add(message.Headers.DateSent);//Дата/Время
           // textBox1.Text = message.Headers.ContentType;
        }

        private void сервисToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Опять проверка из БД ?
            // Открытие формы, если нет поля будут пустыми 
            //Форма "Сервис"
            Service ServiceF = new Service();
            ServiceF.Show();

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.ExitThread();
            Application.Exit();
            
        }
        //public static List<Message> FetchAllMessages(string hostname, int port, bool useSsl, string username, string password)
        //{
        //    // Используем using чтобы соединение автоматически закрывалось
        //    using (Pop3Client client = new Pop3Client())
        //    {

        //        //110 or 995
        //        //pop.mail.ru
        //        //imap.mail.ru 993
        //        // Подключение к серверу
        //        client.Connect(hostname, port, useSsl);

        //        // Аутентификация (проверка логина и пароля)
        //        client.Authenticate(username, password);

        //        // Получение количества сообщений в почтовом ящике
        //        int messageCount = client.GetMessageCount();

        //        // Выделяем память под список сообщений. Мы хотим получить все сообщения
        //        List<Message> allMessages = new List<Message>(messageCount);

        //        // Сообщения нумеруются от 1 до messageCount включительно
        //        // Другим языком нумерация начинается с единицы
        //        // Большинство серверов присваивают новым сообщениям наибольший номер (чем меньше номер тем старее сообщение)
        //        // Т.к. цикл начинается с messageCount, то последние сообщения должны попасть в начало списка
        //        for (int i = messageCount; i > 0; i--)
        //        {
        //            allMessages.Add(client.GetMessage(i));
        //        }

        //        // Возвращаем список сообщений
        //        return allMessages;
        //    }
    }
    

}

