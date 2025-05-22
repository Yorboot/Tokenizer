using System.Text.Json;
using System.Xml.Linq;

namespace nativ;

public class Json
{
    public static List<string> Keywords = new List<string>();
    public static List<string> Identifiers = new List<string>();
    public static List<string> PublicityIdentifiers = new List<string>();
    public static List<List<string>> ReadLanguageData()
    {
        string jsonString = GetJsonFile();
        // Parse JSON
        using var doc = JsonDocument.Parse(jsonString);
        var root = doc.RootElement;

        // Extract each list individually

        foreach (var item in root.GetProperty("keywords").EnumerateArray())
        {
            Keywords.Add(item.GetString());
        }
        
        foreach (var item in root.GetProperty("identifiers").EnumerateArray())
        {
            Identifiers.Add(item.GetString());
        }

        var publicityIdentifiers = new List<string>();
        foreach (var item in root.GetProperty("publicity-identifiers").EnumerateArray())
        {
            PublicityIdentifiers.Add(item.GetString());
        }
        
        return new List<List<string>>()
        {
            Keywords,
            Identifiers,
            PublicityIdentifiers
        };
    }
    

    private static string GetJsonFile()
    {
        try
        {
            return File.ReadAllText(@"./c#.json");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static void WriteJson(List<List<TokenTypes>> tokenTypes, List<string> code)
    {
        List<List<string>> json = new List<List<string>>();
        foreach (var list in tokenTypes)
        {
            List<string> temp = new List<string>();
            foreach (var tokenType in list)
            {
                temp.Add(tokenType.ToString());
            }
            json.Add(temp);
        }
        var data = new
        {
            Tokens = json,
            Code = code
        };
        string jsonString = JsonSerializer.Serialize(data);
        File.WriteAllText(@"./data.json", jsonString);
        Console.WriteLine("Sucessfuly writed to json");
    }
}