using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data.SQLite;
using System.Windows.Forms;
using PaperSleeve;
using System.Diagnostics;

namespace EnvelopePaper.Class
{
   // Данный модуль необходим для подключения к БД, обновлению и вставки данных
    class DataConection
    {
        // Для подключения БД
        // Адрес БД
        private String dbFileName;
        // Конект к БД
        private SQLiteConnection m_dbConn;
        // Команды бд
        private SQLiteCommand m_sqlCmd;

        // Переменные для связи с БД
        // Почтовый адрес
        public string Mail { get; set; }
        // Пароль от почты
        public string password { set; get; }
        // Порт 
        public string Smtp1 { set; get; }
        // номер порта
        public string PortSmtp { set; get; }
        // Порт для приёма
        public string Pop { set; get; }
        // Номер порта приёма
        public string Portpop { set; get; }

        // Подключение к бд
        public void ConectionBD()
        {
            // Проверка есть ли подключение к БД
            dbFileName = Application.StartupPath + @"\MailData.db";
            m_dbConn = new SQLiteConnection();
            m_sqlCmd = new SQLiteCommand();
            try
            {
                m_dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
                m_dbConn.Open();
                m_sqlCmd.Connection = m_dbConn;
                // выполняем команду считать все поля из таблицы  MailInfo
                SQLiteCommand command = new SQLiteCommand("SELECT * FROM 'MailInfo';", m_dbConn);
                SQLiteDataReader reader = command.ExecuteReader();
                // в цикле заносим в глобальные переменные данные из таблицы
                foreach (DbDataRecord record in reader)
                {
                    Mail = record["Email"].ToString();
                    password = record["Password"].ToString();
                    Smtp1 = record["Smtp"].ToString();
                    PortSmtp = record["portSmtp"].ToString();
                    Pop = record["Pop"].ToString();
                    Portpop = record["portPop"].ToString();
                }
                Debug.Print("Connected");
                m_dbConn.Close();
            }
            catch (SQLiteException ex)
            {
               MessageBox.Show("Error: " + ex.Message);
            }
            // Если в БД нет записей вывести сообщения об отсутствие данных
            // Вывести форму для ввода данных о пользователях
            if ((Mail == null) && (password == null) && (Smtp1 == null)
               && (PortSmtp == null) && (Portpop == null))
            {
                // Форма "Пользователь"
                UserForm FormU = new UserForm();
                Debug.Print("Disconnected");
                MessageBox.Show(" Данные по пользователю отсутствуют",
                "Внимание",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
                FormU.ShowDialog();
            }
        }

        // Вставка в БД
        public void InsertBd(string Mail,string Password)
        {
            //Открытие БД на добавление пользователя
            dbFileName = Application.StartupPath + @"\MailData.db";
            m_dbConn = new SQLiteConnection();
            m_sqlCmd = new SQLiteCommand();
            m_dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
            m_dbConn.Open();
            m_sqlCmd.Connection = m_dbConn;
            // выполняем команду закинуть информацию в поля таблицы  MailInfo
            SQLiteCommand command = new SQLiteCommand
                ("INSERT INTO 'MailInfo' ('Email', 'Password', 'Smtp', 'portSmtp', 'Pop', 'portPop') VALUES ('"+Mail+"','"+Password+"','"+ Smtp1 + "'," + PortSmtp + " , '"+Pop+"','"+Portpop+"');",
                m_dbConn);
            command.ExecuteNonQuery();
            m_dbConn.Close();
        }

    }
}
