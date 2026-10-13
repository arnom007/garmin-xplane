using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LOLA.StringHelpers;

namespace LauncherCoreLib.Models.Winfs;

public static class EmbeddedStrings
{
	public static readonly Regex VALID_EMBEDDED_FILE_NAME_REGEX = new Regex("^[A-Za-z0-9_ !@#\\$%&\\(\\)\\+\\-;',.£€¥]+$");

	private static readonly char[] VALID_EMBEDDED_FILENAME_CHARACTERS = PrepareFileNameChars();

	public static System.Collections.Generic.IList<char> InvalidCharactersInFileName(string embeddedFilename)
	{
		return TextHelpers.InvalidCharactersFromWhitelist(((System.Collections.Generic.IEnumerable<char>)embeddedFilename).ToStableDistinctCharList(), VALID_EMBEDDED_FILENAME_CHARACTERS);
	}

	private static char[] PrepareFileNameChars()
	{
		List<char> val = new List<char>();
		char[][] array = new char[12][]
		{
			new char[2] { 'a', 'z' },
			new char[2] { 'A', 'Z' },
			new char[2] { '0', '9' },
			new char[2] { ' ', '!' },
			new char[2] { '#', ')' },
			new char[2] { '+', '.' },
			new char[2] { '_', '_' },
			new char[2] { ';', ';' },
			new char[2] { '@', '@' },
			new char[2] { '€', '€' },
			new char[2] { '¥', '¥' },
			new char[2] { '£', '£' }
		};
		for (int i = 0; i < array.Length; i++)
		{
			char num = array[i][0];
			char c = array[i][1];
			for (char c2 = num; c2 <= c; c2 = (char)(c2 + 1))
			{
				val.Add(c2);
			}
		}
		char[] array2 = val.ToArray();
		System.Array.Sort<char>(array2);
		return array2;
	}
}
