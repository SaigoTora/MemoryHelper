
namespace MemoryHelper.Models.FileManagement
{
	internal class FileElement
	{
		internal string Path { get; private set; }// Full path of the file
		internal string Name { get; private set; }

		internal FileElement(string path, string name)
		{
			Path = path;
			Name = name;
		}
	}
}