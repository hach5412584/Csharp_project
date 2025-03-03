using Microsoft.Win32;
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
using System.Windows.Shapes;

namespace Textbook
{
    /// <summary>
    /// DocumentViewerWindow.xaml 的互動邏輯
    /// </summary>
    public partial class DocumentViewerWindow : Window
    {
        string FileName = "NULL";
        Color currentFontColor = Colors.Black;
        Color currentBackgroundColor = Colors.White;
        Brush currentFontBrush = new SolidColorBrush(Colors.Black);
        Brush currentBackgroundBrush = new SolidColorBrush(Colors.White);
        public DocumentViewerWindow()
        {
            InitializeComponent();
            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            cmbFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 24, 36, 48, 72 };
            if (FileName == "NULL")
            {
                FileName = "NewFile";
            }
            rtb_pos();
        }
        private void Open_Executed(object sender, ExecutedRoutedEventArgs e) //開檔
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Rich Text Format|*.rtf|ALL Files|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(openFileDialog.FileName, FileMode.Open);
                FileName = openFileDialog.FileName;
                TextRange textRange = new TextRange(rtbEdtior.Document.ContentStart, rtbEdtior.Document.ContentEnd);
                textRange.Load(fileStream, System.Windows.DataFormats.Rtf);
            }
            rtb_pos();
        }
        private void Save_Executed(object sender, ExecutedRoutedEventArgs e) //存檔
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "Rich Text Format|*.rtf|ALL Files|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate);
                TextRange textRange = new TextRange(rtbEdtior.Document.ContentStart, rtbEdtior.Document.ContentEnd);
                textRange.Save(fileStream, System.Windows.DataFormats.Rtf);
            }
        }
        private void New_Executed(object sender, ExecutedRoutedEventArgs e) //開新檔
        {
            DocumentViewerWindow myWindow = new DocumentViewerWindow();
            myWindow.Show();
            FileName = "NewFile";
        }

        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e) //字體形狀
        {
            rtbEdtior.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty,cmbFontFamily.SelectedItem);
        }

        private void cmbFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e) //字體大小
        {
            if (cmbFontSize.SelectedItem != null)
            {
                rtbEdtior.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, cmbFontSize.SelectedItem);
            }
        }
        void rtb_pos() //取得現在行數與字數 
        {
            TextPointer tp1 = rtbEdtior.Selection.Start.GetLineStartPosition(0);
            TextPointer tp2 = rtbEdtior.Selection.Start;
            int column = tp1.GetOffsetToPosition(tp2) - 1;

            int BigNumber = int.MaxValue;
            int lineMoved, currentLineNumber;
            rtbEdtior.Selection.Start.GetLineStartPosition(-BigNumber, out lineMoved);
            currentLineNumber = -lineMoved;

            textpos.Content = "Line: " + currentLineNumber.ToString() + "  Column: " + column.ToString() + "  FileName: " + FileName;
        }
        private void rtbEdtior_SelectionChanged(object sender, RoutedEventArgs e) //當關注目標改變
        {
            rtb_pos();
        }
        private void BackGroundColor_Click(object sender, RoutedEventArgs e) //背景顏色
        {
            currentBackgroundColor = GetDialogColor();
            currentBackgroundBrush = new SolidColorBrush(currentBackgroundColor);
            BackGroundColor.Background = currentBackgroundBrush;
            rtbEdtior.Background = currentBackgroundBrush;
        }
        private void FontColor_Click(object sender, RoutedEventArgs e) //字體顏色
        {
            currentFontColor = GetDialogColor();
            currentFontBrush = new SolidColorBrush(currentFontColor);
            rtbEdtior.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, currentFontBrush);
        }
        private Color GetDialogColor() //叫調色板
        {
            ColorDialog dlg = new ColorDialog();
            dlg.ShowDialog();
            System.Drawing.Color dlgColor = dlg.Color;
            return Color.FromArgb(dlgColor.A, dlgColor.R, dlgColor.G, dlgColor.B);
        }
    }
}
