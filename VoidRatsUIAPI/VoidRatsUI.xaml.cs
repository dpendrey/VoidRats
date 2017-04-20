using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VoidRatsUIAPI
{
    /// <summary>
    /// Interaction logic for VoidRatsUI.xaml
    /// </summary>
    public partial class VoidRatsUI : Window
    {
        internal static VoidRatsUI currentWindow = null;
        Control currentContent = null;
        Grid currentMenu = null;

        public VoidRatsUI()
        {
            InitializeComponent();

            currentWindow = this;

            setMenu(Extension.GetMenu("MAINMENU"));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ///Splasher.CloseSplash();

            /*
            if (false && timer == null)
            {
                rand = new Random();

                backgroundPen = new Pen(Brushes.White, 1);
                backgroundBrush = Brushes.White;

                backgroundStatic = new List<Point>();
                for (int i = 0; i < 25; i++)
                    backgroundStatic.Add(new Point(rand.NextDouble(), rand.NextDouble()));

                backgroundMoving = new List<Point>();
                while (backgroundMoving.Count < 25)
                    backgroundMoving.Add(new Point(rand.NextDouble() * ActualWidth / 2, rand.NextDouble() * 2 * Math.PI));

                timer = new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.Background);
                timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
                timer.Tick += new EventHandler(timer_Tick);
                timer.Start();
            }
            */
        }

        internal void setMenu(Menu Menu)
        {
            if (currentMenu != null)
            {
                menuGrid.Children.Remove(currentMenu);

                if (currentMenu is IDisposable)
                    ((IDisposable)currentMenu).Dispose();
                currentMenu = null;
            }

            currentMenu = Menu.BuildMenu();
            menuGrid.Children.Add(currentMenu);

            if (currentMenu.ActualWidth != 200)
            {
            }
        }

        internal void setContent(Control Control)
        {
            if (currentContent != null)
            {
                contentGrid.Children.Remove(currentContent);

                if (currentContent is IDisposable)
                    ((IDisposable)currentContent).Dispose();
                currentContent = null;
            }

            contentGrid.Children.Add(Control);
            currentContent = Control;
        }

        internal void showInfo(string Topic)
        {
            if (currentContent != null && currentContent is VoidRatsUIAPI.VoidRatsInfoCell)
            {
                ((VoidRatsUIAPI.VoidRatsInfoCell)currentContent).ShowInfo(Topic);
            }
            else
            {
                VoidRatsUIAPI.VoidRatsInfoCell newContent = new VoidRatsUIAPI.VoidRatsInfoCell();
                setContent(newContent);
                newContent.ShowInfo(Topic);
            }
        }

        /*
        System.Windows.Threading.DispatcherTimer timer;
        Random rand;
        Pen backgroundPen;
        Brush backgroundBrush;
        List<Point> backgroundStatic;
        List<Point> backgroundMoving;
        void timer_Tick(object sender, EventArgs e)
        {
            GeometryGroup ellipses = new GeometryGroup();
            double width = ActualWidth, height = ActualHeight;

            ellipses.Children.Add(
                    new EllipseGeometry(new Point(0, 0), 0, 0)
                    );
            ellipses.Children.Add(
                    new EllipseGeometry(new Point(width, height), 0, 0)
                    );

            for (int i = 0; i < backgroundStatic.Count; i++)
                ellipses.Children.Add(
                    new EllipseGeometry(new Point(backgroundStatic[i].X * width, backgroundStatic[i].Y * height), 2, 2)
                    );

            while (backgroundMoving.Count < 25)
                backgroundMoving.Add(new Point(0, rand.NextDouble() * 2 * Math.PI));

            for (int i = 0; i < backgroundMoving.Count; i++)
            {
                backgroundMoving[i] = new Point(backgroundMoving[i].X + 2, backgroundMoving[i].Y);
                double x = backgroundMoving[i].X * Math.Cos(backgroundMoving[i].Y) + width / 2,
                    y = backgroundMoving[i].X * Math.Sin(backgroundMoving[i].Y) + height / 2;
                ellipses.Children.Add(
                    new EllipseGeometry(new Point(x, y), 2, 2)
                    );

                if (x < 0 || x > width || y < 0 || y > height)
                {
                    backgroundMoving.RemoveAt(i);
                    i--;
                }
            }

            Drawing tmpB = new GeometryDrawing(backgroundBrush, backgroundPen, ellipses);

            DrawingImage tmpZ = new DrawingImage(tmpB);

            backgroundImage.Source = tmpZ;
        }
        */
    }
}
