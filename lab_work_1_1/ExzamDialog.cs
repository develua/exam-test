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

namespace lab_work_1_1
{
    public partial class ExzamDialog : Form
    {
        List<ExzamQuestion> exzamQuestions;
        Student student;
        List<RadioButton> radioButtons;
        List<CheckBox> checkBoxs;
        List<List<int>> replys;
        int thisIndex;
        double rating;

        public ExzamDialog(List<ExzamQuestion> eq, Student stud)
        {
            InitializeComponent();
            exzamQuestions = eq;
            student = stud;
            thisIndex = 0;
            rating = -1;
            replys = new List<List<int>>(exzamQuestions.Count);

            for (int i = 0; i < exzamQuestions.Count; i++)
                replys.Add(new List<int>());

            radioButtons = new List<RadioButton>()
            {
                radioButton1, radioButton2, radioButton3, radioButton4, radioButton5, radioButton6
            };

            checkBoxs = new List<CheckBox>()
            {
                checkBox1, checkBox2, checkBox3, checkBox4, checkBox5, checkBox6
            };

            toolStripProgressBar1.Maximum = exzamQuestions.Count;

            ShowQuestion();
        }

        void ShowQuestion()
        {
            groupBox1.Text = "Вопрос - " + (thisIndex + 1);
            label1.Text = exzamQuestions[thisIndex].description;

            if(exzamQuestions[thisIndex].type == "radio")
            {
                for (int i = 0; i < exzamQuestions[thisIndex].reply.Count; i++)
                {
                    radioButtons[i].Visible = true;
                    radioButtons[i].Text = exzamQuestions[thisIndex].reply.ElementAt(i).Key;

                    if (replys[thisIndex].Count > 0)
                        if (replys[thisIndex][0] == i)
                            radioButtons[i].Checked = true;
                }
            }
            else if (exzamQuestions[thisIndex].type == "checkbox")
            {
                for (int i = 0; i < exzamQuestions[thisIndex].reply.Count; i++)
                {
                    checkBoxs[i].Visible = true;
                    checkBoxs[i].Text = exzamQuestions[thisIndex].reply.ElementAt(i).Key;

                    if (replys[thisIndex].Count > 0)
                        for (int j = 0; j < replys[thisIndex].Count; j++)
                        {
                            if (replys[thisIndex][j] == i)
                                checkBoxs[i].Checked = true;
                        }
                }
            }
        }

        void ProgresBarStateEdit()
        {
            int countReplys = 0;

            for (int i = 0; i < replys.Count; i++)
                if (replys[i].Count > 0)
                    countReplys++;

            if (countReplys > toolStripProgressBar1.Value)
                toolStripProgressBar1.Value++;
            else if (countReplys < toolStripProgressBar1.Value)
                toolStripProgressBar1.Value--;

            if (toolStripProgressBar1.Value == exzamQuestions.Count)
                button3.Enabled = true;
            else
                button3.Enabled = false;
        }

        void SeveReplys(object sender, EventArgs e)
        {
            if (exzamQuestions[thisIndex].type == "radio")
            {
                replys[thisIndex].Clear();

                for (int i = 0; i < exzamQuestions[thisIndex].reply.Count; i++)
                    if (radioButtons[i].Checked == true)
                    {
                        replys[thisIndex].Add(i);
                        break;
                    }
            }
            else if (exzamQuestions[thisIndex].type == "checkbox")
            {
                replys[thisIndex].Clear();

                for (int i = 0; i < exzamQuestions[thisIndex].reply.Count; i++)
                    if (checkBoxs[i].Checked == true)
                        replys[thisIndex].Add(i);
            }

            ProgresBarStateEdit();
        }

        private void ExzamDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (rating > -1)
            {
                Application.OpenForms[0].Close();
                return;
            }

            if (MessageBox.Show("Вы уверены что хотите закрыть окно?", "Контрольный вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                Application.OpenForms[0].Close();
            else
                e.Cancel = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            thisIndex--;
            button2.Enabled = true;

            if (thisIndex == 0)
                button1.Enabled = false;

            for (int i = 0; i < radioButtons.Count; i++)
            {
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

            if (thisIndex == exzamQuestions.Count - 1)
                button2.Enabled = false;

            for (int i = 0; i < radioButtons.Count; i++)
            {
                radioButtons[i].Visible = false;
                radioButtons[i].Checked = false;
                checkBoxs[i].Visible = false;
                checkBoxs[i].Checked = false;
            }

            ShowQuestion();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            double trueReplyCount = 0, userTrueReplyCount = 0;
            int trueCheckBoxCount;

            XmlTextWriter writer = new XmlTextWriter("result/" + student.fio + " " + student.group + ".xml", Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();
            writer.WriteStartElement("ListReplys");

            for (int i = 0; i < exzamQuestions.Count; i++)
            {
                writer.WriteStartElement("Replys");

                writer.WriteElementString("Question", exzamQuestions[i].description);
                writer.WriteElementString("Type", exzamQuestions[i].type);

                for (int j = 0; j < exzamQuestions[i].reply.Count; j++)
                {
                    writer.WriteStartElement("Reply");

                    if (exzamQuestions[i].reply.ElementAt(j).Value)
                        writer.WriteAttributeString("aright", "true");

                    if (exzamQuestions[i].reply.ElementAt(j).Value)
                        trueReplyCount++;

                    trueCheckBoxCount = 0;

                    for (int h = 0; h < exzamQuestions[i].reply.Count; h++)
                        if (exzamQuestions[i].reply.ElementAt(h).Value)
                            trueCheckBoxCount++;

                    for (int k = 0; k < replys[i].Count; k++)
                        if (replys[i][k] == j)
                        {
                            if (exzamQuestions[i].reply.ElementAt(j).Value)
                            {
                                if (exzamQuestions[i].type == "radio")
                                    userTrueReplyCount++;
                                else if (exzamQuestions[i].type == "checkbox")
                                    userTrueReplyCount += 1.0 / trueCheckBoxCount;
                            }
                            else if (exzamQuestions[i].type == "checkbox")
                                userTrueReplyCount -= 1.0 / exzamQuestions[i].reply.Count;

                            writer.WriteAttributeString("userAright", "true");
                        }

                    writer.WriteString(exzamQuestions[i].reply.ElementAt(j).Key);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            rating = (1000.0 / trueReplyCount) * userTrueReplyCount;
            rating = (rating < 0) ? 0 : rating;

            writer.WriteStartElement("Result");
            writer.WriteString(Convert.ToInt32(rating).ToString());
            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();

            MessageBox.Show("Вы сдали экзамен на " + Convert.ToInt32(rating) + " балов из 1000.", "Результат экзамена", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }
    }
}
