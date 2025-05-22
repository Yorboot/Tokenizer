namespace nativ;

internal class LanguageData(string language, List<string> keywords, List<string> identifiers, List<string> publicityIdentifiers)
{
    public string Language { set; get; } = language;
    public List<string> Keywords { set; get; } = keywords;
    public List<string> Identifiers { set; get;} = identifiers;
    public List<string> PublicityIdentifiers { set; get; } = publicityIdentifiers;
}