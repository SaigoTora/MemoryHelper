using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using MemoryHelper.Models.FileManagement;
using MemoryHelper.Models.Quiz;
using MemoryHelper.Models.Utilities;

namespace MemoryHelper
{
	internal class Program
	{
		private static List<FileElement> files = FileManager.GetListFiles();
		private static readonly Random _random = new Random();

		private static void Main(string[] args)
		{
			Console.WriteLine($"Hello {Environment.UserName}! MemoryHelper allows you to learn the necessary material faster.");
			if (files.Count == 0)
				ShowInitialInstructions();

			Run();
		}

		private static void ShowInitialInstructions()
		{
			Console.WriteLine($"To use the application you need to create text files here:\n{FileManager.Path}");
			CreateTextFile();
			WaitForKeyPress();
			Console.Clear();
		}
		private static void Run()
		{
			while (true)
			{
				List<string> options = new List<string>() { "Options menu" };
				options.AddRange(files.Select(f => f.Name));
				PrintText(StringFormatter.FormatStrings(options, "Select the topic of interest:", 0), ConsoleColor.Yellow);

				int topicNumber = GetInputNumber("Enter topic number: ", 0, files.Count + 1);
				Console.Clear();
				if (topicNumber == 0)
				{
					OpenOptionsMenu();
					WaitForKeyPress();
				}
				else
					OpenTopicMenu(files[topicNumber - 1]);
				Console.Clear();
			}
		}
		private static void OpenOptionsMenu()
		{
			List<string> optionsText = new List<string>{ "Create a file with questions",
						"Update list of questions", "Delete file with questions",
						"Back", "Close the program"};
			PrintText(StringFormatter.FormatStrings(optionsText, "OPTIONS MENU:", 0), ConsoleColor.Yellow);

			int actionNumber = GetInputNumber("Enter action number: ", 0, optionsText.Count);
			Console.Clear();
			switch (actionNumber)
			{
				case 0:
					CreateTextFile();
					break;
				case 1:
					RefreshQuestionsList();
					PrintText("The list of questions has been successfully updated!", ConsoleColor.Green);
					break;
				case 2:
					DeleteTextFile();
					break;
				case 3:
					return;
				case 4:
					ExitProgram();
					break;
				default:
					PrintText("Invalid action selected! Please try again.", ConsoleColor.Red);
					break;
			}
		}
		private static void OpenTopicMenu(FileElement currentFile)
		{
			while (true)
			{
				if (!FileManager.IsFileExist(currentFile.Path))
				{
					PrintText($"File named {currentFile.Name} doesn't exist!", ConsoleColor.Red);
					RefreshQuestionsList();
					WaitForKeyPress();
					return;
				}

				Console.WriteLine(StringFormatter.FormatString($"Selected topic: \"{currentFile.Name}\""));
				List<string> optionsText = new List<string>{ "Open file with questions",
						"Start asking questions","Back to menu","Close the program"};
				PrintText(StringFormatter.FormatStrings(optionsText, "MENU:", 0), ConsoleColor.Yellow);

				int actionNumber = GetInputNumber("Enter action number: ", 0, optionsText.Count);
				switch (actionNumber)
				{
					case 0:
						using (var process = Process.Start(currentFile.Path))
						{ process.WaitForExit(); }
						break;
					case 1:
						List<Question> questions = FileManager.GetQuestions(currentFile.Path);
						AskQuestions(questions);
						break;
					case 2: return;
					case 3:
						ExitProgram();
						break;
					default:
						PrintText("Invalid action selected! Please try again.", ConsoleColor.Red);
						break;
				}
				Console.Clear();
			}
		}

		private static void CreateTextFile()
		{
			Console.Write("Enter the name of the text file you want to create: ");
			string name = Console.ReadLine();
			FileElement fileElement = files.Find(f => f.Name == name);
			if (fileElement != null && FileManager.IsFileExist(fileElement.Path))
			{
				PrintText($"A file named {name} already exists!", ConsoleColor.Red);
				return;
			}
			string path = FileManager.CreateTextFileAndReturnFullPath(name);
			RefreshQuestionsList();

			Console.Clear();
			PrintText($"The text file named \"{name}\" was successfully created!", ConsoleColor.Green);
			Console.WriteLine("The format of the questions is as follows:\n{ Question | Answer }");
		}
		private static void RefreshQuestionsList()
			=> files = FileManager.GetListFiles();
		private static void DeleteTextFile()
		{
			var fileNames = files.Select(f => f.Name);
			Console.WriteLine(StringFormatter.FormatStrings(fileNames, "All files:"));

			Console.Write("Enter the name of the text file you want to delete: ");
			string fileName = Console.ReadLine();

			FileElement fileElement = files.Find(f => f.Name == fileName);
			if (fileElement == null)
			{
				PrintText($"File named \"{fileName}\" doesn't exist", ConsoleColor.Red);
				return;
			}
			files.Remove(fileElement);
			FileManager.DeleteFile(fileElement.Path);

			PrintText($"The text file named \"{fileName}\" was successfully deleted!", ConsoleColor.Green);
		}

