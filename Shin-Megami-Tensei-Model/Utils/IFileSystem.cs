namespace Shin_Megami_Tensei_Model.Utils;

public interface IFileSystem
{
    string ReadAllText(string path);
    bool FileExists(string path);
    string[] GetFiles(string directory, string pattern);
}