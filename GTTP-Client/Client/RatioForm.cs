using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using httpMethodsApp;
namespace ClientRaw
{
    public partial class RatioForm : Form
    {
        
        private double[] _ratios;
        private RatioTypeEnum _type;
        private double sum
        {
            get { return txtA.DoubleValue + txtC.DoubleValue + txtG.DoubleValue + txtT.DoubleValue; }

        }
        public RatioTypeEnum RatioType
        {
            get { return _type; }
            set { _type = value; }
        }
        public double[] ratios
        {
            get { return this._ratios; }
            set { _ratios = value; }
        }

        public RatioForm()
        {
            InitializeComponent();
            randomRatios(FormatData.specialChars.Length);
        }
       
        private void randomRatios(int length)
        {
            Random rand = new Random();
            _ratios = new double[length];
            double charCountRatioSum = 0;
            for (int index = 0; index < length; index++)
            {
                double ratio = rand.NextDouble();
                if (ratio < 0.001)
                {
                    ratio = 0.1;
                }
                _ratios[index] = ratio;
                charCountRatioSum += ratio;
            }


            //Normalizing the ratios to be summed to one
            for (int index = 0; index < length; index++)
            {

                _ratios[index] /= charCountRatioSum;
            }
        }
         
        private void autoRdBtn_CheckedChanged(object sender, EventArgs e)
        {
            _type = RatioTypeEnum.Automatic;
            enableTextBox(false);

        }

        private void rdManualBtn_CheckedChanged(object sender, EventArgs e)
        {
            _type = RatioTypeEnum.Manual;
            enableTextBox(true);

        }


        private void enableTextBox(bool enable)
        {
            txtA.Enabled = enable;
            txtC.Enabled = enable;
            txtG.Enabled = enable;
            txtT.Enabled = enable;
        }

        private void displayRatios()
        {
            txtA.Text = _ratios[0].ToString();
            txtC.Text = _ratios[1].ToString();
            txtG.Text = _ratios[2].ToString();
            txtT.Text = _ratios[3].ToString();
        }
        private void saveRatios()
        {
            _ratios[0] = txtA.DoubleValue;
            _ratios[1] = txtC.DoubleValue;
            _ratios[2] = txtG.DoubleValue;
            _ratios[3] = txtT.DoubleValue;

            
        }
        private void clearText()
        {
            txtA.Text = "";
            txtC.Text = "";
            txtG.Text = "";
            txtT.Text = "";
        }

        private void RatioForm_Load(object sender, EventArgs e)
        {
            if (_type == RatioTypeEnum.Automatic)
            {

                enableTextBox(false);
                clearText();

            }
            else
            {
                rdManualBtn.Checked = true;
                enableTextBox(true);
                displayRatios();

            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            
            if (_type == RatioTypeEnum.Manual)
            {
                if (Math.Abs(sum - 1) > 0.0000000001)
                {
                    MessageBox.Show("Total ratio must be equal 1.0 ");
                    return;

                }
                saveRatios();
            }
          
           this.DialogResult  = System.Windows.Forms.DialogResult.OK;
           this.Close();
        }

        private void texBox_TextChanged(object sender, EventArgs e)
        {
            NumericTextBox textBox = (NumericTextBox)sender;

            if (textBox.DoubleValue > 1.0)
            {
                MessageBox.Show("The value can not be more than 1.0");
               // txtA.Text = "";
                textBox.Text = "";
                return;
            }
            if (textBox.DoubleValue < 0)
            {
                MessageBox.Show("The value can not be less than 0.0");
                textBox.Text = "";
                return;
            }

            lbSum.Text = sum.ToString();
           
        }





     

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       
    }
}
