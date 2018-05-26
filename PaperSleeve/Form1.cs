using EnvelopePaper.Class;
using EnvelopePaper.Forms;
using OpenPop.Common.Logging;
using OpenPop.Pop3;
using OpenPop.Pop3.Exceptions;
using OpenPop.Mime;
using OpenPop.Mime.Header;
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
using Message = OpenPop.Mime.Message;

namespace PaperSleeve
{
    public interface IMainForm
    {
        string Content { get; set; }
        void SetSembolCount(int count);
        event EventHandler ContentChanged;
    }


    public partial class Form1 : Form, IMainForm
    {
        private readonly Pop3Client pop3Client;
        private readonly Dictionary<int, Message> messages = new Dictionary<int, Message>();
        //private SaveFileDialog saveFile;

        private void ReceiveMails()
        {
            // Disable buttons while working
            //connectAndRetrieveButton.Enabled = false;
            //uidlButton.Enabled = false;
            //progressBar.Value = 0;

            try
            {
                if (pop3Client.Connected)
                    pop3Client.Disconnect();
                pop3Client.Connect(Con.Pop, int.Parse(Con.Portpop), true);
                pop3Client.Authenticate(Con.Mail, Con.password);
                int count = pop3Client.GetMessageCount();
                //totalMessagesTextBox.Text = count.ToString();
                //messageTextBox.Text = "";
                messages.Clear();
                treeView1.Nodes.Clear();
               // listAttachments.Nodes.Clear();

                int success = 0;
                int fail = 0;
                for (int i = count; i >= 1; i -= 1)
                {
                    // Check if the form is closed while we are working. If so, abort
                    if (IsDisposed)
                        return;

                    // Refresh the form while fetching emails
                    // This will fix the "Application is not responding" problem
                    Application.DoEvents();

                    try
                    {
                        Message message = pop3Client.GetMessage(i);

                        // Add the message to the dictionary from the messageNumber to the Message
                        messages.Add(i, message);

                        // Create a TreeNode tree that mimics the Message hierarchy
                        TreeNode node = new OpenPop.TestApplication.TreeNodeBuilder().VisitMessage(message);

                        // Set the Tag property to the messageNumber
                        // We can use this to find the Message again later
                        node.Tag = i;

                        // Show the built node in our list of messages
                        treeView1.Nodes.Add(node);

                        success++;
                    }
                    catch (Exception e)
                    {
                        DefaultLogger.Log.LogError(
                            "TestForm: Message fetching failed: " + e.Message + "\r\n" +
                            "Stack trace:\r\n" +
                            e.StackTrace);
                        fail++;
                    }

                    //progressBar.Value = (int)(((double)(count - i) / count) * 100);
                }

                MessageBox.Show(this, "Почта получена!\nУспешно: " + success + "\nПровалено: " + fail, "Загрузка Сообщений завершена");
            }
            catch (InvalidLoginException)
            {
                MessageBox.Show(this, "The server did not accept the user credentials!", "POP3 Server Authentication");
            }
            catch (PopServerNotFoundException)
            {
                MessageBox.Show(this, "The server could not be found", "POP3 Retrieval");
            }
            catch (PopServerLockedException)
            {
                MessageBox.Show(this, "The mailbox is locked. It might be in use or under maintenance. Are you connected elsewhere?", "POP3 Account Locked");
            }
            catch (LoginDelayException)
            {
                MessageBox.Show(this, "Login not allowed. Server enforces delay between logins. Have you connected recently?", "POP3 Account Login Delay");
            }
            catch (Exception e)
            {
                MessageBox.Show(this, "Error occurred retrieving mail. " + e.Message, "POP3 Retrieval");
            }
        }




        // Объект для связи с учетками БД
        DataConection Con = new DataConection();

        // Для вложений
        Attachment attData;


        public Form1()
        {
            pop3Client = new Pop3Client();
            InitializeComponent();
            textBox1.TextChanged += TextBox1_TextChanged;
            numericUpDown1.ValueChanged += NumericUpDown1_ValueChanged;
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;

        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (ContentChanged != null) ContentChanged(this, EventArgs.Empty);
        }

