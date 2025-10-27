namespace Shin_Megami_Tensei_Model.Utils;

public interface IJsonSerializer
{
    T Deserialize<T>(string json);
    string Serialize<T>(T obj);
}