namespace SlideMarkup;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: SlideMarkup <input file>");
            return;
        }
        
        var input = File.ReadAllText(args[0]).Replace("\r", "");
        var lexer = new Lexer(input);

        var tokens = lexer.Lex();
        if (args.Contains("--lex-only"))
        {
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }

            return;
        }
    }
}