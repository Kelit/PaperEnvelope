using System;
using System.Data.Common;
using System.Data.SQLite;
using System.Windows.Forms;

namespace EnvelopePaper.Class
{
    // Данный модуль необходим для подключения БД к таблице Service
    // Сохранение изменений типа шрифта и размер и обновление
    class ServiceConnecting
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
        public string Font1 { get; set; }
        // Пароль от почты
        public string Size1 { set; get; }


        // Метод подключения к бд
        public void ServiceConnector()
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
                SQLiteCommand command = new SQLiteCommand("SELECT * FROM 'Service';", m_dbConn);
                SQLiteDataReader reader = command.ExecuteReader();
                // в цикле заносим в глобальные переменные данные из таблицы
                foreach (DbDataRecord record in reader)
                {
                    Font1 = record["Font"].ToString();
                    Size1 = record["Size"].ToString();
                }

                if((Font1=="")&&(Size1==""))
                {
                    Font1 = "Arial";
                    Size1 = "12";
                }


                m_dbConn.Close();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Обновление данных в БД о шрифте и размере
        public void ServiceRefresh(object con)
        {
            //Открытие БД на добавление пользователя
            ServiceConnecting r = (ServiceConnecting)con;
            dbFileName = Application.StartupPath + @"\MailData.db";
            m_dbConn = new SQLiteConnection();
            m_sqlCmd = new SQLiteCommand();
            m_dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
            m_dbConn.Open();
            m_sqlCmd.Connection = m_dbConn;
            // выполняем команду закинуть информацию в поля таблицы  MailInfo
            SQLiteCommand command = new SQLiteCommand
                ("INSERT INTO 'Service' ('Font', 'Size') VALUES ('" + r.Font1+ "','" + r.Size1 + "');",
                m_dbConn);
            command.ExecuteNonQuery();
            m_dbConn.Close();
        }
    }
}
