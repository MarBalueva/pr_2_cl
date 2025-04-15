using System;
using System.Windows.Forms;

namespace pr_2_cl
{
    public partial class MainForm : Form
    {
        private readonly FileClient client = new FileClient();

        public MainForm()
        {
            InitializeComponent();
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFilePath.Text))
            {
                MessageBox.Show("Пожалуйста, выберите файл.");
                return;
            }

            try
            {
                btnSend.Enabled = false;
                string result = await client.SendFileAsync(txtFilePath.Text);
                txtResponse.Text = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
            finally
            {
                btnSend.Enabled = true;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtFilePath.Text = ofd.FileName;
                }
            }
        }
    }

}
