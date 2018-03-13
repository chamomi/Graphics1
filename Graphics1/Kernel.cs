using System;
using System.Windows.Forms;

namespace Graphics1
{
    public partial class Kernel : Form
    {
        Form1 parent = null;
        public Kernel(Form1 par, int height, int width, int[,] kernel = null, int divisor = 1, int offset = 0)
        {
            InitializeComponent();
            parent = par;

            Table.Controls.Clear();
            Table.ColumnStyles.Clear();
            Table.RowStyles.Clear();

            Table.Height = height * 20;
            Table.Width = width * 50;
            Table.ColumnCount = width;
            Table.RowCount = height;
            for (int i = 0; i < width; i++)
                Table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
            for (int i = 0; i < height; i++)
                Table.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    var tbox = new TextBox();
                    tbox.Dock = DockStyle.Fill;
                    Table.Controls.Add(tbox, j, i);
                }

            divisorBox.Text = divisor.ToString();
            offsetBox.Text = offset.ToString();

            if(kernel!=null)
            {
                for (int i = 0; i < height; i++)
                    for (int j = 0; j < width; j++)
                        Table.GetControlFromPosition(j, i).Text = kernel[i, j].ToString();
            }
        }

        private void applyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int divisor = Int32.Parse(divisorBox.Text);
                int offset = Int32.Parse(offsetBox.Text);

                int[,] kernel = new int[Table.RowCount, Table.ColumnCount];
                for (int i = 0; i < Table.RowCount; i++)
                    for (int j = 0; j < Table.ColumnCount; j++)
                        kernel[i, j] = Int32.Parse(Table.GetControlFromPosition(j, i).Text);

                parent.applyCustom(kernel, divisor, offset);
                //this.Close();
            }
            catch(System.FormatException)
            {
                MessageBox.Show("Please check that all kernel elements, divisor and offset are integers");
            }
        }
    }
}
