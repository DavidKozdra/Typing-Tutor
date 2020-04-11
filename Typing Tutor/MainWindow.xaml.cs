using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using Typing_Tutor;

namespace Typing_Tutor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	///

public static class FileManager{
		private static readonly string PD="Resources";
		private static string PreferancesLocation = $"{PD}/Preferances.txt";
		private static char DataSeperator = '`';
		public static string Content;

		public static void Remove(string name) {
			var x = Load();
			if(x.Length <= 1){
				File.Delete(PreferancesLocation);
				return;
			}
				List<string> strings = new List<string>();
				foreach (var item in x)
				{
					if(item.ToLower() ==name.ToLower()){
							continue;
					}
							strings.Add(item);
					Console.WriteLine(name);
				}
				CreateWithoutLoad(strings.ToArray());
		}
			static void Add(string item) {
				if(Content != string.Empty){
				Content += DataSeperator;
				}
				Content += item;
			}
		public static void CreateWithoutLoad(params string[] Data)
		{
			if(!Directory.Exists(PD)){
				Directory.CreateDirectory(PD);
			}

			Content = "";
			foreach (var item in Data)
			{
				 Add(item);
			}

			System.IO.File.WriteAllText(PreferancesLocation,Content);
		}

		public static void Create(params string[] Data)
		{
			if(!Directory.Exists(PD)){
				Directory.CreateDirectory(PD);
			}

			var Content = "";
			void Add(string item) {
				if(Content != string.Empty){
				Content += DataSeperator;
				}
				Content += item;
			}
			foreach (var item in Data)
			{
				 Add(item);
			}
			if(File.Exists(PreferancesLocation)){
				foreach (var item in Load()) Add(item);
			}
			System.IO.File.WriteAllText(PreferancesLocation,Content);
		}
		public static string[] Load()
		{
			if (!File.Exists(PreferancesLocation))
			{
				return null;
			}
			string [] Data={};

			void AddToData(string item) {
				if(Data == null || Data.Length<1){
					Data = new string[]{item};
				}
				else{
					string[] OldData = Data;
					Data = new string[OldData.Length+1];
				  for (int i = 0; i < OldData.Length; i++){
					Data[i] = OldData[i];
				  }
				  Data[Data.Length-1] = item;
				}
			}
			var Content =File.ReadAllText(PreferancesLocation).Split(DataSeperator);
			if(Content.Length <=1){
				return Data;
			}
			foreach (var item in Content)
			{
				var _Data="";

				for (int i = 0; i < item.Length; i++)
				{
					var _char =item[i];
					if(_char==DataSeperator){
					continue;
					}
					_Data +=_char;
				}
				AddToData(_Data);
			}

			return Data;
		}
	}
	public partial class MainWindow : Window
	{
		private Random rnd = new Random(); // set rand here to avoid overflow
		public string TargetText; // test that must be typed in
		public int CurrentIndex, Errors, MAX; // where we are in target text, how many we have wrong, Max needed to continue
		public string[] Files;

		public Stopwatch Timer = new Stopwatch(); //Timer that counts how long time has pasted
		private int RandIgnore; // number to ingnore durring new file index randimazation so we do not repeat

		public MainWindow()
		{
			InitializeComponent();
			Files = FileManager.Load();
			if(Files !=null){
			foreach (string item in Files)
			{
				FileCollection.Items.Add(item); // files into collections
			}
			}

		   // RandIgnore = Rand(FileCollection.Items.Count); //set a new file index
			//FormatFile(FileCollection.Items[RandIgnore].ToString()); //format a random file and set it
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
			//* stack overflow
			if (DonePanel.Visibility == Visibility.Visible)
			{
				DonePanel.Visibility = Visibility.Hidden;
			}
			 int r=0,i =0;
			while ( r == RandIgnore) //if this is a new random number
			{
				i++;
				r= Rand(FileCollection.Items.Count);
				if(i>100){
					break;
				}
			}
			FormatFile(FileCollection.Items[r].ToString());
			RandIgnore = r;
			Output.Text = TargetText;
			Errors = 0;
			CurrentIndex = 0;
			Score.Text = "Right: " + CurrentIndex + "  Errors: " + Errors;
			Timer.Start();
		}
		private void Remove_Click(object sender, RoutedEventArgs e)
		{
			if(FileCollection.SelectedItem!=null){
				FileManager.Remove((FileCollection.SelectedItem).ToString());
				FileCollection.Items.Remove(FileCollection.SelectedItem);
			}
			else {
				Console.WriteLine("Null");
			}
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
			Remove.Visibility=Visibility.Visible;
		}

		private void Done_Click(object sender, RoutedEventArgs e)
		{
			Timer.Stop();
			double Grade=0;
			if(TargetText !=null){
			 Grade = Convert.ToDouble((CurrentIndex - Errors)) / Convert.ToDouble(TargetText.Length) * 100;
				}
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
			DoneText.Text = "Grade :  " + System.Math.Round(Grade,2) + "%   Wpm =" +  System.Math.Round(WPM / Convert.ToDouble((1 + CurrentIndex) / 3),2);
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
			Remove.Visibility = Visibility.Hidden;
			Timer.Start();
		}

		private void FileSelection(object sender, MouseButtonEventArgs e)
		{
			var Item = ItemsControl.ContainerFromElement(sender as ListBox, e.OriginalSource as DependencyObject) as ListBoxItem;
			if(Item!=null){
			FormatFile(Item.Content.ToString());
			Output.Text = TargetText;
			RandIgnore = FileCollection.Items.IndexOf(Item);
			}
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
				FileManager.Create(openFileDialog.FileName);
				Output.Text = TargetText;
				FileCollection.Items.Add(openFileDialog.FileName);
			}
		}

		private void GetTyped(object sender, KeyEventArgs e)
		{
			if ( TargetText !=null && CurrentIndex != TargetText.Length )
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
			if (TargetText !=null &&CurrentIndex == TargetText.Length)
			{
				Done_Click(null, null);
			}
			Score.Text = "Right: " + CurrentIndex + "  Errors: " + Errors;
		}
	}
}