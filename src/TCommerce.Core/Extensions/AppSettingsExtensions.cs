using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;

public static class AppSettingsExtensions
{
    private static string filePath = Directory.GetCurrentDirectory() + "\\appsettings.json";

    public static string GetKey(string key)
    {
        var json = File.ReadAllText(filePath);
        dynamic jsonObj = JObject.Parse(json);
        var keys = key.Split(':');
        dynamic tempObj = jsonObj;

        for (int i = 0; i < keys.Length; i++)
        {
            if (tempObj[keys[i]] == null)
            {
                return string.Empty;
            }
            tempObj = tempObj[keys[i]];
        }

        return tempObj.ToString();
    }

    // Tạo một key mới trong file appsettings.json
    public static void CreateKey(string key)
    {
        var json = File.ReadAllText(filePath);
        dynamic jsonObj = JObject.Parse(json);
        var keys = key.Split(':');
        dynamic tempObj = jsonObj;

        for (int i = 0; i < keys.Length - 1; i++)
        {
            if (tempObj[keys[i]] == null)
            {
                tempObj[keys[i]] = new JObject();
            }
            tempObj = tempObj[keys[i]];
        }

        if (tempObj[keys.Last()] != null)
        {
            return;
        }

        tempObj[keys.Last()] = "";
        string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
        File.WriteAllText(filePath, output);
    }

    // Thêm dữ liệu vào một key trong file appsettings.json
    public static void AddToKey(string key, string value)
    {
        var json = File.ReadAllText(filePath);
        dynamic jsonObj = JObject.Parse(json);
        var keys = key.Split(':');
        dynamic tempObj = jsonObj;

        for (int i = 0; i < keys.Length - 1; i++)
        {
            if (tempObj[keys[i]] == null)
            {
                throw new Exception("Key does not exist");
            }
            tempObj = tempObj[keys[i]];
        }

        if (tempObj[keys.Last()] == null)
        {
            throw new Exception("Key does not exist");
        }

        tempObj[keys.Last()] = value;
        string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
        File.WriteAllText(filePath, output);
    }

    // Xóa một key trong file appsettings.json
    public static void DeleteKey(string key)
    {
        var json = File.ReadAllText(filePath);
        dynamic jsonObj = JObject.Parse(json);
        var keys = key.Split(':');
        dynamic tempObj = jsonObj;

        for (int i = 0; i < keys.Length - 1; i++)
        {
            if (tempObj[keys[i]] == null)
            {
                throw new Exception("Key does not exist");
            }
            tempObj = tempObj[keys[i]];
        }

        if (tempObj[keys.Last()] == null)
        {
            throw new Exception("Key does not exist");
        }

        tempObj.Remove(keys.Last());
        string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
        File.WriteAllText(filePath, output);
    }

    // Xóa dữ liệu trong một key trong file appsettings.json
    public static void ClearKey(string key)
    {
        AddToKey(key, "");
    }
}