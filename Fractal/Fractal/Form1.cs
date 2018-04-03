using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace Fractal
{
    public partial class Form1 : Form
    {
        private const int MAX = 256;      // max iterations
        private const double SX = -2.025; // start value real
        private const double SY = -1.125; // start value imaginary
        private const double EX = 0.6;    // end value real
        private const double EY = 1.125;  // end value imaginary
        private static int x1, y1, xs, ys, xe, ye;
        private static double xstart, ystart, xende, yende, xzoom, yzoom;
        private static bool action, rectangle, finished;
        private static float xy;
        private bool mouseDown = false;
        //private Image picture;
        private Bitmap picture;
        private Graphics g;
        private Graphics g1;
        private Cursor c1, c2;
        private HSB HSBcol;
        private Pen pen;
        private int j;

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // MessageBox.Show("This is Madelbrot Version 1.0.0.1");
            Form2 version = new Form2();
            version.Show();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog file = new SaveFileDialog();
            file.Filter = "JPG(*.JPG | *.JPG";
            if(file.ShowDialog() == DialogResult.OK)
            {
                picture.Save(file.FileName);
            }
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Close();
            picBox1.Image = null;
            start();
            //Application.Restart();
        }

        private void blueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // j = 150;
            j = 165;
            mandelbrot();
            Refresh();
        }

        private void greenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            j = 60;
            mandelbrot();
            Refresh();
        }

        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {
            j = 255;
            mandelbrot();
            Refresh();
        }

        private void purpleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            j = 220;
            mandelbrot();
            Refresh();
        }

        private void orangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            j = 15;
            mandelbrot();
            Refresh();
        }

        private void brownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Color c1 = Color.FromArgb(165, 42, 42);
         //   j = Convert.ToByte(c1);
            mandelbrot();
            Refresh();
        }

        private void picBox1_Load(object sender, EventArgs e)
        {
            //HSB hhh = new HSB();
        }

        private void Form1_Load(object sender, EventArgs e)
        { 
            HSB hsb = new HSB();
        }

        private void picBox1_Click(object sender, EventArgs e)
        {
            update();
        }

        private void picBox1_MouseDown(object sender, MouseEventArgs e)
        {
            //e.consume();
            if (action)
            {
                mouseDown = true;
                xs = e.X;
                ys = e.Y;
            }
        }

        private void picBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //e.consume();

            if (action && mouseDown)
            {
                xe = e.X;
                ye = e.Y;
                rectangle = true;
                picBox1.Refresh();
                //picBox1.Invalidate();
            }
        }

        private void picBox1_MouseUp(object sender, MouseEventArgs e)
        {
            int z, w;

            //e.consume();
            if (action)
            {
                xe = e.X;
                ye = e.Y;
                if (xs > xe)
                {
                    z = xs;
                    xs = xe;
                    xe = z;
                }
                if (ys > ye)
                {
                    z = ys;
                    ys = ye;
                    ye = z;
                }
                w = (xe - xs);
                z = (ye - ys);
                if ((w < 2) && (z < 2)) initvalues();
                else
                {
                    if (((float)w > (float)z * xy)) ye = (int)((float)ys + (float)w / xy);
                    else xe = (int)((float)xs + (float)z * xy);
                    xende = xstart + xzoom * (double)xe;
                    yende = ystart + yzoom * (double)ye;
                    xstart += xzoom * (double)xs;
                    ystart += yzoom * (double)ys;
                }
                xzoom = (xende - xstart) / (double)x1;
                yzoom = (yende - ystart) / (double)y1;
                mandelbrot();
                rectangle = false;
                picBox1.Refresh();
                //picBox1.Invalidate();
                mouseDown = false;
            }
        }

        public Form1()
        {
            InitializeComponent();
            HSBcol = new HSB();
            //setSize(640, 480);
            this.picBox1.Size = new System.Drawing.Size(640, 480); // equivalent of setSize in java code
            finished = false;
            c1 = Cursors.Default;
            c2 = Cursors.Cross;
            x1 = picBox1.Width;
            y1 = picBox1.Height;
            xy = (float)x1 / (float)y1;
            picture = new Bitmap(picBox1.Width, picBox1.Height);
            g1 = Graphics.FromImage(picture);
            finished = true;

            start();

        }

        public void start()
        {
            action = false;
            rectangle = false;
            initvalues();
            xzoom = (xende - xstart) / (double)x1;
            yzoom = (yende - ystart) / (double)y1;
            mandelbrot();
        }

        
        private void initvalues() // reset start values
        {
            xstart = SX;
            ystart = SY;
            xende = EX;
            yende = EY;
            if ((float)((xende - xstart) / (yende - ystart)) != xy)
                xstart = xende - (yende - ystart) * (double)xy;
        }

        private void mandelbrot() // calculate all points
        {
            int x, y;
            float h, b, alt = 0.0f;
            Pen pen = new Pen(Color.White);

            action = false;
            this.Cursor = c1; // in java setCursor(c1)
            picBox1.Cursor = c2;

            //showStatus("Mandelbrot-Set will be produced - please wait..."); will do later
            for (x = 0; x < x1; x += 2)
            {
                for (y = 0; y < y1; y++)
                {
                    h = pointcolour(xstart + xzoom * (double)x, ystart + yzoom * (double)y); // hue value

                    if (h != alt)
                    {
                        b = 1.0f - h * h; // brightness

                        HSBcol.fromHSB(h, 0.8f, b); //convert hsb to rgb then make a Java Color
                        Color col = Color.FromArgb(Convert.ToByte(HSBcol.rChan), Convert.ToByte(HSBcol.gChan), Convert.ToByte(HSBcol.bChan));

                        pen = new Pen(col);

                        //djm end
                        //djm added to convert to RGB from HSB

                        alt = h;
                    }
                    g1.DrawLine(pen, new Point(x, y), new Point(x + 1, y)); // drawing pixel
                }
                //showStatus("Mandelbrot-Set ready - please select zoom area with pressed mouse.");
                Cursor.Current = c1;
            }

            picBox1.Image = picture;
            action = true;
        }

        private float pointcolour(double xwert, double ywert) // color value from 0.0 to 1.0 by iterations
        {
            double r = 0.0, i = 0.0, m = 0.0;// real, imaginary, absolute value or distance
            int j2;
            j2 = j;
            while ((j2 < MAX) && (m < 4.0))
            {
                j2++;
                m = r * r - i * i; // x^2 - y^2
                i = 2.0 * r * i + ywert; // 2xy + c
                r = m + xwert;
            }
            return (float)j2 / (float)MAX;
        }

        private void picBox1_Paint(object sender, PaintEventArgs e)
        {
            update();
            
        }

        public void update()
        {
            Image tempPic = Image.FromHbitmap(picture.GetHbitmap());
            Graphics g = Graphics.FromImage(tempPic);

            if (rectangle)
            {
                Pen pen1 = new Pen(Color.Black);

                Rectangle rect;

                if (xs < xe)
                {
                    if (ys < ye)
                    {
                        rect = new Rectangle(xs, ys, (xe - xs), (ye - ys));
                    }
                    else
                    {
                        rect = new Rectangle(xs, ye, (xe - xs), (ys - ye));
                    }
                }
                else
                {
                    if (ys < ye)
                    {
                        rect = new Rectangle(xe, ys, (xs - xe), (ye - ys));
                    }
                    else
                    {
                        rect = new Rectangle(xe, ye, (xs - xe), (ys - ye));
                    }
                }

                g.DrawRectangle(pen1, rect);
                picBox1.Image = tempPic;

            }
        }
    }
    
}