using System.Text;
using Markdig.Helpers;
using SlideMarkup.Types;

namespace SlideMarkup;

public class Lexer
{
    private string Input;
    private int Position;
    
    public Lexer(string input)
    {
        Input = input;
        Position = 0;
    }

    private char GetNextChar()
    {
        if (Input.Length > Position)
        {
            return Input[Position++];
        }
        else
        {
            return '\0';
        }
    }

    public List<Token> Lex()
    {
        var tokens = new List<Token>();
        char? pc = null;
        char? cc = null;
        StringBuilder buffer = new();

        var AddTextToken = (() =>
        {
            if (buffer.Length <= 0) return;
            tokens.Add(new Token(TokenType.Text, buffer.ToString()));
            buffer.Clear();
        });
        
        while (cc != '\0')
        {
            pc = cc;
            cc = GetNextChar();

            // 前の文字がエスケープ文字なら，強制的に現在の文字をバッファに追加
            if (pc == '\\')
            {
                buffer.Append(cc);
                continue;
            }
            
            // 現在の文字がEOFなら，バッファに残っている文字をトークンに追加して終了
            if (cc == '\0')
            {
                AddTextToken();
                tokens.Add(new Token(TokenType.EOF, ""));
                break;
            }

            //------ ブロックコマンド ------
            // 変数定義
            if (IsFirstLine(pc) && cc == '@')
            {
                AddTextToken();
                var name = new StringBuilder();
                var value = new StringBuilder();
                cc = GetNextChar();
                do
                {
                    name.Append(cc);
                    cc = GetNextChar();
                } while (cc != '[');

                cc = GetNextChar();
                do
                {
                    value.Append(cc);
                    cc = GetNextChar();
                } while (cc != ']');
                tokens.Add(new Token(TokenType.MacroDefinition, name.ToString(), [value.ToString()]));
            }
            // セクションタイトル
            else if (IsFirstLine(pc) && cc == '#')
            {
                AddTextToken();
                while (cc != '\n' && cc != '\0')
                {
                    cc = GetNextChar();
                    buffer.Append(cc);
                }
                tokens.Add(new Token(TokenType.SectionTitle, buffer.ToString().Trim()));
                buffer.Clear();
            }
            // ページ区切り
            else if (IsFirstLine(pc) && cc == '-')
            {
                AddTextToken();
                var count = 1;
                while ((cc = GetNextChar()) == '-') count++;
                if (count != 3) continue; 
                tokens.Add(new Token(TokenType.PageSeparator, ""));
            }
            
            //------ インラインコマンド ------
            // コマンド
            else if (cc == ';')
            {
                AddTextToken();
                var commandBuilder = new StringBuilder();
                do
                {
                    cc = GetNextChar();
                    if (IsCommandUsableChar((char)cc))
                    {
                        commandBuilder.Append((char)cc);
                    }
                } while (IsCommandUsableChar((char)cc));

                Position--;
                tokens.Add(new Token(TokenType.Command, commandBuilder.ToString()));
            }
            // 改行
            else if (cc == '\n')
            {
                AddTextToken();
                tokens.Add(new Token(TokenType.NextLine, ""));
            }
            // その他
            else
            {
                buffer.Append(cc);
            }
        }

        return tokens;
    }
    
    private bool IsCommandUsableChar(char c)
    {
        return c == '_' || c == '-' || c is >= '0' and <= '9' || c is >= 'a' and <= 'z' || c is >= 'A' and <= 'Z';
    }

    private bool IsFirstLine(char? c)
    {
        return c is null or '\n';
    }
}