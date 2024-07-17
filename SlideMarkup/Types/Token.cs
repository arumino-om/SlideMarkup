using System.Text;

namespace SlideMarkup.Types;

public enum TokenType
{
    // SlideMarker構文
    Command,
    MacroDefinition,
    PageSeparator,
    SectionTitle,
    
    // Markdown構文
    Section2,
    Section3,
    Section4,
    Section5,
    Section6,
    BoldMarker,
    ItalicMarker,
    Quotes,
    NextLine,
    Text,
    EOF
}

public class Token
{
    private TokenType Type { get; }
    private string Value { get; }
    private string[]? Attributes { get; }
    
    public Token(TokenType type, string value, string[]? attributes = null)
    {
        Type = type;
        Value = value;
        Attributes = attributes;
    }
    
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"Type: {Type}");
        if (!string.IsNullOrEmpty(Value)) sb.Append(", Value: " + Value);
        if (Attributes != null) sb.Append(", Attributes: [" + string.Join(", ", Attributes) + "]");
        return sb.ToString();
    }
}