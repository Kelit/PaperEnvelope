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

        //// Глобальные переменные из Метода Load Form1
        string Mail;
        string password;


        DataConection r = new DataConection();

        // Переменные для связи с БД
        // Порт 
        public string Smtp1 { set; get; }
        // номер порта
        public string PortSmtp { set; get; }
        // Порт для приёма
        public string Pop { set; get; }
        // Номер порта приёма
        public string Portpop { set; get; }

        ////Адрес БД
        //string dbFileName;
        //// Конект к БД
        //private SQLiteConnection Connect;
        //// Команды бд
        //private SQLiteCommand SqlCmd;



        //Метод нужен для обрашения к Form1 и забору данных
        public void LoadData(string Mail_, string password_)
        {
            Mail = Mail_;
            password = password_;
        }

        //// Метод получает информацию о соединение из исключения, если в БД нету информации о пользователи
        //public void LoadData(string dbFN, SQLiteConnection m_dbConn, SQLiteCommand m_sqlCmd)
        //{
        //    
        //}


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
                // Если строки изменили, сохранить в БД
                r.InsertBd(textBox1.Text,textBox2.Text);
                //Подключить БД
                r.ConectionBD();

                //дополнить закрытием формы
            }
            
        }

        

        private void timer1_Tick(object sender, EventArgs e)
        {
            //if ((textBox1.Text != CMail) || (textBox1.Text != password))
            //{
            //    button1.Visible = true;
            //}
        }

       
    }
}
