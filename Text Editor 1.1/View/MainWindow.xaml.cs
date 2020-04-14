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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Text_Editor_1._1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _currentFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Document.rtf";
        private int _wordCount = 0;
        public MainWindow()
        {

            InitializeComponent();
            this.DataContext = this;

            foreach (var item in Fonts.SystemFontFamilies)
            {
                fontbox.Items.Add(item.Source);

            }
            fontbox.SelectedIndex = 0;

            for (int i = 6; i < 32; i++)
            {
                fonstsize.Items.Add((i * 2 + 2).ToString());
            }

            fonstsize.SelectedIndex = 0;

            fileNameTxtBox.Text = _currentFilePath;
        }

        private void savebtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sv = new System.Windows.Forms.SaveFileDialog();
            sv.Filter = "Text file (*.rtf)|*.rtf";
            if (sv.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextRange range;

                FileStream fStream;

                range = new TextRange(textarea.Document.ContentStart, textarea.Document.ContentEnd);

                fStream = new FileStream($"{sv.FileName}", FileMode.Create);

                range.Save(fStream, System.Windows.DataFormats.Rtf);

                fStream.Close();

                _currentFilePath = sv.FileName;
                fileNameTxtBox.Text = _currentFilePath;

                this.Title = System.IO.Path.GetFileNameWithoutExtension(sv.FileName) + " - Text Editor 1.1";
            }
        }

        private void fontbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var currentFont = fontbox.SelectedItem;
            textarea.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, currentFont);
        }

        private void fonstsize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            double fontSize = 0;
            double.TryParse(fonstsize.SelectedItem.ToString(), out fontSize);
            textarea.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
        }

        private void undobtn_Click(object sender, RoutedEventArgs e)
        {
            textarea.Undo();
        }

        private void redobtn_Click(object sender, RoutedEventArgs e)
        {
            textarea.Redo();
        }

        private void cutbtnbtn_Click(object sender, RoutedEventArgs e)
        {
            textarea.Cut();
        }

        private void copybtn_Click(object sender, RoutedEventArgs e)
        {
            textarea.Copy();
        }

        private void pastebtn_Click(object sender, RoutedEventArgs e)
        {
            textarea.Paste();
        }
        private void selectallbtn_Click(object sender, RoutedEventArgs e)
        {
            textarea.SelectAll();
            textarea.SelectionBrush = Brushes.LightBlue;
            textarea.AutoWordSelection = true;
        }



        private void colorpickerbtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog cd = new System.Windows.Forms.ColorDialog();
            cd.ShowDialog();

            System.Drawing.Color dc = cd.Color;

            System.Windows.Media.Color mc = ColorToColor(dc);

            Brush brush = new SolidColorBrush(mc);

            textarea.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
        }
        public static System.Windows.Media.Color ColorToColor(System.Drawing.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        private void textarea_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (auto_save.IsChecked == true)
            {
                TextRange range;
                FileStream fStream;
                range = new TextRange(textarea.Document.ContentStart, textarea.Document.ContentEnd);
                fStream = new FileStream($"{_currentFilePath}", FileMode.OpenOrCreate);
                range.Save(fStream, System.Windows.DataFormats.Rtf);
                fStream.Close();
            }

            if (string.IsNullOrEmpty(GetText(textarea)))
            {
                wordcountlbl.Content = "Words : 0";
            }
            else
            {
                wordcountlbl.Content = "Words: " + FindWordsCount();
            }

        }

        private long FindWordsCount()
        {

            string[] words = new string[1];

            words = GetText(textarea).Split(' ', ',', '\n');

            return words.Length - 1;
        }
        private string GetText(System.Windows.Controls.RichTextBox richTextBox)
        {
            TextRange content = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            return content.Text;
        }

        private void speellcheckbtn_Click(object sender, RoutedEventArgs e)
        {
            var brush = new ImageBrush();

            if (textarea.SpellCheck.IsEnabled)
            {
                textarea.SpellCheck.IsEnabled = false;
                speellcheckbtn.Content = FindResource("disable");
            }
            else
            {
                textarea.SpellCheck.IsEnabled = true;
                speellcheckbtn.Content = FindResource("enable");

            }

            speellcheckbtn.Background = brush;
        }

        private void calendarbtn_Click(object sender, RoutedEventArgs e)
        {

            System.IO.MemoryStream stream = new System.IO.MemoryStream(ASCIIEncoding.Default.GetBytes(DateTime.Now.ToShortDateString()));
            textarea.Selection.Load(stream, DataFormats.Rtf);

        }

        private void leftbtn_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.AlignLeft.Execute(null, textarea);
        }

        private void centerbtn_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.AlignCenter.Execute(null, textarea);
        }

        private void rightbtn_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.AlignRight.Execute(null, textarea);
        }

        private void loadbtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog of = new System.Windows.Forms.OpenFileDialog();
            of.Filter = "rtf files (*.rtf)|*.rtf";

            if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = of.FileName;

                TextRange range;

                FileStream fStream;

                if (File.Exists(fileName))
                {
                    range = new TextRange(textarea.Document.ContentStart, textarea.Document.ContentEnd);

                    fStream = new FileStream(fileName, FileMode.OpenOrCreate);
                    range.Load(fStream, System.Windows.DataFormats.Rtf);

                    fStream.Close();

                    this.Title = System.IO.Path.GetFileNameWithoutExtension(of.FileName) + " - Text Editor 1.1";

                    _currentFilePath = of.FileName;
                    fileNameTxtBox.Text = _currentFilePath;
                    _currentFilePath = of.FileName;
                }
            }
        }
    }
}
