using System;
using System.Windows.Forms;
namespace ServerClient
{
    public partial class AppSettingsForm : Form
    {
        public AppSettingsForm()
        {
            InitializeComponent();
            PortNumbertxt.Text = AppSettings.Port.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (PortNumbertxt.IntValue > 0 && PortNumbertxt.IntValue < ushort.MaxValue)
            {
                AppSettings.Port = PortNumbertxt.IntValue;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;

            }
            else if (PortNumbertxt.IntValue > ushort.MaxValue)
            {
                MessageBox.Show("Port can not be greater than " + ushort.MaxValue);
                return;
            }
            else
            {
                MessageBox.Show("Port can not be less than or equal to zero");

            }
             
            
        }

    }
}
