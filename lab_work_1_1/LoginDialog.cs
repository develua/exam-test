using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab_work_1_1
{
    public partial class LoginDialog : Form
    {
        const string login = "admin";
        const string password = "123456";
        ComboBox comboBoxRef;

        public LoginDialog(ComboBox cbr)
        {
            InitializeComponent();
            comboBoxRef = cbr;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == login && textBox2.Text == password)
            {
                SettingsDialog settingsDialog = new SettingsDialog(comboBoxRef);
                Visible = false;
                settingsDialog.ShowDialog();
                Close();
            }
            else
                MessageBox.Show("Не верно введен логин или пароль.", "Ошибка входа", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void LoginDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.OpenForms[0].Visible = true;
        }
    }
}
