using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using MemoryHelper.Models.Quiz;

namespace MemoryHelper.Models.FileManagement
{
	internal static class FileManager
	{
		private const char QUESTION_START = '{';
		private const char QUESTION_SEPARATOR = '|';
		private const char QUESTION_END = '}';
		private const string DEFAULT_DIRECTORY_NAME = "Questions";
		private const string DEFAULT_TEXT = "{Question|Answer}\n";

		internal static readonly string Path = $"{Environment.CurrentDirectory}";

		internal static bool IsFileExist(string path)
			=> File.Exists(path);
		internal static void DeleteFile(string path)
		{
			if (File.Exists(path))
				File.Delete(path);
		}
		internal static string CreateTextFileAndReturnFullPath(string name)
		{
			Directory.CreateDirectory($"{Path}\\{DEFAULT_DIRECTORY_NAME}");
			string path = $"{Path}\\{DEFAULT_DIRECTORY_NAME}\\{name}.txt";

			File.WriteAllText(path, DEFAULT_TEXT);
			return path;
		}
		internal static List<FileElement> GetListFiles()
		{
			List<FileElement> fileElements = new List<FileElement>();

			foreach (string currentPath in Directory.GetFiles(Path, "*txt", SearchOption.AllDirectories))
			{
				FileElement element = new FileElement(currentPath, System.IO.Path.GetFileNameWithoutExtension(currentPath));
				fileElements.Add(element);
			}

			return fileElements;
		}
		internal static List<Question> GetQuestions(string path)
		{
			List<Question> questions = new List<Question>();

			string allText = File.ReadAllText(path);
			StringBuilder questionText = new StringBuilder(),
				questionAnswer = new StringBuilder();

			int i = 0;
			while (i < allText.Length)
			{// Loop through file text
				i = MoveCounterToChar(allText, i, QUESTION_START);
				if (i >= allText.Length)
					break;
				i++;

				while (i < allText.Length && allText[i] != QUESTION_SEPARATOR)
					questionText.Append(allText[i++]);// Reading the question
				i++;

				while (i < allText.Length && allText[i] != QUESTION_END)
					questionAnswer.Append(allText[i++]);// Reading the answer
				i++;

				Question currentQuestion = new Question(questionText.ToString().Trim('\r', '\n'),
					questionAnswer.ToString().Trim('\r', '\n'));
				questions.Add(currentQuestion);

				questionText.Clear();
				questionAnswer.Clear();
			}

			return questions;
		}
		private static int MoveCounterToChar(string text, int i, char ch)
		{// The method moves i to the desired character
			for (; i < text.Length; i++)
				if (text[i] == ch)
					break;

			return i;
		}
	}
}