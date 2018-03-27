using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
        int[,] D2 = new int[2, 2] { { 1, 3 }, { 4, 2 } };
        int[,] D3 = new int[3, 3] { { 3, 7, 4 }, { 6, 1, 9 }, { 2, 8, 5 } };
        int[,] D4 = new int[4, 4] { { 1, 9, 3, 11 }, { 13, 5, 15, 7 }, { 4, 12, 2, 10 }, { 16, 8, 14, 5 } };
        int[,] D6 = new int[6, 6] { { 9, 25, 13, 11, 27, 15 }, { 21, 1, 33, 23, 3, 35 }, { 5, 29, 17, 7, 31, 19 }, { 12, 28, 16, 10, 26, 14 }, { 24, 4, 36, 22, 2, 34 }, { 8, 32, 20, 6, 30, 18 } };

        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.Add("2");
            comboBox1.Items.Add("4");
            comboBox1.Items.Add("8");
            comboBox1.Items.Add("16");
            comboBox1.SelectedIndex = 0;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
            }
            if (pictureBox2.Image != null)
            {
                pictureBox2.Image.Dispose();
            }
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            if (op.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Grayscale(Image.FromFile(op.FileName));
                //pictureBox1.Image = Image.FromFile(op.FileName);
            }
        }

        //Checks if value is in color range
        private int Check(double n)
        {
            if (n < 0) return 0;
            else if (n > 255) return 255;
            else return (int)n;
        }

        private Bitmap Grayscale(Image img)
        {
            Color c;
            Bitmap b = (Bitmap)img;
            for (int j = 0; j < b.Height; j++)
                for (int i = 0; i < b.Width; i++)
                {
                    c = b.GetPixel(i, j);
                    int avg = (int)(c.R * 0.3 + c.G * 0.6 + c.B * 0.1);
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

        private Bitmap ApplyMatrix(int[,] kernel, int divisor = 1, int offset = 0)
        {
            Bitmap b, result;
            try
            {
                b = new Bitmap(pictureBox1.Image);
                result = new Bitmap(pictureBox1.Image);

                int x, y;
                int halfwidth = kernel.GetLength(1) / 2;
                int halfheigth = kernel.GetLength(0) / 2;
                int wend = halfwidth;
                if (kernel.GetLength(1) % 2 == 1) wend++;
                int hend = halfheigth;
                if (kernel.GetLength(0) % 2 == 1) hend++;
                int[] sum = new int[3];
                Color c;

                for (int i = 0; i < b.Width; i++)
                    for (int j = 0; j < b.Height; j++)
                    {
                        for (int k = 0; k < 3; k++)
                            sum[k] = 0;
                        for (int m = -halfwidth; m < wend; m++)
                            for (int n = -halfheigth; n < hend; n++)
                            {
                                if ((i + m) < 0) x = 0;
                                else if ((i + m) >= b.Width) x = b.Width - 1;
                                else x = i + m;

                                if ((j + n) < 0) y = 0;
                                else if ((j + n) >= b.Height) y = b.Height - 1;
                                else y = j + n;

                                c = b.GetPixel(x, y);
                                sum[0] += c.R * kernel[n + halfheigth, m + halfwidth];
                                sum[1] += c.G * kernel[n + halfheigth, m + halfwidth];
                                sum[2] += c.B * kernel[n + halfheigth, m + halfwidth];
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
            catch (NullReferenceException)
            {
                MessageBox.Show("No initial file was chosen");
                return null;
            }
        }

        private void meanFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = ApplyMatrix(Mean, 9);
        }

        private void gaussianBlurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = ApplyMatrix(Gauss, 16);
        }

        private void sharpeningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = ApplyMatrix(Sharp);
        }

        private void edgeDetectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = ApplyMatrix(Edge);
        }

        private void embossToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = ApplyMatrix(Emboss);
        }

        public void applyCustom(int[,] kernel, int divisor, int offset)
        {
            pictureBox2.Image = ApplyMatrix(kernel, divisor, offset);
        }


        //Displaying filters' kernels
        private void meanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ker = new Kernel(this, 3, 3, Mean, 9);
            ker.Show();
        }

        private void gaussianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ker = new Kernel(this, 3, 3, Gauss, 16);
            ker.Show();
        }

        private void sharpeningToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var ker = new Kernel(this, 3, 3, Sharp);
            ker.Show();
        }

        private void edgeDetectionToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            var ker = new Kernel(this, 3, 3, Edge);
            ker.Show();
        }

        private void embossToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var ker = new Kernel(this, 3, 3, Emboss);
            ker.Show();
        }

        private void Generate_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "") return;
            int h, w;
            try
            {
                h = Int32.Parse(textBox1.Text);
                w = Int32.Parse(textBox2.Text);
            }
            catch (System.FormatException)
            {
                MessageBox.Show("Please enter integers");
                return;
            }
            var ker = new Kernel(this, h, w);
            ker.Show();
        }

        //Dithering
        private void randomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            try
            {
                List<int> thr = new List<int>();
                Color c;
                Bitmap b = new Bitmap(pictureBox1.Image);
                for (int j = 0; j < b.Height; j++)
                    for (int i = 0; i < b.Width; i++)
                    {
                        c = b.GetPixel(i, j);
                        for (int k = 0; k < Int32.Parse(comboBox1.SelectedItem.ToString()); k++)
                        {
                            int threshold = rnd.Next(256);
                            thr.Add(threshold);
                        }
                        thr.Sort();
                        int colors = Int32.Parse(comboBox1.SelectedItem.ToString());
                        bool set = false;
                        foreach (var th in thr)
                        {
                            if (c.R < th)
                            {
                                b.SetPixel(i, j, Color.FromArgb(c.A, Check((double)255 * thr.IndexOf(th) / (colors-1)), Check((double)255 * thr.IndexOf(th) / (colors-1)), Check((double)255 * thr.IndexOf(th) / (colors-1))));
                                set = true;
                                break;
                            }
                        }
                        if (!set) b.SetPixel(i, j, Color.FromArgb(c.A, 255, 255, 255));
                        thr.Clear();
                    }
                pictureBox2.Image = b;
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("No initial file was chosen");
            }
        }

        private void averageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Color c;
                Bitmap b = new Bitmap(pictureBox1.Image);
                int k = Int32.Parse(comboBox1.SelectedItem.ToString());
                List<int> list = new List<int>();
                list.Add(0);
                list.Add(255);
                List<int> list1 = new List<int>();
                while(list.Count<(k+1))
                {
                    for (int j = 0; j < list.Count - 1; j++)
                        list1.Add(findAverage(new Range(list[j], list[j + 1])));
                    foreach (var el in list1)
                        list.Add(el);
                    list1.Clear();
                    list.Sort();
                }
                list.RemoveAt(0);
                list.RemoveAt(list.Count - 1);

                for (int j = 0; j < b.Height; j++)
                    for (int i = 0; i < b.Width; i++)
                    {
                        c = b.GetPixel(i, j);
                        bool set = false;
                        foreach (var th in list)
                        {
                            if (c.R < th)
                            {
                                b.SetPixel(i, j, Color.FromArgb(c.A, Check((double)255 * list.IndexOf(th) / (k-1)), Check((double)255 * list.IndexOf(th) / (k-1)), Check((double)255 * list.IndexOf(th) / (k-1))));
                                set = true;
                                break;
                            }
                        }
                        if (!set) b.SetPixel(i, j, Color.FromArgb(c.A, 255, 255, 255));
                    }
                pictureBox2.Image = b;
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("No initial file was chosen");
            }
        }

        private int findAverage(Range r)
        {
            Color c;
            int threshold = 0, count = 0;
            Bitmap b = new Bitmap(pictureBox1.Image);
            for (int j = 0; j < b.Height; j++)
                for (int i = 0; i < b.Width; i++)
                {
                    c = b.GetPixel(i, j);
                    if ((c.R >= r.start) && (c.R <= r.end))
                    {
                        threshold += c.R;
                        count++;
                    }
                }
            return threshold / count;
        }

        private void orderedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Color c;
                Bitmap bit = new Bitmap(pictureBox1.Image);
                for (int j = 0; j < bit.Height; j++)
                    for (int i = 0; i < bit.Width; i++)
                    {
                        c = bit.GetPixel(i, j);
                        double threshold = 0;
                        if (toolStripMenuItem2.Checked) threshold = (double)D2[i % 2, j % 2] / 5;
                        else if (toolStripMenuItem3.Checked) threshold = (double)D3[i % 3, j % 3] / 10;
                        else if (toolStripMenuItem4.Checked) threshold = (double)D4[i % 4, j % 4] / 17;
                        else if (toolStripMenuItem5.Checked) threshold = (double)D6[i % 6, j % 6] / 37;

                        if (c.R < threshold * 255)
                            bit.SetPixel(i, j, Color.FromArgb(c.A, 0, 0, 0));
                        else
                            bit.SetPixel(i, j, Color.FromArgb(c.A, 255, 255, 255));
                    }
                pictureBox2.Image = bit;
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("No initial file was chosen");
            }
        }

        private void Click2(object sender, EventArgs e)
        {
            toolStripMenuItem2.Checked = true;
            if (toolStripMenuItem3.Checked) toolStripMenuItem3.Checked = false;
            if (toolStripMenuItem4.Checked) toolStripMenuItem4.Checked = false;
            if (toolStripMenuItem5.Checked) toolStripMenuItem5.Checked = false;
        }

        private void Click3(object sender, EventArgs e)
        {
            toolStripMenuItem3.Checked = true;
            if (toolStripMenuItem2.Checked) toolStripMenuItem2.Checked = false;
            if (toolStripMenuItem4.Checked) toolStripMenuItem4.Checked = false;
            if (toolStripMenuItem5.Checked) toolStripMenuItem5.Checked = false;
        }

        private void Click4(object sender, EventArgs e)
        {
            toolStripMenuItem4.Checked = true;
            if (toolStripMenuItem3.Checked) toolStripMenuItem3.Checked = false;
            if (toolStripMenuItem2.Checked) toolStripMenuItem2.Checked = false;
            if (toolStripMenuItem5.Checked) toolStripMenuItem5.Checked = false;
        }

        private void Click5(object sender, EventArgs e)
        {
            toolStripMenuItem5.Checked = true;
            if (toolStripMenuItem3.Checked) toolStripMenuItem3.Checked = false;
            if (toolStripMenuItem4.Checked) toolStripMenuItem4.Checked = false;
            if (toolStripMenuItem2.Checked) toolStripMenuItem2.Checked = false;
        }

        private void medianCutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int k = Int32.Parse(toolStripTextBox1.Text);
            if(!IsPowerOfTwo((ulong)k)) 
            {
                MessageBox.Show("k can be only power of 2");
                return;
            }
            List<Range> list = new List<Range>();
            Color c;

            int min = 255, max = 0;
            Bitmap b1 = new Bitmap(pictureBox1.Image);
            for (int j = 0; j < b1.Height; j++)
                for (int i = 0; i < b1.Width; i++)
                {
                    c = b1.GetPixel(i, j);
                    if (c.R < min) min = c.R;
                    if (c.R > max) max = c.R;
                }
            list.Add(new Range(min, max));

            for (int i=0;i<(k-1);i++)
            {
                int len = list.Count;
                for(int j=0;j<len;j++)
                {
                    list.Add(new Range(list[0].start, list[0].center));
                    list.Add(new Range(list[0].center, list[0].end));
                    list.RemoveAt(0);
                }
            }

            Bitmap b = new Bitmap(pictureBox1.Image);
            for (int j = 0; j < b.Height; j++)
                for (int i = 0; i < b.Width; i++)
                {
                    c = b.GetPixel(i, j);
                    int inten = 0;
                    foreach(var r in list)
                    {
                        if ((c.R >= r.start) && (c.R <= r.end))
                        {
                            inten = r.center;
                            break;
                        }
                    }
                    try
                    {
                        b.SetPixel(i, j, Color.FromArgb(c.A, inten, inten, inten));
                    }
                    catch (InvalidOperationException)
                    {
                        MessageBox.Show("Images with indexes pixels are unfortunately unsupported");
                        break;
                    }
                }
            pictureBox2.Image = b;
        }

        bool IsPowerOfTwo(ulong x)
        {
            return (x != 0) && ((x & (x - 1)) == 0);
        }
    }
}
