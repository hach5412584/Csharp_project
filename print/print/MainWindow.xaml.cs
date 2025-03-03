using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace print
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        Point start, dest;
        Color currentStrokeColor = Colors.Red;
        Color currentFillColor = Colors.Yellow;
        Brush currentStrokeBrush = new SolidColorBrush (Colors.Red);
        Brush currentFillBrush = new SolidColorBrush(Colors.Yellow);
        string currentShape = "Line";
        string currentAction = "Draw"; 
        int currentStrokeThickness = 5;
        PointCollection myPointCollection = new PointCollection();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MyCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)  //滑鼠移動時
        {
            switch (currentAction)
            {
                case "Draw":
                    if (e.LeftButton == MouseButtonState.Pressed)  //當滑鼠左鍵被按下
                    {
                        MyCanvas.Cursor = System.Windows.Input.Cursors.Pen;  //鼠標樣式
                        dest = e.GetPosition(MyCanvas);
                        MyLabel.Content = $"座標點:({start}) - ({dest})";
                    }
                    break ;
                case "Erase":
                    var selectedShape = e.OriginalSource as Shape;
                    MyCanvas.Children.Remove(selectedShape); //刪除碰到的圖形
                    if (selectedShape is System.Windows.Shapes.Polygon) //碰到多邊形時
                    {
                        myPointCollection.Clear();
                    }
                    break;
                default:
                    break;
            }
        }

        private void MyCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) //滑鼠放開
        {
            if (currentAction != "Erase")
            {
                switch (currentShape)
                {
                    case "Line":
                        DrawLine();
                        break;
                    case "Ellipse":
                        DrawEllipse();
                        break;
                    case "Rectangle":
                        DrawRectangle();
                        break;
                    case "Polygon":
                        if(myPointCollection.Count >= 3) //當點有3個(含)以上
                        {
                            DrawPolygon();
                        }
                        break;
                    default:
                        return;
                }
            }
            MyCanvas.Cursor = System.Windows.Input.Cursors.Arrow;
            
        }

        private void DrawPolygon() //畫多邊形
        {
            Polygon newPolygon = new Polygon();
            newPolygon.Fill = currentFillBrush;
            newPolygon.Stroke = currentStrokeBrush;
            newPolygon.StrokeThickness = currentStrokeThickness;
            newPolygon.Points = myPointCollection;
            MyCanvas.Children.Add(newPolygon);
        }

        private void DrawEllipse() //畫圓形
        {
            AdjustPoint();
            double width = dest.X - start.X;
            double height = dest.Y - start.Y;
            Ellipse newEllipse = new Ellipse()
            {
                Stroke = currentStrokeBrush,
                StrokeThickness = currentStrokeThickness,
                Width = width,
                Height = height,
                Fill = currentFillBrush
            };
            newEllipse.SetValue(Canvas.LeftProperty, start.X);
            newEllipse.SetValue(Canvas.TopProperty, start.Y);
            MyCanvas.Children.Add(newEllipse);
        }

        private void DrawRectangle() //畫矩形
        {
            AdjustPoint();
            double width = dest.X - start.X;
            double height = dest.Y - start.Y;
            Rectangle newRectangle = new Rectangle()
            {
                Stroke = currentStrokeBrush,
                StrokeThickness = currentStrokeThickness,
                Width = width,
                Height = height,
                Fill = currentFillBrush
            };
            newRectangle.SetValue(Canvas.LeftProperty, start.X);
            newRectangle.SetValue(Canvas.TopProperty, start.Y);
            MyCanvas.Children.Add(newRectangle);
        }

        private void AdjustPoint()
        {
            double X_min, Y_min, X_max, Y_max;
            X_min = Math.Min(start.X, dest.X);
            Y_min = Math.Min(start.Y, dest.Y);
            X_max = Math.Max(start.X, dest.X);
            Y_max = Math.Max(start.Y, dest.Y);

            start.X = X_min;
            start.Y = Y_min;
            dest.X = X_max;
            dest.Y = Y_max;
        }

        private void DrawLine() //畫線
        {
            Line newLine = new Line();
            newLine.Stroke = currentStrokeBrush;
            newLine.StrokeThickness = currentStrokeThickness;
            newLine.X1 = start.X;
            newLine.Y1 = start.Y;
            newLine.X2 = dest.X;
            newLine.Y2 = dest.Y;

            MyCanvas.Children.Add(newLine);
        }

        private void StrokeThicknessSliedr_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) //當滑條數值改變
        {
            currentStrokeThickness = Convert.ToInt32(StrokeThicknessSliedr.Value);
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)//當顏色按鈕按下
        {
            currentStrokeColor = GetDialogColor();
            currentStrokeBrush = new SolidColorBrush(currentStrokeColor);
            ColorButton.Background = currentStrokeBrush;
        }

        private Color GetDialogColor()//叫調色板
        {
            ColorDialog dlg = new ColorDialog();
            dlg.ShowDialog();
            System.Drawing.Color dlgColor = dlg.Color;
            return Color.FromArgb(dlgColor.A, dlgColor.R, dlgColor.G, dlgColor.B);
        }

        private void ShapeButton_Click(object sender, RoutedEventArgs e) //ShapeButton群組按下
        {
            System.Windows.Controls.RadioButton btn = sender as System.Windows.Controls.RadioButton;
            currentShape = btn.Content.ToString(); //按鈕內的文字
            currentAction = "Draw";
        }

        private void FillButton_Click(object sender, RoutedEventArgs e)
        {
            currentFillColor = GetDialogColor();
            currentFillBrush = new SolidColorBrush(currentFillColor);
            FillButton.Background = currentFillBrush;
        }

        private void EraseButton_Click(object sender, RoutedEventArgs e)
        {
            currentAction = "Erase";
        }

        private void MenuCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (MenuCheckBox.IsChecked == true)
            {
                MyToolBarTray.Visibility = Visibility.Visible;
                MyCanvas.Height -= MyToolBarTray.Height;
            }
            else
            {
                MyToolBarTray.Visibility = Visibility.Collapsed;
                MyCanvas.Height +=  MyToolBarTray.Height;
            }
        }

        private void ClearCanvasButton_Click(object sender, RoutedEventArgs e)
        {
            MyCanvas.Children.Clear();
            myPointCollection.Clear();
        }

        private void SaveCanvasMenuItem_Click(object sender, RoutedEventArgs e)//存檔
        {
            int w = Convert.ToInt32(MyCanvas.RenderSize.Width); 
            int h = Convert.ToInt32(MyCanvas.RenderSize.Height);

            //建立bitmap
            RenderTargetBitmap rtb = new RenderTargetBitmap(w, h, 64d, 64d, PixelFormats.Default); 
            rtb.Render(MyCanvas);
            //建立bitmap

            //編碼
            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(rtb));
            JpegBitmapEncoder jpge = new JpegBitmapEncoder();
            jpge.Frames.Add(BitmapFrame.Create(rtb));
            //編碼

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "儲存小畫家檔案";
            saveFileDialog.DefaultExt = "*.png|*.jpg";
            saveFileDialog.Filter = "PNG黨案(*.png)|*.png|全部檔案|*.*|JPG黨案(*.jpg)|*.jpg";
            saveFileDialog.ShowDialog();
            string path = saveFileDialog.FileName;

            using (var fs = File.Create(path)) //當下使用(使用完會將佔用記憶體清除)
            {
                png.Save(fs);
                jpge.Save(fs);
            }

        }

        private void ContextMenuCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (ContextMenuCheckBox.IsChecked == true)
            {
                MyToolBarTray.Visibility = Visibility.Visible;
                MyCanvas.Height -= MyToolBarTray.Height;
            }
            else
            {
                MyToolBarTray.Visibility = Visibility.Collapsed;
                MyCanvas.Height += MyToolBarTray.Height;
            }
        }

        private void PolygonButton_Click(object sender, RoutedEventArgs e)
        {
            currentAction = "Polygon";
            currentShape = "Polygon";
        }

        private void MyCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currentAction == "Draw")
            {
                MyCanvas.Cursor = System.Windows.Input.Cursors.Cross;
                start = e.GetPosition(MyCanvas);
                MyLabel.Content = $"座標點:{start}";
            }
            else if (currentAction == "Polygon")
            {
                MyCanvas.Cursor = System.Windows.Input.Cursors.Cross;
                start = e.GetPosition(MyCanvas);
                myPointCollection.Add(start);
            }
        }
    }
}
