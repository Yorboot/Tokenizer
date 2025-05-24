using System.Text.RegularExpressions;

namespace nativ;

public enum TokenTypes
{
    Keyword,
    Identifier,
    Integers,
    Operator,
    String,
    Comment,
    Whitespace,
    Unknown,
    Floats,
    Getter,
    Setter,
    Char,
    Boolean,
    NotSet,
    PublicityIdentifiers,
}
public class Token
{
    public TokenTypes TokenType { get; }
    public string Value { get; }

    public Token(TokenTypes tokenType, string value)
    {
        this.TokenType = tokenType;
        this.Value = value;
    }
}
public class Tokenizer
{
    public static List<List<string>> Tokens = Json.ReadLanguageData();
    private static readonly List<string> Keywords = Tokens[0];
    private static readonly List<string> Identifiers = Tokens[1];
    private static readonly List<string> PublicityIndentifiers = Tokens[2];

    private static readonly Regex TokenRegex = new Regex(
        @"
        (\b\d+\b)|              # Matches integers (BigInteger)
        (\b(?:true|false)\b)|   # Matches Boolean values
        (\bchar\b)|             # Matches 'char' keyword
        (\b(?:get|set)\b)|      # Matches 'getter' and 'setter'
        (\b(?:decimal|double|BigInteger)\b)|  # Matches specific data types
        (\b\w+\b)|              # Matches identifiers/keywords
        ([+\-*/=<>])|           # Matches operators
        ([""'].*?[""'])|        # Matches strings
        (//.*)|                 # Matches comments
        (\s+)",                 
        RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
    public List<Token> Tokenize(string input)
    {
        var tokens = new List<Token>();
        var matches = TokenRegex.Matches(input);

        foreach (Match match in matches)
        {
            string value = match.Value;
            TokenTypes type = TokenTypes.Unknown;
            if (Keywords.Contains(value))
            {
                type = TokenTypes.Keyword;
                tokens.Add(new Token(type,value));
            }

            if (Identifiers.Contains(value))
            {
                type = TokenTypes.Identifier;
                tokens.Add(new Token(type,value));
            }

            if (PublicityIndentifiers.Contains(value))
            {
                type = TokenTypes.PublicityIdentifiers;
                tokens.Add(new Token(type,value));
            }
            else
            {
                switch (value)
                {
                    case var v when Regex.IsMatch(v, @"^[+\-*/=<>]$"):
                        type = TokenTypes.Operator;
                        break;

                    case var v when Regex.IsMatch(v, @"^[""'].*[""']$"):
                        type = TokenTypes.String;
                        break;

                    case var v when Regex.IsMatch(v, @"^//.*$"):
                        type = TokenTypes.Comment;
                        break;

                    case var v when Regex.IsMatch(v, @"^\s+$"):
                        type = TokenTypes.Whitespace;
                        break;

                    case var v when Regex.IsMatch(v, @"(\b\d+\b)"):
                        type = TokenTypes.Integers;
                        break;
                    case var v when Regex.IsMatch(v, @":decimal|double"):
                        type = TokenTypes.Floats;
                        break;
                    case var v when Regex.IsMatch(v, @":true|false"):
                        type = TokenTypes.Boolean;
                        break;
                    case var v when Regex.IsMatch(v, @"\b function\s+get[A-Z]\w*\s*\("):
                        type = TokenTypes.Getter;
                        break;

                    case var v when Regex.IsMatch(v, @"\b function\s+set[A-Z]\w*\s*\("):
                        type = TokenTypes.Setter;
                        break;
                    default:
                        type = TokenTypes.Unknown;
                        break;
                }
                tokens.Add(new Token(type,value));
            }
        }

        return tokens;
    }
        
}
class Program
{
    public static void Main()
    {
        var tokenizer = new Tokenizer();
        string code = "case if else switch private public internal class";
        List<Token> tokens = tokenizer.Tokenize(code);
        List<TokenTypes> keywords = new List<TokenTypes>();
        List<TokenTypes> identifiers = new List<TokenTypes>();
        List<TokenTypes> publicityIdentifiers = new List<TokenTypes>();
        foreach (var token in tokens)
        {
            switch (token.TokenType)
            {
                case TokenTypes.Identifier:
                    identifiers.Add(token.TokenType);
                    break;
                case TokenTypes.Keyword:
                    keywords.Add(token.TokenType);
                    break;
                case TokenTypes.PublicityIdentifiers:
                    publicityIdentifiers.Add(token.TokenType);
                    break;
            }
            // Console.WriteLine($"[{token.TokenType}] {token.Value}");
        }
        
        foreach (var tokenType in keywords)
        {
            Console.WriteLine(tokenType);
        }

        foreach (var token in identifiers)
        {
            Console.WriteLine(token);
        }

        foreach (var tokenType in publicityIdentifiers)
        {
            Console.WriteLine(tokenType);
        }

        List<List<TokenTypes>> list = new List<List<TokenTypes>>
        {
            keywords,
            identifiers,
            publicityIdentifiers
        };
        List<string> Code = new List<string>(code.Split(' ')) ;
        Json.WriteJson(list,Code);
    }
}
    