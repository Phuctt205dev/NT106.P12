namespace Calculator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        float data1, data2;
        string pheptinh;

        private void button1_Click(object sender, EventArgs e)
        {
            hienthi2.Text = hienthi2.Text + "7";
        }

        private void button9_Click(object sender, EventArgs e)
        {
            hienthi2.Text = hienthi2.Text + "5";
        }

        private void button18_Click(object sender, EventArgs e)
        {
            hienthi2.Text = hienthi2.Text + "0";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            hienthi2.Text = hienthi2.Text + "4";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            hienthi2.Text = hienthi2.Text + "8";
        }

        private void button14_Click(object sender, EventArgs e)
        {
            hienthi2.Text = hienthi2.Text + "1";
        }

        private void button13_Click(object sender, EventArgs e)
        {
            hienthi2.Text = hienthi2.Text + "2";
        }

        private void button12_Click(object sender, EventArgs e)
        {
            hienthi2.Text = hienthi2.Text + "3";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            hienthi2.Text = hienthi2.Text + "6";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            hienthi2.Text = hienthi2.Text + "9";
        }

        private void button16_Click(object sender, EventArgs e)
        {
            hienthi2.Text = hienthi2.Text + ".";
        }

        private void button15_Click(object sender, EventArgs e)
        {
            pheptinh = "cong";
            data1 = float.Parse(hienthi2.Text);
            hienthi2.Clear();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            pheptinh = "nhan";
            data1 = float.Parse(hienthi2.Text);
            hienthi2.Clear();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            pheptinh = "tru";
            data1 = float.Parse(hienthi2.Text);
            hienthi2.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            pheptinh = "chia";
            data1 = float.Parse(hienthi2.Text);
            hienthi2.Clear();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            hienthi2.Clear();
            hienthi1.Clear();
        }

        private void button19_Click(object sender, EventArgs e)
        {
            data2 = float.Parse(hienthi2.Text);

            switch (pheptinh)
            {
                case "cong":
                    data1 = data1 + data2;
                    break;
                case "tru":
                    data1 = data1 - data2;
                    break;
                case "nhan":
                    data1 = data1 * data2;
                    break;
                case "chia":
                    if (data2 != 0)
                    {
                        data1 = data1 / data2;
                    }
                    else
                    {
                        hienthi1.Text = "Cannot divide by 0";
                        return;
                    }
                    break;
                default:
                    hienthi1.Text = "Error";
                    return;
            }

            hienthi1.Text = data1.ToString();
            hienthi2.Clear();
        }


        private void button5_Click(object sender, EventArgs e)
        {
            hienthi2.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