        // Для шрифта
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Font = new Font((string)comboBox1.SelectedItem, (float)numericUpDown1.Value);
        }
        // Для размера шрифта
        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
           textBox1.Font = new Font((string)comboBox1.SelectedItem, (float)numericUpDown1.Value);
        }

        // Для обработки текста
        
        public void SetSembolCount(int count)
        {
            lbSymn.Text = count.ToString();
        }

        public string Content
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }

        public event EventHandler ContentChanged;

        //----------------------------------

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
            comboBox1.Items.Add("Calibri");
            comboBox1.Items.Add("Times New Roman");
            comboBox1.Items.Add("Chaparral Pro Light");

            
            // Если в БД нет записей вывести сообщения об отсутствие данных
            // Вывести форму для ввода данных о пользователях
            if ((Con.Mail == null) && (Con.password == null) && (Con.Smtp1 == null)
               && (Con.PortSmtp == null) && (Con.Portpop == null))
            {
                // Форма "Пользователь"
                UserForm FormU = new UserForm();
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
                //прикрепляем вложение
                if (attData != null) Message.Attachments.Add(attData);
                Smtp.Send(Message);

                MessageBox.Show("Сообщение успешно отправлено",
                                "!",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Information);

                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                attData = null; label5.Text = "";
                }
            }
            catch(Exception exx)
            {
                MessageBox.Show(exx.Message+", возможно вы указали неверны адрес почты");
            }
        }

      
        private void button3_Click(object sender, EventArgs e)
        {
          
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.ExitThread();
            Application.Exit();
            
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            webBrowser1.DocumentText = "";
            textBox1.Focus();
            attData = null; label5.Text = "";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.ShowDialog();

            attData = new Attachment(dlg.FileName);
            label5.Text = attData.Name;
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm About = new AboutForm();
            About.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ReceiveMails();
        }

        private static int GetMessageNumberFromSelectedNode(TreeNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            // Check if we are at the root, by seeing if it has the Tag property set to an int
            if (node.Tag is int)
            {
                return (int)node.Tag;
            }

            // Otherwise we are not at the root, move up the tree
            return GetMessageNumberFromSelectedNode(node.Parent);
        }

        private void ListMessagesMessageSelected(object sender, TreeViewEventArgs e)
        {
            // Fetch out the selected message
            Message message = messages[GetMessageNumberFromSelectedNode(treeView1.SelectedNode)];

            // If the selected node contains a MessagePart and we can display the contents - display them
            if (treeView1.SelectedNode.Tag is MessagePart)
            {
                
                MessagePart selectedMessagePart = (MessagePart)treeView1.SelectedNode.Tag;
                if (selectedMessagePart.IsText)
                {
                    
                    
                    
                    // We can show text MessageParts
                    webBrowser1.DocumentText = selectedMessagePart.GetBodyAsText();
                   // webBrowser1.Navigating += WebBrowser1_Navigating;
                    textBox1.Text = selectedMessagePart.GetBodyAsText();

                }
                else
                {
                    // We are not able to show non-text MessageParts (MultiPart messages, images, pdf's ...)
                    textBox1.Text = "<<OpenPop>>Не удается отобразить эту часть сообщения электронной почты. Это не текст<<OpenPop>>";
                }
            }
            else
            {
                
                // If the selected node is not a subnode and therefore does not
                // have a MessagePart in it's Tag property, we genericly find some content to show

                // Find the first text/plain version
                MessagePart plainTextPart = message.FindFirstPlainTextVersion();
                if (plainTextPart != null)
                {
                    // The message had a text/plain version - show that one
                    textBox1.Text = plainTextPart.GetBodyAsText();
                }
                else
                {
                    // Try to find a body to show in some of the other text versions
                    List<MessagePart> textVersions = message.FindAllTextVersions();
                    if (textVersions.Count >= 1)
                        textBox1.Text = textVersions[0].GetBodyAsText();
                    else
                        textBox1.Text = "<<OpenPop>> не могу найти текстовую версию тела в это сообщение, чтобы показать <<OpenPop>>";
                }
            }

            // Clear the attachment list from any previus shown attachments
            treeView2.Nodes.Clear();

            // Build up the attachment list
            List<MessagePart> attachments = message.FindAllAttachments();
            foreach (MessagePart attachment in attachments)
            {
                // Add the attachment to the list of attachments
                TreeNode addedNode = treeView2.Nodes.Add((attachment.FileName));

                // Keep a reference to the attachment in the Tag property
                addedNode.Tag = attachment;
            }

            // Only show that attachmentPanel if there is attachments in the message
            bool hadAttachments = attachments.Count > 0;
            //attachmentPanel.Visible = hadAttachments;

            
        }

        //private void WebBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        //{
        //    System.Windows.Forms.HtmlDocument document =
        //    this.webBrowser1.Document;

        //    if (document != null && document.All["userName"] != null &&
        //    String.IsNullOrEmpty(
        //    document.All["userName"].GetAttribute("value")))
        //    {
        //        e.Cancel = true;
        //        System.Windows.Forms.MessageBox.Show(
        //        "You must enter your name before you can navigate to " +
        //        e.Url.ToString());
        //    }
        //}

        private void ListAttachmentsAttachmentSelected(object sender, TreeViewEventArgs e)
        {
            // Fetch the attachment part which is currently selected
            MessagePart attachment = (MessagePart)treeView2.SelectedNode.Tag;

            if (attachment != null)
            {
                SaveFileDialog sav = new SaveFileDialog();
                sav.FileName = attachment.FileName;
                DialogResult result = sav.ShowDialog();
                if (result != DialogResult.OK)
                    return;

                // Now we want to save the attachment
                FileInfo file = new FileInfo(sav.FileName);

                // Check if the file already exists
                if (file.Exists)
                {
                    // User was asked when he chose the file, if he wanted to overwrite it
                    // Therefore, when we get to here, it is okay to delete the file
                    file.Delete();
                }

                // Lets try to save to the file
                try
                {
                    attachment.Save(file);

                    MessageBox.Show(this, "Вложение успешно сохранено");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Ошибка сохранения вложения: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show(this, "Вложения нет");
            }
        }

        // удаление 
        private void удалитьСообщениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                DialogResult drRet = MessageBox.Show(this, "Are you sure to delete the email?", "Delete email", MessageBoxButtons.YesNo);
                if (drRet == DialogResult.Yes)
                {
                    int messageNumber = GetMessageNumberFromSelectedNode(treeView1.SelectedNode);
                    pop3Client.DeleteMessage(messageNumber);

                    treeView1.Nodes[messageNumber].Remove();

                    drRet = MessageBox.Show(this, "Do you want to receive email again (this will commit your changes)?", "Receive email", MessageBoxButtons.YesNo);
                    if (drRet == DialogResult.Yes)
                        ReceiveMails();
                }
            }
        }
    }
    

}

