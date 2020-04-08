using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Typing_Tutor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Random rnd = new Random(); // set rand here to avoid overflow
        public string TargetText = ""; // test that must be typed in
        public int CurrentIndex = 0, Errors, MAX; // where we are in target text, how many we have wrong, Max needed to continue
        public string[] Files = { "C:/Users/David/source/repos/Typing Tutor/Typing Tutor/Text/ch2.txt", "C:/Users/David/source/repos/Typing Tutor/Typing Tutor/Text/ch3.txt", "C:/Users/David/source/repos/Typing Tutor/Typing Tutor/Text/ch4.txt", "C:/Users/David/source/repos/Typing Tutor/Typing Tutor/Text/ch5.txt" };
        public Stopwatch Timer = new Stopwatch(); //Timer that counts how long time has pasted
        private int RandIgnore; // number to ingnore durring new file index randimazation so we do not repeat

        public MainWindow()
        {
            InitializeComponent();
          
            foreach (var item in Files)
            {
                FileCollection.Items.Add(item); // files into collections
            }
            RandIgnore = Rand(FileCollection.Items.Count); //set a new file index
            FormatFile(FileCollection.Items[RandIgnore].ToString()); //format a random file and set it
            Output.Text = TargetText; // make it the set output
        }

        //random number from 0 to max
        public int Rand(int max)
        {
            int x = rnd.Next(0, max);
            return x;
        }

        //remove double spaces and tabs from file in path set it as target text
        public void FormatFile(string path)
        {
            StreamReader myFile = new StreamReader(path);
            string x = myFile.ReadToEnd().ToString();
            x = Regex.Replace(x, @"\s+", " "); //removes more any double spaces or indents
            TargetText = x; //could be output = this
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (DonePanel.Visibility == Visibility.Visible)
            {
                DonePanel.Visibility = Visibility.Hidden;
            }
            int r = Rand(FileCollection.Items.Count);
            if (r != RandIgnore) //if this is a new random number
            {
                FormatFile(FileCollection.Items[r].ToString());
                RandIgnore = r;
            }
            else
            {
                NextButton_Click(null, null); //redo if repated
            }
            Output.Text = TargetText;
            Errors = 0;
            CurrentIndex = 0;
            Score.Text = "Right: " + CurrentIndex + "  Errors: " + Errors;
            Timer.Start();
        }

        private void MainMenu_Click(object sender, RoutedEventArgs e)
        {
            StartButton.Visibility = Visibility.Visible;
            Title.Visibility = Visibility.Visible;
            Output.Visibility = Visibility.Hidden;
            FileCollection.Visibility = Visibility.Visible;
            DonePanel.Visibility = Visibility.Hidden;
            dis.Visibility = Visibility.Visible;
            Custom.Visibility = Visibility.Visible;
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            Timer.Stop();
            double Grade = Convert.ToDouble((CurrentIndex - Errors)) / Convert.ToDouble(TargetText.Length) * 100;
            DonePanel.Visibility = Visibility.Visible;
            double WPM;
            if (Grade < 0) { Grade = 0; }
            if (CurrentIndex>0)
            {
                 WPM = Convert.ToDouble(Timer.ElapsedMilliseconds / 28);
            }
            else {
                WPM = 0;
            }
            DoneText.Text = "Grade :  " + Grade + "%   Wpm =" + WPM / Convert.ToDouble((1 + CurrentIndex) / 3);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.Visibility = Visibility.Hidden;
            Title.Visibility = Visibility.Hidden;
            Output.Visibility = Visibility.Visible;
            Custom.Visibility = Visibility.Hidden;
            FileCollection.Visibility = Visibility.Hidden;
            DonePanel.Visibility = Visibility.Hidden;
            dis.Visibility = Visibility.Hidden;
            Timer.Start();
        }

        private void FileSelection(object sender, MouseButtonEventArgs e)
        {
            var Item = ItemsControl.ContainerFromElement(sender as ListBox, e.OriginalSource as DependencyObject) as ListBoxItem;
            FormatFile(Item.Content.ToString());
            Output.Text = TargetText;
            RandIgnore = FileCollection.Items.IndexOf(Item);
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
                FileCollection.Items.Add(openFileDialog.FileName);
            }
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
                    if ((e.Key == Key.OemBackslash && "/" == TargetText[CurrentIndex].ToString()) || (e.Key == Key.OemQuestion && "?" == TargetText[CurrentIndex].ToString()) || (e.Key == Key.OemMinus && "-" == TargetText[CurrentIndex].ToString()) || (e.Key == Key.OemSemicolon && "" == TargetText[CurrentIndex].ToString()) || (e.Key == Key.OemSemicolon && ";" == TargetText[CurrentIndex].ToString()) || (e.Key == Key.OemComma && "," == TargetText[CurrentIndex].ToString()) || (e.Key == Key.OemPeriod && "." == TargetText[CurrentIndex].ToString()) || (e.Key == Key.Space && TargetText[CurrentIndex].ToString() == " "))
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