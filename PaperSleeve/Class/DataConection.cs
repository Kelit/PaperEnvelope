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
        //Мыло
        public string Mail { get; set; }
        //Пароль от мыла
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

            if ((Mail == null) && (password == null) && (Smtp1 == null)
               && (PortSmtp == null) && (Portpop == null))
            {
                //Форма "Пользователь"
                UserForm FormU = new UserForm();
                Debug.Print("Disconnected");
                MessageBox.Show(" Данные по пользователю отсутствуют",
                "Внимание",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
                //FormU.LoadData(dbFileName, m_dbConn, m_sqlCmd);
                FormU.ShowDialog();

                //var f = new UserForm();
               // f.Owner = this;
                // главная форма будет недоступна
                //f.ShowDialog();
            }


        }

        // Вставка в БД
        public void InsertBd(string Mail,string Password)
        {
            // Тут нужно добавить парсер smtp и pop по мылу типа mail.ru,yandex.ru,gmail.com
            int istart = Mail.IndexOf("@", StringComparison.InvariantCultureIgnoreCase);
            if (istart < 0)
                MessageBox.Show("Вы ввели не правильный адрес");

            string Malo = Mail.Remove(0, istart+1);

            switch (Malo)
            {
                case "mail.ru":
                    Portpop = "995";
                    PortSmtp = "25";
                    Pop = "pop.mail.ru";
                    Smtp1 = "stmp.mail.ru";
                    break;
                case "yandex.ru":
                    Portpop = "995";
                    PortSmtp = "465";
                    Pop = "pop.yandex.ru";
                    Smtp1 = "stmp.yandex.ru";
                    break;
                case "gmail.com":
                    Portpop = "993";
                    PortSmtp = "465";
                    Pop = "pop.gmail.com";
                    Smtp1 = "stmp.gmail.com";
                    break;
            }


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
