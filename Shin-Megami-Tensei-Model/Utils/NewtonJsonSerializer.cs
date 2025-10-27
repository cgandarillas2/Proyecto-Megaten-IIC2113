using Newtonsoft.Json;

namespace Shin_Megami_Tensei_Model.Utils;

public class NewtonJsonSerializer: IJsonSerializer
{
    public T Deserialize<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }

    public string Serialize<T>(T obj)
    {
        return JsonConvert.SerializeObject(obj, Formatting.Indented);
    }
}