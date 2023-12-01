using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace _2D_KleinGordon
{
    public partial class Form1 : Form
    {
        const int n = 25;
        const double dt = 0.04;
        const double dx = 1;
        const int lx = 200;
        const int ly = 200;

        const double m = 0.1;

        double[] fa = new double[lx*ly];
        double[] fb = new double[lx*ly];
        double[] f1 = new double[lx*ly];

        Bitmap bmp = new Bitmap(lx, ly);
        public Form1()
        {
            InitializeComponent();
            SetGaussian(((double)lx)/2, ((double)ly)/2, 30);
            time();
        }
        private void SetGaussian(double posX, double posY, double s)
        {
            for (int x = 0; x < lx; x++)
                for (int y = 0; y < ly; y++)
                {
                    int i = x + y * lx;
                    if (!onBorder(i))
                    {
                        fa[i] = Math.Exp(-((x - posX) * (x - posX) + (y - posY) * (y - posY)) / s) * 10;
                        fb[i] = fa[i];
                    }
                }
        }
        private void time()
        {
            for (int _ = 0; _ < n; _++)
            {
                ParallelEnumerable.Range(0, lx * ly).AsParallel().Where(i => !onBorder(i)).ForAll(i => {
                    f1[i] = 2 * fa[i] - fb[i] + ((fa[i - 1] + fa[i + 1] + fa[i - lx] + fa[i + lx] - 4 * fa[i]) / (dx * dx) - m * m * fa[i]) * dt * dt;
                });
                for (int i = 0; i < lx*ly; i++)
                    if (!onBorder(i))
                    {
                        fb[i] = fa[i];
                        fa[i] = f1[i];
                    }
            } 
            Canvas.Invalidate();
        }

        bool onBorder(int i)
        {
            int x = i % lx;
            int y = i / lx;
            return x == 0 || y == 0 || x == lx - 1 || y == ly - 1;
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            for (int x = 0; x < lx; x++)
                for (int y = 0; y < ly; y++)
                {
                    int i = x + y * lx;
                    int a = (int)(Math.Abs(fa[i]) * 255);
                    a = a > 255 ? 255 : (a < 0 ? 0 : a);
                    bmp.SetPixel(x, y, fa[i] > 0 ? Color.FromArgb(a, 0, 0) : Color.FromArgb(0, 0, a));
                }
            e.Graphics.DrawImage(bmp,0,0,Canvas.Width,Canvas.Height);
            time();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Space)
                SetGaussian(((double)lx) / 2, ((double)ly) / 2, 30);
        }
    }
}
