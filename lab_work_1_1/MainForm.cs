using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;

namespace lab_work_1_1
{
    public partial class MainForm : Form
    {
        SettingsProgram settingsProgram;
        List<ExzamQuestion> exzamQuestions = new List<ExzamQuestion>();

        public MainForm()
        {
            InitializeComponent();
        }

        void LoadSettings()
        {
            if (File.Exists("data/settings.bin"))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream fs = new FileStream("data/settings.bin", FileMode.Open))
                {
                    if (fs.Length != 0)
                        settingsProgram = (SettingsProgram)formatter.Deserialize(fs);
                    else
                        settingsProgram = new SettingsProgram();
                }
            }
            else
                settingsProgram = new SettingsProgram();
        }

        bool ValidateXml(string path)
        {
            XDocument xDocument = XDocument.Load(path);
            XmlSchemaSet schemaSet = new XmlSchemaSet();

            XPathDocument document = new XPathDocument(path);
            XPathNavigator navigator = document.CreateNavigator();
            XPathNodeIterator nodes;

            nodes = navigator.Select("/*/@noNamespaceSchemaLocation");
            nodes.MoveNext();

            schemaSet.Add(null, "data/" + nodes.Current.Value);

            try
            {
                xDocument.Validate(schemaSet, null);
                return true;
            }
            catch
            {
                return false;
            }
        }

        void LoadStudens()
        {
            if (File.Exists(settingsProgram.studenFile))
            {
                if (ValidateXml(settingsProgram.studenFile))
                {
                    XPathDocument document = new XPathDocument(settingsProgram.studenFile);
                    XPathNavigator navigator = document.CreateNavigator();
                    XPathNodeIterator nodes = null, temp;
                    string fio, pass, group;

                    comboBox1.Items.Clear();

                    nodes = navigator.Select("/ListStudents/student");
                    while (nodes.MoveNext())
                    {
                        temp = nodes.Current.Select("fio");
                        temp.MoveNext();
                        fio = temp.Current.Value;

                        temp = nodes.Current.Select("password");
                        temp.MoveNext();
                        pass = temp.Current.Value;

                        temp = nodes.Current.Select("group");
                        temp.MoveNext();
                        group = temp.Current.Value;

                        comboBox1.Items.Add(new Student(fio, pass, group));
                    }
                }
            }
        }

        void LoadExzam()
        {
            if (File.Exists(settingsProgram.exzamFile))
            {
                if (ValidateXml(settingsProgram.exzamFile))
                {
                    XPathDocument document = new XPathDocument(settingsProgram.exzamFile);
                    XPathNavigator navigator = document.CreateNavigator();
                    XPathNodeIterator nodes, temp;
                    string description, type;
                    Dictionary<string, bool> reply;

                    exzamQuestions.Clear();
                    nodes = navigator.Select("/ListQuestions/question");

                    while (nodes.MoveNext())
                    {
                        reply = new Dictionary<string, bool>();

                        temp = nodes.Current.Select("description");
                        temp.MoveNext();
                        description = temp.Current.Value;

                        temp = nodes.Current.Select("type");
                        temp.MoveNext();
                        type = temp.Current.Value;

                        temp = nodes.Current.Select("reply");
                        while (temp.MoveNext())
                        {
                            if (string.Compare(temp.Current.GetAttribute("aright", ""), "true", true) == 0)
                                reply.Add(temp.Current.Value, true);
                            else
                                reply.Add(temp.Current.Value, false);
                        }

                        exzamQuestions.Add(new ExzamQuestion(description, type, reply));
                    }
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LoginDialog adminGialog = new LoginDialog(comboBox1);
            Visible = false;
            adminGialog.ShowDialog();
        }

        private void MainForm_VisibleChanged(object sender, EventArgs e)
        {
            try
            {
                LoadSettings();
                LoadStudens();
                LoadExzam();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (exzamQuestions.Count > 0)
            {
                if (comboBox1.SelectedItem == null)
                {
                    MessageBox.Show("Вы не выбрали пользователя.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (textBox1.Text == "Ваш пароль")
                {
                    MessageBox.Show("Вы не ввели ваш пароль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (((Student)comboBox1.SelectedItem).password == textBox1.Text)
                {
                    ExzamDialog exzamDialog = new ExzamDialog(exzamQuestions, ((Student)comboBox1.SelectedItem));
                    Visible = false;
                    exzamDialog.ShowDialog();
                    
                }
                else
                    MessageBox.Show("Вы ввели не верный пароль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
                MessageBox.Show("Экзаменационные вопросы еще не загружены.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Ваш пароль")
            {
                textBox1.Text = "";
                textBox1.UseSystemPasswordChar = true;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text = "Ваш пароль";
                textBox1.UseSystemPasswordChar = false;
            }
        }
    }

    [Serializable]
    public class SettingsProgram
    {
        public string exzamFile { get; set; }
        public string studenFile { get; set; }
    }

    public class Student
    {
        public string fio { get; private set; }
        public string password { get; private set; }
        public string group { get; private set; }

        public Student(string f, string p, string g)
        {
            fio = f;
            password = p;
            group = g;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", fio, group);
        }
    }

    public class ExzamQuestion
    {
        public string description { get; private set; }
        public string type { get; private set; }
        public Dictionary<string, bool> reply { get; private set; }

        public ExzamQuestion(string d, string t, Dictionary<string, bool> r)
        {
            description = d;
            type = t;
            reply = r;
        }
    }
}
