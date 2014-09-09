using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace lex
{
  class Program
  {
    static void Main(string[] args)
    {
      string code = null;

      if(args.Length > 0) {
        code = File.ReadAllText(args[0]);
      } else {

      StringBuilder sb = new StringBuilder();

      string s = null;
      while((s = Console.ReadLine()) != "!E")
        sb.AppendLine(s);

      code = sb.ToString();
      }

      Lexer lexer = new Lexer();

      lexer.Code = code;

      lexer.ScanAll();

      foreach(var token in lexer.Tokens)
        Console.WriteLine(token.ToString());

      Parser parser = new Parser();
      parser.Tokens = lexer.Tokens.ToArray();

      parser.ParseAll();

      ast.VMState state = new ast.VMState();

      foreach (var stmt in parser.Program)
        stmt.Execute(state);

      Console.ReadKey();
    }
  }
}