		private static int GetInputNumber(string caption, int? minValue = null, int? maxValue = null)
		{
			bool isInputValid = default;
			int result = default;

			while (!isInputValid)
			{
				Console.Write(caption);
				isInputValid = Int32.TryParse(Console.ReadLine(), out result);
				if (!isInputValid)
					PrintText(StringFormatter.FormatString("Invalid number format!"), ConsoleColor.Red);
				else if (minValue.HasValue && result < minValue || maxValue.HasValue && result >= maxValue)
				{
					PrintText(StringFormatter.FormatString("This number doesn't exist!"), ConsoleColor.Red);
					isInputValid = false;
				}
			}
			return result;
		}
		private static void PrintText(string text, ConsoleColor color)
		{
			ConsoleColor originalColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.WriteLine(text);
			Console.ForegroundColor = originalColor;
		}
		private static void WaitForKeyPress(string caption = "Press any key to continue...")
		{
			Console.Write(caption);
			Console.ReadKey();
		}

		private static void AskQuestions(List<Question> questions)
		{
			if (questions.Count == 0)
			{
				Console.WriteLine("There are no questions in this file yet.");
				WaitForKeyPress();
				return;
			}
			questions = ShuffleQuestions(questions);
			List<Question> wrongQuestions = new List<Question>();

			for (int i = 0; i < questions.Count; i++)
			{
				Console.Clear();
				PrintText(StringFormatter.FormatString($"Question {i + 1} of {questions.Count}:\n" +
					$"{questions[i].Text}"), ConsoleColor.Yellow);

				if (questions[i].Answer != string.Empty)
				{
					WaitForKeyPress("Press any key to see the answer...");
					Console.WriteLine();
					PrintText(StringFormatter.FormatString($"Answer:\n{questions[i].Answer}"), ConsoleColor.Green);
				}
				Console.WriteLine();
				bool isCorrect = AskYesOrNo("Was your answer correct?");
				if (!isCorrect)
					wrongQuestions.Add(questions[i]);
			}

			Console.Clear();
			AskIncorrectResults(questions, wrongQuestions, questions.Count - wrongQuestions.Count);
		}
		private static void AskIncorrectResults(List<Question> questions, List<Question> wrongQuestions, int countCorrectAnswers)
		{
			PrintResults(questions, countCorrectAnswers);
			Thread.Sleep(1000);
			if (wrongQuestions.Count != 0)
			{
				if (AskYesOrNo("Do you want to practice incorrect answers?"))
					AskQuestions(wrongQuestions);
			}
			else
				WaitForKeyPress();
		}
		private static void PrintResults(List<Question> questions, int countCorrectAnswers)
		{
			double percentage = ((double)countCorrectAnswers / questions.Count) * 100;
			Console.WriteLine(StringFormatter.FormatString($"Results:\n{countCorrectAnswers} / {questions.Count} correct answers ({percentage:0.##}%)."));
		}

		private static List<Question> ShuffleQuestions(List<Question> questions)
		{
			List<Question> questionClones = questions.Select(question => (Question)question.Clone()).ToList();
			List<Question> resultList = new List<Question>(questions.Count);
			int randomIndex;

			while (questionClones.Count > 0)
			{
				randomIndex = _random.Next(questionClones.Count);
				resultList.Add(questionClones[randomIndex]);
				questionClones.RemoveAt(randomIndex);
			}

			return resultList;
		}
		private static bool AskYesOrNo(string question)
		{
			PrintText(StringFormatter.FormatString(question + "\nY - Yes      N - No"), ConsoleColor.Yellow);
			while (true)
			{
				ConsoleKey key = Console.ReadKey().Key;
				if (key == ConsoleKey.Y)
					return true;
				else if (key == ConsoleKey.N)
					return false;
			}
		}

		private static void ExitProgram() => Environment.Exit(0);
	}
}