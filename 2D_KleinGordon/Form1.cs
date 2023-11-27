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
            for (int x = 0; x < lx; x++)
            {
                for (int y = 0; y < ly; y++)
                {
                    int i = x + y * lx;
                    fa[i] = Math.Exp(-(((double)x - lx / 2) * ((double)x - lx / 2) + ((double)y - ly / 2) * ((double)y - ly / 2)) / 30)*10;
                    fb[i] = fa[i];
                }
            }
            time();
        }
        private void time()
        {
            for (int _ = 0; _ < n; _++)
            {
                ParallelEnumerable.Range(0, lx * ly).AsParallel().Where(i => !onBorder(i)).ForAll(i => {
                    f1[i] = 2 * fa[i] - fb[i] + (dx2f(i) + dy2f(i) - m * m * fa[i]) * dt * dt;
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
            if (x == 0 || y == 0 || x == lx - 1 || y == ly - 1)
                return true;
            else return false;
        }

        private double dx2f(int i)
        {
            return (fa[i-1] + fa[i+1] - 2 * fa[i]) / (dx * dx);
        }
        private double dy2f(int i)
        {
            return (fa[i-lx] + fa[i+lx] - 2 * fa[i]) / (dx * dx);
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            for (int x = 0; x < lx; x++)
                for (int y = 0; y < ly; y++)
                {
                    int i = x + y * lx;
                    int a = (int)(Math.Abs(fa[i]) * 255);
                    if(a>255)
                        a=255;
                    if (a < 0)
                        a = 0;
                    if (fa[i] > 0)
                        bmp.SetPixel(x, y, Color.FromArgb(a, 0, 0));
                    else
                        bmp.SetPixel(x, y, Color.FromArgb(0, 0, a));
                }
            e.Graphics.DrawImage(bmp,0,0,Canvas.Width,Canvas.Height);
            time();
        }
    }
}
