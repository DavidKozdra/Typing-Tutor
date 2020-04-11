using System;
using System.IO;
using Typing_Tutor;


namespace Typing_Tutor
{
	public class FileManager
	{
		private static readonly string PreferancesLocation = "Resources/Preferances.txt";
		private static char DataSeperator = '`';

		public static void Create(params string[] Data)
		{
			var content = "";

			foreach (var item in Data)
			{
				content += item;
			}
			System.IO.File.WriteAllText(PreferancesLocation, content);

		}
		public static string[] Load()
		{
			if (!File.Exists(PreferancesLocation))
			{

				return null;
			}

			return File.ReadAllText(PreferancesLocation).Split(DataSeperator); ;
		}
	}

}

