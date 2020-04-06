using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Typing_Tutor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string TargetText = "";
        public int CurrentIndex = 0, Errors, MAX;
        public string[] Files = { "C:/Users/David/source/repos/Typing Tutor/Typing Tutor/Text/ch2.txt", "C:/Users/David/source/repos/Typing Tutor/Typing Tutor/Text/ch3.txt", "C:/Users/David/source/repos/Typing Tutor/Typing Tutor/Text/ch4.txt", "C:/Users/David/source/repos/Typing Tutor/Typing Tutor/Text/ch5.txt" };
        public Stopwatch Timer = new Stopwatch();
        int RandIgnore;
        public MainWindow()
        {
            RandIgnore = Rand(Files.Length);
            FormatFile(Files[RandIgnore]);
            InitializeComponent();
            Output.Text = TargetText;
            Score.Text = "Right: " + CurrentIndex + "  Errors: " + Errors;
        }

        public int Rand(int max)
        {
            Random rnd = new Random();
            int x = rnd.Next(0, max);
            return x;
        }

        public void FormatFile(string path)
        {
            StreamReader myFile = new StreamReader(path);

            string x = myFile.ReadToEnd().ToString();
            x = Regex.Replace(x, @"\s+", " ");
            TargetText = x;
            Timer.Start();
            Console.WriteLine(TargetText);
               
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            DonePanel.Visibility = Visibility.Hidden;
            int r = Rand(Files.Length);
            if (r != RandIgnore)
            {
                FormatFile(Files[r]);
                RandIgnore = r;
            }
            else {
                NextButton_Click(null,null);
            }
            Output.Text = TargetText;
            Errors = 0;
            CurrentIndex = 0;
            Score.Text = "Right: " + CurrentIndex + "  Errors: " + Errors;
        }

        private void MainMenu_Click(object sender, RoutedEventArgs e)
        {
            Custom.Visibility = Visibility.Visible;
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            Timer.Stop();
            double Grade = Convert.ToDouble((CurrentIndex - Errors)) / Convert.ToDouble(TargetText.Length) * 100;
            DonePanel.Visibility = Visibility.Visible;
            if (Grade < 0) { Grade = 0; }
            DoneText.Text = "Grade :  " + Grade + "%   Wpm =" + Convert.ToDouble(Timer.ElapsedMilliseconds/ 20) / Convert.ToDouble((1+CurrentIndex)/ 3);
        }

        private void Custombtn_Click(object sender, RoutedEventArgs e)
        {        
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != "")
            {
                Timer.Stop();
                Errors = 0;
                CurrentIndex = 0;
                FormatFile(openFileDialog.FileName);
                Output.Text = TargetText;
            }
            e.Handled = true;
            Score.Text = "Right: " + CurrentIndex + "  Errors: " + Errors;
        }

        private void GetTyped(object sender, KeyEventArgs e)
        {
            Console.WriteLine(TargetText[CurrentIndex]);
            if (CurrentIndex != TargetText.Length)
            {
                if (e.Key.ToString() == TargetText[CurrentIndex].ToString().ToUpper())
                {
                    Console.WriteLine("Correct");
                    CurrentIndex++;
                    Output.Text = Output.Text.Substring(1, TargetText.Length - CurrentIndex);
                }
                else
                {
                    if ((e.Key == Key.OemQuestion && "?" == TargetText[CurrentIndex].ToString()) || (e.Key == Key.OemMinus && "-" == TargetText[CurrentIndex].ToString()) || (e.Key == Key.OemSemicolon && "" == TargetText[CurrentIndex].ToString()) || (e.Key == Key.OemSemicolon && ";" == TargetText[CurrentIndex].ToString()) || (e.Key == Key.OemComma && "," == TargetText[CurrentIndex].ToString()) || (e.Key == Key.OemPeriod && "." == TargetText[CurrentIndex].ToString()) || (e.Key == Key.Space && TargetText[CurrentIndex].ToString() == " "))
                    {
                        Console.WriteLine("Correct");
                        CurrentIndex++;
                        Output.Text = Output.Text.Substring(1, TargetText.Length - CurrentIndex);
                    }
                    else
                    {
                        Console.WriteLine("Errors");
                        Errors++;
                    }
                }
            }
            if (CurrentIndex == TargetText.Length)
            {
                Done_Click(null, null);
            }

            Score.Text = "Right: " + CurrentIndex + "  Errors: " + Errors;
        }
    }
}