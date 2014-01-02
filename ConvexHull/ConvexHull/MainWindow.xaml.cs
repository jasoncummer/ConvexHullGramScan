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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConvexHull
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random rand = new Random();

        

        Point[] sortPoints = new Point[100];// The set of points for computing the convex hull.
        List<Point> points = new List<Point>();
        int pointCount = 0;
        Shape[] userPointShapes = new Shape[100];
        List<Point> upper = new List<Point>();
        List<Point> lower = new List<Point>();

        public MainWindow()
        {
            InitializeComponent();
            this.MouseDown += new MouseButtonEventHandler(Page_MouseMove);
        }

      


        /// <summary>
        /// “CONVEXHULL(P)”
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void findHullButton_Click(object sender, RoutedEventArgs e)
        {/*
            Input. A set P of points in the plane.
            Output. A list containing the vertices of CH(P) in clockwise order.
            1. Sort the points by x-coordinate, resulting in a sequence p1, . . . , pn. for degeneracys have a sort for same Xs with different Ys
            2. Put the points p1 and p2 in a list Lupper, with p1 as the first point.
            3. for i←3 to n
            4. do Append pi to Lupper.
            5. while Lupper contains more than two points and the last three points
            in Lupper do not make a right turn
            6. do Delete the middle of the last three points from Lupper.
            7. Put the points pn 6 and pn−1 in a list Llower, with pn as the first point.
            8. for i←n−2 downto 1
            9. do Append pi to Llower.
            10. while Llower contains more than 2 points and the last three points
            in Llower do not make a right turn
            11. do Delete the middle of the last three points from Llower.
            12. Remove the first and the last point from Llower to avoid duplication of the
            points where the upper and lower hull meet.
            13. Append Llower to Lupper, and call the resulting list L.
            14. return L
              */
            // do to call to the findConvexHull algorithm
            // sort P with increaseing x then y values

            int size = points.Count;
            sortPoints = new Point[size];
            int i = 0; 
            foreach (Point point in points){
                sortPoints[i] = point;
                i ++;
            }

            Quicksort(sortPoints, 0, sortPoints.Length - 1);
            printPointArray(sortPoints);


            upper.Add(sortPoints[0]);
            try
            {
                upper.Add(sortPoints[1]);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("list to short, upper: " + ex );
            }
            for (i = 2; i < sortPoints.Length; i++)
            {
                upper.Add(sortPoints[i]);


                while (upper.Count > 2 && threePointRight(upper[upper.Count - 3], upper[upper.Count - 2], upper[upper.Count - 1]) < -1) 
                {
                    // http://140.129.20.249/~jmchen/compg/slides/chull.pdf
                    // http://en.wikipedia.org/wiki/Graham_scan
                    
                    // remove middle 
                    upper.RemoveAt(upper.Count -2);
                }
                // Swap
                //Point tmp = points[i];
                //points[i] = points[j];
                //points[j] = tmp;
            }

            lower.Add(sortPoints[sortPoints.Length - 1]);
            try
            {
                lower.Add(sortPoints[sortPoints.Length - 2]);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("list to short,lower : " + ex);
            }

            for (i = sortPoints.Length-3; i >= 0; i--)
            {
                lower.Add(sortPoints[i]);


                while (lower.Count > 2 && threePointRight(lower[lower.Count - 3], lower[lower.Count - 2], lower[lower.Count - 1]) < -1)
                {
                    lower.RemoveAt(lower.Count - 2);
                }
            }

            
            try
            {
                lower.RemoveAt(lower.Count - 1);
                lower.RemoveAt(0);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("list lower was small " + ex); 
            }

            foreach (Point p in lower)
            {
                upper.Add(p);
            }

            // upper is now the list that makes up the convex hull in clockwise order

            // draw hull points 
            drawHull(upper);

            // fill hull

            if (sortPoints.Length == 1)
            {
                Status.Text = "A list of one is its own hull.";
            }
            else
            {
                Status.Text = "Convex Hull Computed.";
            }


        }

        void drawHull(List<Point> list)
        {
            for (int i = 0; i < list.Count-1 ; i++)
            {
                createLine(list[i], list[i + 1]);
            }

            createLine(list[0], list[list.Count -1]);
        }

        double threePointRight(Point p1, Point p2, Point p3)// aka ccw
        {
            double result = (p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X);
                System.Diagnostics.Debug.WriteLine("TPR: " + result);
            return result;
        }


        // http://snipd.net/quicksort-in-c
        //public static void Quicksort(IComparable[] elements, int left, int right)
        public static void Quicksort(Point[] elements, int left, int right)
        {
            int i = left, j = right;
            Point pivot = elements[(left + right) / 2];

            while (i <= j)
            {
                while (elements[i].X < pivot.X)
                {
                    i++;
                }

                while (elements[j].X > pivot.X)
                {
                    j--;
                }

                if (i <= j)
                {
                    Point tmp = new Point();
                    //// test for y 
                    //if (elements[i].X == elements[j].X)
                    //{
                    //    if (elements[i].Y > elements[j].Y)
                    //    {
                    //        // Swap
                    //        tmp = elements[i];
                    //        elements[i] = elements[j];
                    //        elements[j] = tmp;
                    //    }// else dont swap them as they are in the correct order
                    //} else if (elements[i].X < elements[j].X)
                    //{
                    //    // Swap
                    //    tmp = elements[i];
                    //    elements[i] = elements[j];
                    //    elements[j] = tmp;
                    //}

                    // Swap
                    tmp = elements[i];
                    elements[i] = elements[j];
                    elements[j] = tmp;

                    i++;
                    j--;
                }
            }

            // Recursive calls
            if (left < j)
            {
                Quicksort(elements, left, j);
            }

            if (i < right)
            {
                Quicksort(elements, i, right);
            }
        }


        void printPointArray(Point[] p)
        {
            for (int i = 0; i < p.Length; i++)//pointCount; i++)
            {
                System.Diagnostics.Debug.WriteLine(p[i]);
            }
        }

        void Page_MouseMove(object sender, MouseButtonEventArgs e)
        {
            //size of the graph canvas
            // x : 200
            // y : 200

            // get the mouse position
            Point mousePosition = e.GetPosition(GraphCanvas);


            // make sure point is in the graph canvas
            if (mousePosition.X < 200 && mousePosition.Y < 200 && mousePosition.X > 0 && mousePosition.Y > 0)
            {
                
                // add a new shape to the stage
                System.Diagnostics.Debug.WriteLine("In the GraphCanvas");
                Status.Text = "Point :  " + mousePosition.X + " : " + mousePosition.Y;
                drawUserPoint(mousePosition);
                

            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Not in the graphics canvas");
                Status.Text = "Not in the graphics canvas";
            }

        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            // just make sure the button is working < TODO > Put this in an if DEF
            System.Diagnostics.Debug.WriteLine("Clear button");

            // clear points from canvas 
            //remove shape from stage
            GraphCanvas.Children.Clear();
            // clear points form points[]
            points = new List<Point>();
            pointCount = 0 ;
            sortPoints = null;
            
            // clear lines from the hull
            upper = new List<Point>();
            lower = new List<Point>();

            // clear the fill polygon


            // Clear the text
            Status.Text = "";

        }


        //    // this is just a test line
        //    createLine(new Point(0, 200), new Point(200, 0));
        private void createLine(Point p1, Point p2)
        {
            Line myLine = new Line();
            myLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            myLine.X1 = p1.X;
            myLine.X2 = p2.X;
            myLine.Y1 = p1.Y;
            myLine.Y2 = p2.Y;
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.StrokeThickness = 2;
            GraphCanvas.Children.Add(myLine);
        }

        private void drawPoint(Point p)
        {
            //create shape
            System.Diagnostics.Debug.WriteLine("Creating shape");

            Shape shape = new Ellipse();
            shape.SetValue(Canvas.LeftProperty, p.X);
            shape.SetValue(Canvas.TopProperty, p.Y);
            shape.HorizontalAlignment = HorizontalAlignment.Left;
            shape.VerticalAlignment = VerticalAlignment.Center;
            shape.Width = 4;
            shape.Height = 4;
            shape.Stroke = new SolidColorBrush(Colors.Black);
            shape.StrokeThickness = 3.0;

            GradientBrush gb = new LinearGradientBrush();
            gb.GradientStops = new GradientStopCollection();
            GradientStop g1 = new GradientStop();
            g1.Color = Colors.Red;
            gb.GradientStops.Add(g1);
            g1 = new GradientStop();
            g1.Color = Colors.Blue;
            g1.Offset = 2;
            gb.GradientStops.Add(g1);

            shape.Fill = gb;

            shape.Visibility = Visibility.Visible;
            shape.Opacity = 0.5;

            GraphCanvas.Children.Add(shape);

        }

        private void drawUserPoint(Point p)
        {
            //create shape
            System.Diagnostics.Debug.WriteLine("Creating shape");

            Shape shape = new Ellipse();
            shape.SetValue(Canvas.LeftProperty, p.X); //+ 7); dont need this any more because before i was forgetting to add this to the graphcanvas, was layoutroot before
            shape.SetValue(Canvas.TopProperty, p.Y); //+ 18);
            shape.HorizontalAlignment = HorizontalAlignment.Left;
            shape.VerticalAlignment = VerticalAlignment.Center;
            shape.Width = 4;
            shape.Height = 4;
            shape.Stroke = new SolidColorBrush(Colors.Black);
            shape.StrokeThickness = 3.0;

            GradientBrush gb = new LinearGradientBrush();
            gb.GradientStops = new GradientStopCollection();
            GradientStop g1 = new GradientStop();
            g1.Color = Colors.Red;
            gb.GradientStops.Add(g1);
            g1 = new GradientStop();
            g1.Color = Colors.Blue;
            //g1.Offset = 2;
            gb.GradientStops.Add(g1);

            shape.Fill = gb;

            shape.Visibility = Visibility.Visible;
            shape.Opacity = 0.5;
            //shape.Name = "theUsersPoint";

            //LayoutRoot.Children.Add(shape);
            GraphCanvas.Children.Add(shape);


            // <todo> have dynamic array size increase
            userPointShapes[pointCount] = shape;
            
            points.Add(new Point(p.X, p.Y));
            pointCount++;

        }

    }
}
