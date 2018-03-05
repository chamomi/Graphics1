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
    public partial class Form1 : Form
    {
        int BrChange = 50;
        int ContChange = 50;
        int[,] Mean = new int[3, 3] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
        int[,] Gauss = new int[3, 3] { { 1, 2, 1 }, { 2, 4, 2 }, { 1, 2, 1 } };
        int[,] Sharp = new int[3, 3] { { -1, -1, -1 }, { -1, 9, -1 }, { -1, -1, -1 } };
        int[,] Edge = new int[3, 3] { { -1, -1, -1 }, { -1, 8, -1 }, { -1, -1, -1 } };
        int[,] Emboss = new int[3, 3] { { -1, -1, 0 }, { -1, 1, 1 }, { 0, 1, 1 } };

        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
            }
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            if (op.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Grayscale(Image.FromFile(op.FileName));
            }
        }

        //Checks if value is in color range
        private int Check(double n)
        {
            if (n < 0) return 0;
            else if (n > 255) return 255;
            else return (int)n;
        }

        //Grayscale
        private Bitmap Grayscale(Image img)
        {
            Color c;
            Bitmap b = (Bitmap)img;
            for (int j = 0; j < b.Height; j++)
                for (int i = 0; i < b.Width; i++)
                {
                    c = b.GetPixel(i, j);
                    int avg = (c.R + c.G + c.B) / 3;
                    try
                    {
                        b.SetPixel(i, j, Color.FromArgb(c.A, avg, avg, avg));
                    }
                    catch (InvalidOperationException)
                    {
                        MessageBox.Show("Images with indexes pixels are unfortunately unsupported");
                        break;
                    }
                }
            return b;
        }

        private void inversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Color c;
                Bitmap b = new Bitmap(pictureBox1.Image);
                for (int j = 0; j < b.Height; j++)
                    for (int i = 0; i < b.Width; i++)
                    {
                        c = b.GetPixel(i, j);
                        b.SetPixel(i, j, Color.FromArgb(c.A, 255 - c.R, 255 - c.G, 255 - c.B));
                    }
                pictureBox2.Image = b;
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("No initial file was chosen");
            }
        }

        private void brightnessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Color c;
                Bitmap b = new Bitmap(pictureBox1.Image);
                for (int j = 0; j < b.Height; j++)
                    for (int i = 0; i < b.Width; i++)
                    {
                        c = b.GetPixel(i, j);
                        b.SetPixel(i, j, Color.FromArgb(c.A, Check(c.R + BrChange), Check(c.G + BrChange), Check(c.B + BrChange)));
                    }
                pictureBox2.Image = b;
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("No initial file was chosen");
            }
        }

        private void contrastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                float contr = ((float)(ContChange + 255)) / 255.0f;
                contr *= contr;

                Color c;
                Bitmap b = new Bitmap(pictureBox1.Image);
                for (int j = 0; j < b.Height; j++)
                    for (int i = 0; i < b.Width; i++)
                    {
                        c = b.GetPixel(i, j);
                        b.SetPixel(i, j, Color.FromArgb(c.A, Check((((float)c.R / 255.0 - 0.5) * contr + 0.5) * 255.0),
                            Check((((float)c.G / 255.0 - 0.5) * contr + 0.5) * 255.0),
                            Check((((float)c.B / 255.0 - 0.5) * contr + 0.5) * 255.0)));
                    }
                pictureBox2.Image = b;
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("No initial file was chosen");
            }
        }

        //Applies kernel matrix to image b, returns processed image
        private Bitmap ApplyMatrix(Bitmap b, int[,] kernel, int divisor = 1, int offset = 0)
        {
            Bitmap result = new Bitmap(pictureBox1.Image);

            int x, y;
            int[] sum = new int[3];
            Color c;
            for (int i = 0; i < b.Width; i++)
                for (int j = 0; j < b.Height; j++)
                {
                    for (int k = 0; k < 3; k++)
                        sum[k] = 0;
                    for (int m = 0; m < kernel.GetLength(1); m++)
                        for (int n = 0; n < kernel.GetLength(0); n++)
                        {
                            if ((i + m) < 0) x = 0;
                            else if ((i + m) >= b.Width) x = b.Width - 1;
                            else x = i + m;

                            if ((j + n) < 0) y = 0;
                            else if ((j + n) >= b.Height) y = b.Height - 1;
                            else y = j + n;

                            c = b.GetPixel(x, y);
                            sum[0] += c.R * kernel[n, m];
                            sum[1] += c.G * kernel[n, m];
                            sum[2] += c.B * kernel[n, m];
                        }
                    try
                    {
                        result.SetPixel(i, j, Color.FromArgb(result.GetPixel(i, j).A, Check(sum[0] / divisor + offset), Check(sum[1] / divisor + offset), Check(sum[2] / divisor + offset)));
                    }
                    catch (InvalidOperationException)
                    {
                        MessageBox.Show("Images with indexes pixels are unfortunately unsupported");
                        break;
                    }
                }
            return result;
        }

        private void meanFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = ApplyMatrix((Bitmap)pictureBox1.Image, Mean, 9);
        }

        private void gaussianBlurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = ApplyMatrix((Bitmap)pictureBox1.Image, Gauss, 16);
        }

        private void sharpeningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = ApplyMatrix((Bitmap)pictureBox1.Image, Sharp);
        }

        private void edgeDetectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = ApplyMatrix((Bitmap)pictureBox1.Image, Edge);
        }

        private void embossToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = ApplyMatrix((Bitmap)pictureBox1.Image, Emboss);
        }
    }
}
