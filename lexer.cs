using System;
using System.Collections.Generic;
using System.Text;

namespace lex
{
  enum TokenType
  {
    Word,
    Str,
    Number,
    Symbol,
    Boolean
  }

  class Token
  {
    public object value;
    public TokenType type;
    public int line;

    public override string ToString()
    {
      return "( type {0} at line {1} value: {2} )".f(type.ToString(),line.ToString(),value.ToString());
    }
  }

  class Lexer
  {
    public Queue<Token> Tokens
    {
      get;set;
    }

    public string Code{
      get;set;
    }

    int index;
    int linecount;

    public char CurrentChar {
      get {
        return Code[index];
      }
    }

    public Lexer() {
      Tokens = new Queue<Token>();
    }

    void Next()
    {
      index++;
    }

    void Prev()
    {
      index--;
    }

    public void ScanAll()
    {
      index = 0;

      if(Code == "" || Code == null)
        throw new Exception("Cannot parse nothing...");

      linecount = 0;

      while(index < Code.Length)
      {
        if(IsDigit())
          ScanNumber();
        else if (IsQuote())
          ScanString();
        else if (IsChar())
          ScanWord();
        else if (IsNewLine())
          linecount++;
        else if(IsWhitespace())
          {}//Skip whitespace
        else
          ScanSymbol();

        Next();
      }
    }

    public void ScanString()
    {
      if(!IsQuote())
        throw new Exception("String does not start with '\"' , current index: {0}".f(index));

      Next(); //skip '"'

      StringBuilder sb = new StringBuilder();

      bool escaped = false;

      while(!IsQuote() || escaped){
        if(escaped && CurrentChar == 'n') {
          sb.Append('\n');
          Next();
          escaped = false;
          continue;
        }
        escaped = false;
        if(IsEscape())
          escaped = true;
        else
          sb.Append(CurrentChar);
        Next();
        CheckForEOF("string");
      }

      Next(); //skip ending '"'

      Token stringtoken = new Token();
      stringtoken.type = TokenType.Str;
      stringtoken.value = sb.ToString();
      stringtoken.line = linecount;

      Tokens.Enqueue(stringtoken);
    }

    public void ScanNumber()
    {
      if(!IsDigit())
        throw new Exception("Unexpected character in Number token, index: {0}".f(index));

      StringBuilder sb = new StringBuilder();

      while(IsDigit()) {
        sb.Append(CurrentChar);
        Next();
      }

      //check for decimal
      if(IsDecimal())
      {
        Next(); //skip decimal point
        sb.Append(',');
        while(IsDigit()) {
          sb.Append(CurrentChar);
          Next();
        }
      }

      Token numtoken = new Token();
      numtoken.type = TokenType.Number;
      numtoken.value = (float)Convert.ToDecimal(sb.ToString());
      numtoken.line = linecount;

      Tokens.Enqueue(numtoken);
    }

    public void ScanWord()
    {
      if(!IsChar())
        throw new Exception("First character in Word must be char, index: {0}".f(index));

      StringBuilder sb = new StringBuilder();

      while(IsChar() || IsDigit() || IsSeparator()){
        sb.Append(CurrentChar);
        Next();
      }

      Prev();

      string word = sb.ToString();

      if(word == "true" || word == "false") {
        Token booltoken = new Token();
        booltoken.type = TokenType.Boolean;
        booltoken.value = word == "true";
        booltoken.line = linecount;
        Tokens.Enqueue(booltoken);
      } else {
        Token wordtoken = new Token();
        wordtoken.type = TokenType.Word;
        wordtoken.value = sb.ToString();
        wordtoken.line = linecount;
        Tokens.Enqueue(wordtoken);
      }
    }

    public void ScanSymbol()
    {
      Token symtoken = new Token();
      symtoken.type = TokenType.Symbol;
      symtoken.value = CurrentChar;
      symtoken.line = linecount;
      Tokens.Enqueue(symtoken);
    }

    public bool IsChar()
    {
      return char.IsLetter(CurrentChar);
    }

    public bool IsDigit()
    {
      return char.IsDigit(CurrentChar);
    }

    public bool IsQuote()
    {
      return CurrentChar == '"' || CurrentChar == '\'';
    }

    public bool IsDecimal()
    {
      return CurrentChar == '.' || CurrentChar == ',';
    }

    public bool IsSeparator()
    {
      return CurrentChar == '_' || CurrentChar == '-';
    }

    public bool IsWhitespace()
    {
      return CurrentChar == ' ' || CurrentChar == '\t' || CurrentChar == '\n' || CurrentChar == '\r';
    }

    public bool IsNewLine()
    {
      return CurrentChar == '\n';
    }

    public bool IsEscape()
    {
      return CurrentChar == '\\';
    }

    public void CheckForEOF(string s)
    {
      if (index >= Code.Length)
        throw new Exception("Unexpected EOF in {0}, index: {1}".f(s,index));
    }
  }
}
