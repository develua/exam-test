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

namespace lab_work_1_1
{
    public partial class SettingsDialog : Form
    {
        SettingsProgram settingsProgram = new SettingsProgram();

        public SettingsDialog(ComboBox cbf)
        {
            InitializeComponent();

            for (int i = 0; i < cbf.Items.Count; i++)
                comboBox1.Items.Add(cbf.Items[i]);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == openFileDialog1.ShowDialog())
            {
                textBox1.Text = openFileDialog1.FileName;
                settingsProgram.studenFile = textBox1.Text;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == openFileDialog1.ShowDialog())
            {
                textBox2.Text = openFileDialog1.FileName;
                settingsProgram.exzamFile = textBox2.Text;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (settingsProgram.studenFile != null && settingsProgram.exzamFile != null)
            {
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    using (FileStream fs = new FileStream("data/settings.bin", FileMode.Create))
                    {
                        formatter.Serialize(fs, settingsProgram);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                Application.OpenForms[0].Visible = true;
                Close();
            }
            else
                MessageBox.Show("Вы не выбрали нудные файлы.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);           
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Вы не выбрали пользователя.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Student stud = (Student)comboBox1.SelectedItem;

            if (!File.Exists("result/" + stud.fio + " " + stud.group + ".xml"))
            {
                MessageBox.Show("Файл не найден для выбранного студента.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ExzamResults exzamResults = new ExzamResults(this, (Student)comboBox1.SelectedItem);
            Visible = false;
            exzamResults.ShowDialog();
        }
    }
}
