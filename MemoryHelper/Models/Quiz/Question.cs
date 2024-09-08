using System;

namespace MemoryHelper.Models.Quiz
{
	internal class Question : ICloneable
	{
		internal string Text { get; private set; }// Question text
		internal string Answer { get; private set; }

		internal Question(string text, string answer)
		{
			Text = text;
			Answer = answer;
		}

		public object Clone()
			=> new Question(Text, Answer);
	}
}