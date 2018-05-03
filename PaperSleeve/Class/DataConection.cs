using System;
using System.Data.Common;
using System.Data.SQLite;
using System.Windows.Forms;


namespace EnvelopePaper.Class
{
    // Данный модуль необходим для подключения к БД, обновлению
    // удалению и вставки данных о пользователе
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
                m_dbConn.Close();
            }
            catch (SQLiteException ex)
            {
               MessageBox.Show("Error: " + ex.Message);
            }
            
        }

        // Вставка в БД
        public void InsertBd(object con)
        {
            //Открытие БД на добавление пользователя
            DataConection r = (DataConection)con;
            dbFileName = Application.StartupPath + @"\MailData.db";
            m_dbConn = new SQLiteConnection();
            m_sqlCmd = new SQLiteCommand();
            m_dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
            m_dbConn.Open();
            m_sqlCmd.Connection = m_dbConn;
            // выполняем команду закинуть информацию в поля таблицы  MailInfo
            SQLiteCommand command = new SQLiteCommand
                ("INSERT INTO 'MailInfo' ('Email', 'Password', 'Smtp', 'portSmtp', 'Pop', 'portPop') VALUES ('"+r.Mail+"','"+r.password+"','"+ r.Smtp1 + "'," + r.PortSmtp + " , '"+r.Pop+"','"+r.Portpop+"');",
                m_dbConn);
            command.ExecuteNonQuery();
            m_dbConn.Close();
        }

        // Метод удаления поля из бд
        public void Delet()
        {
            //Открытие БД на удаление
            dbFileName = Application.StartupPath + @"\MailData.db";
            m_dbConn = new SQLiteConnection();
            m_sqlCmd = new SQLiteCommand();
            m_dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
            m_dbConn.Open();
            m_sqlCmd.Connection = m_dbConn;
            // выполняем команду удалить пользователя из  MailInfo
            SQLiteCommand command = new SQLiteCommand
                ("DELETE FROM MailInfo", m_dbConn);
            command.ExecuteNonQuery();
            m_dbConn.Close();
        }

    }
}
