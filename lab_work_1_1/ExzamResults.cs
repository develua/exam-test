using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace lab_work_1_1
{
    public partial class ExzamResults : Form
    {
        List<ExzamQuestionRes> exzamQuestionRes;
        SettingsDialog parentDialog;
        List<CheckBox> checkBoxs;
        List<RadioButton> radioButtons;
        List<Label> checkLable;
        Student student;
        int thisIndex;

        public ExzamResults(SettingsDialog sdr, Student stud)
        {
            InitializeComponent();
            parentDialog = sdr;
            student = stud;
            thisIndex = 0;
            exzamQuestionRes = new List<ExzamQuestionRes>();

            checkBoxs = new List<CheckBox>()
            {
                checkBox1, checkBox2, checkBox3, checkBox4, checkBox5, checkBox6
            };

            radioButtons = new List<RadioButton>()
            {
                radioButton1, radioButton2, radioButton3, radioButton4, radioButton5, radioButton6
            };

            checkLable = new List<Label>()
            {
                checkLable1, checkLable2, checkLable3, checkLable4, checkLable5, checkLable6
            };

            LoadListReplys();
            ShowQuestion();
        }

        private void ExzamResults_FormClosing(object sender, FormClosingEventArgs e)
        {
            parentDialog.Visible = true;
        }

        void LoadListReplys()
        {
            label2.Text = student.fio;
            label3.Text = student.group;

            try
            {
                XPathDocument document = new XPathDocument("result/" + student.fio + " " + student.group + ".xml");
                XPathNavigator navigator = document.CreateNavigator();
                XPathNodeIterator nodes, temp;

                string description, type;
                Dictionary<string, bool[]> reply;
                bool[] boolReply;

                nodes = navigator.Select("/ListReplys/Replys");

                while (nodes.MoveNext())
                {
                    reply = new Dictionary<string, bool[]>();
                        
                    temp = nodes.Current.Select("Question");
                    temp.MoveNext();
                    description = temp.Current.Value;

                    temp = nodes.Current.Select("Type");
                    temp.MoveNext();
                    type = temp.Current.Value;

                    temp = nodes.Current.Select("Reply");
                    for (int i = 0; i < temp.Count; i++)
                    {
                        boolReply = new bool[2];
                        temp.MoveNext();

                        if (string.Compare(temp.Current.GetAttribute("aright", ""), "true", true) == 0)
                            boolReply[0] = true;
                        else
                            boolReply[0] = false;

                        if (string.Compare(temp.Current.GetAttribute("userAright", ""), "true", true) == 0)
                            boolReply[1] = true;
                        else
                            boolReply[1] = false;

                        reply.Add(temp.Current.Value, boolReply);
                    }

                    exzamQuestionRes.Add(new ExzamQuestionRes(description, type, reply));
                }

                nodes = navigator.Select("/ListReplys/Result");
                nodes.MoveNext();
                label4.Text = nodes.Current.Value;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void ShowQuestion()
        {
            groupBox2.Text = "Вопрос - " + (thisIndex + 1);
            label1.Text = exzamQuestionRes[thisIndex].description;

            if (exzamQuestionRes[thisIndex].type == "radio")
            {
                for (int i = 0; i < exzamQuestionRes[thisIndex].reply.Count; i++)
                {
                    radioButtons[i].Visible = true;
                    checkLable[i].Visible = true;
                    checkLable[i].Text = exzamQuestionRes[thisIndex].reply.ElementAt(i).Key;

                    if (exzamQuestionRes[thisIndex].reply.ElementAt(i).Value[0])
                        checkLable[i].ForeColor = Color.Green;

                    if (exzamQuestionRes[thisIndex].reply.ElementAt(i).Value[1])
                        radioButtons[i].Checked = true;
                }
            }
            else if (exzamQuestionRes[thisIndex].type == "checkbox")
            {
                for (int i = 0; i < exzamQuestionRes[thisIndex].reply.Count; i++)
                {
                    checkBoxs[i].Visible = true;
                    checkLable[i].Visible = true;
                    checkLable[i].Text = exzamQuestionRes[thisIndex].reply.ElementAt(i).Key;

                    if (exzamQuestionRes[thisIndex].reply.ElementAt(i).Value[0])
                        checkLable[i].ForeColor = Color.Green;

                    if (exzamQuestionRes[thisIndex].reply.ElementAt(i).Value[1])
                        checkBoxs[i].Checked = true;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            thisIndex--;
            button2.Enabled = true;

            if (thisIndex == 0)
                button1.Enabled = false;

            for (int i = 0; i < checkLable.Count; i++)
            {
                checkLable[i].Visible = false;
                checkLable[i].ForeColor = Color.Black;
                radioButtons[i].Visible = false;
                radioButtons[i].Checked = false;
                checkBoxs[i].Visible = false;
                checkBoxs[i].Checked = false;
            }

            ShowQuestion();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            thisIndex++;
            button1.Enabled = true;

            if (thisIndex == exzamQuestionRes.Count - 1)
                button2.Enabled = false;

            for (int i = 0; i < checkLable.Count; i++)
            {
                checkLable[i].Visible = false;
                checkLable[i].ForeColor = Color.Black;
                radioButtons[i].Visible = false;
                radioButtons[i].Checked = false;
                checkBoxs[i].Visible = false;
                checkBoxs[i].Checked = false;
            }

            ShowQuestion();
        }
    }

    class ExzamQuestionRes : ExzamQuestion
    {
        public new Dictionary<string, bool[]> reply { get; private set; }

        public ExzamQuestionRes(string d, string t, Dictionary<string, bool[]> r) : base(d, t, null)
        {
            reply = r;
        }
    }
}