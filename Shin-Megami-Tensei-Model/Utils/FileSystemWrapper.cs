namespace Shin_Megami_Tensei_Model.Utils;

public class FileSystemWrapper: IFileSystem
{
    public string ReadAllText(string path)
    {
        return File.ReadAllText(path);
    }

    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public string[] GetFiles(string directory, string pattern)
    {
        var files = Directory.GetFiles(directory, pattern);
        Array.Sort(files);
        return files;
    }
}