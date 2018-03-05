using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graphics1
{
    public partial class Kernel : Form
    {
        public Kernel(int height, int width, int[,] kernel = null, int divisor = 1, int offset = 0)
        {
            InitializeComponent();

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

            textBox1.Text = divisor.ToString();
            textBox2.Text = offset.ToString();

            if(kernel!=null)
            {
                for (int i = 0; i < height; i++)
                    for (int j = 0; j < width; j++)
                        Table.GetControlFromPosition(j, i).Text = kernel[i, j].ToString();
            }
        }

    }
}
