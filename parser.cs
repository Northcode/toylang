using System;
using System.Collections.Generic;

namespace lex.ast
{
  public class VMState
  {
    public Stack<object> MainStack {
      get;set;
    }

    public Dictionary<string,object> Locals {
      get;set;
    }

    public void SetLocal(string key, object value) {
      if (!Locals.ContainsKey(key))
        Locals.Add(key,value);
      else
        Locals[key] = value;
    }

    public object GetLocal(string key) {
      if (!Locals.ContainsKey(key))
        throw new Exception("Local {0}, does not exist, please set".f(key));
      else
        return Locals[key];
    }

    public VMState() {
      MainStack = new Stack<object>();
      Locals = new Dictionary<string,object>();
    }
  }

  public abstract class Stmt
  {
    public abstract void Execute(VMState state);
  }

  public abstract class Expr : Stmt
  {
    public override void Execute(VMState state) {
      Eval(state);
    }

    public abstract void Eval(VMState state);
  }

  public abstract class Literal : Expr
  {
    public override void Eval(VMState state) {
      state.MainStack.Push(GetValue());
    }

    public abstract object GetValue();
  }

  public class BaseLit<T> : Literal
  {
    T value;

    public BaseLit(T Value) {
      this.value = Value;
    }

    public override object GetValue() {
      return this.value;
    }
  }

  public class VarRef : Expr
  {
    string key;

    public VarRef(string Key) {
      key = Key;
    }

    public override void Eval(VMState state) {
      state.MainStack.Push(state.GetLocal(key));
    }
  }

  public enum BinOp
  {
    Plus,Min,Mul,Div,Mod
  }

  public class ArithExpr : Expr
  {
    Expr l1;
    BinOp op;
    Expr l2;

    public ArithExpr(Expr A, BinOp Op, Expr B) {
      l1 = A;
      op = Op;
      l2 = B;
    }

    public override void Eval(VMState state) {
      l2.Eval(state);
      l1.Eval(state);
      object a = state.MainStack.Pop();
      object b = state.MainStack.Pop();
      if (a is int && b is int)
      {
        int _a = (int)a;
        int _b = (int)b;
        int r = 0;
        switch (op)
        {
          case BinOp.Plus:
            r = _a + _b;
            break;
          case BinOp.Min:
            r = _a - _b;
            break;
          case BinOp.Mul:
            r = _a * _b;
            break;
          case BinOp.Div:
            r = _a / _b;
            break;
          case BinOp.Mod:
            r = _a % _b;
            break;
        }
        state.MainStack.Push(r);
      }
      else if (a is float && b is float)
      {
        float _a = (float)a;
        float _b = (float)b;
        float r = 0;
        switch (op)
        {
          case BinOp.Plus:
            r = _a + _b;
            break;
          case BinOp.Min:
            r = _a - _b;
            break;
          case BinOp.Mul:
            r = _a * _b;
            break;
          case BinOp.Div:
            r = _a / _b;
            break;
          case BinOp.Mod:
            r = _a % _b;
            break;
        }
        state.MainStack.Push(r);
      }
    }
  }

  public enum BoolOp
  {
    Eq,Ne,Gt,Lt,Ge,Le,And,Or,Not
  }

  public class BoolExpr : Expr
  {
    Expr a;
    BoolOp o;
    Expr b;

    public BoolExpr(Expr A, BoolOp O, Expr B) {
      a = A;
      o = O;
      b = B;
    }

    float ObjToFloat(object o)
    {
      if(o is int)
        return (float)o;
      else if(o is float)
        return (float)o;
      else
        throw new Exception("Cannot convert value to number: {0}".f(o));
    }

    public override void Eval(VMState state) {
      b.Eval(state); a.Eval(state);
      object _a = state.MainStack.Pop();

      if(o == BoolOp.Not) {
        if(_a is bool)
        {
          state.MainStack.Push(!((bool)_a));
          return;
        } else
          throw new Exception("Cannot apply Negation operator on non boolean: {0}".f(_a));
      }

      object _b = state.MainStack.Pop();

      bool r = false;

      if(_a is bool && _b is bool)
      {
        bool a_ = (bool)_a; bool b_ = (bool)_b;
        switch (o) {
          case BoolOp.Eq:
            r = a_ == b_;
            break;
          case BoolOp.Ne:
            r = a_ != b_;
            break;
          case BoolOp.And:
            r = a_ && b_;
            break;
          case BoolOp.Or:
            r = a_ || b_;
            break;
        }
      }
      else
      {
        float a_ = ObjToFloat(_a);
        float b_ = ObjToFloat(_b);

        switch(o) {
          case BoolOp.Eq:
            r = a_ == b_;
            break;
          case BoolOp.Ne:
            r = a_ != b_;
            break;
          case BoolOp.Gt:
            r = a_ > b_;
            break;
          case BoolOp.Lt:
            r = a_ < b_;
            break;
          case BoolOp.Ge:
            r = a_ >= b_;
            break;
          case BoolOp.Le:
            r = a_ <= b_;
            break;
        }
      }

      state.MainStack.Push(r);
    }
  }

  public class Print : Stmt
  {
    public Print() {
    }

    public override void Execute(VMState state) {
      Console.Write(state.MainStack.Peek());
    }
  }

  public class Dup : Stmt
  {
    public override void Execute(VMState state) {
      object o = state.MainStack.Pop();
      state.MainStack.Push(o);
      state.MainStack.Push(o);
    }
  }

  public class Push : Stmt
  {
    Expr value;

    public Push(Expr Value) {
      value = Value;
    }

    public override void Execute(VMState state) {
      value.Eval(state);
    }
  }

  public class Pop : Stmt
  {
    public override void Execute(VMState state) {
      state.MainStack.Pop();
    }
  }

  public class Read : Stmt
  {
    public override void Execute(VMState state) {
      state.MainStack.Push(Console.ReadLine());
    }
  }

  public class CInt : Stmt
  {
    public override void Execute(VMState state) {
      object o = state.MainStack.Pop();
      state.MainStack.Push(Convert.ToInt32(o));
    }
  }

  public class CFloat : Stmt
  {
    public override void Execute(VMState state) {
      object o = state.MainStack.Pop();
      state.MainStack.Push((float)Convert.ToDecimal(o));
    }
  }

  public class CStr : Stmt
  {
    public override void Execute(VMState state) {
      object o = state.MainStack.Pop();
      state.MainStack.Push(o.ToString());
    }
  }

  public class CBool : Stmt
  {
    public override void Execute(VMState state) {
      object o = state.MainStack.Pop();
      state.MainStack.Push(Convert.ToBoolean(o));
    }
  }

  public class StLoc : Stmt
  {
    string key;

    public StLoc(string Key) {
      key = Key;
    }

    public override void Execute(VMState state) {
      state.SetLocal(key,state.MainStack.Pop());
    }
  }

  public class LdLoc : Stmt
  {
    string key;

    public LdLoc(string Key) {
      key = Key;
    }

    public override void Execute(VMState state) {
      state.MainStack.Push(state.GetLocal(key));
    }
  }

  public class RmLoc : Stmt
  {
    string key;

    public RmLoc(string Key) {
      key = Key;
    }

    public override void Execute(VMState state) {
      state.Locals.Remove(key);
    }
  }
}

namespace lex
{
  class Parser
  {
    public Token[] Tokens {
      get;set;
    }

    public Queue<ast.Stmt> Program {
      get;set;
    }

    int index;

    Token CurrentToken {
      get { return Tokens[index]; }
    }

    bool EOT {
      get { return index >= Tokens.Length; }
    }

    void Next() {index++;}
    void Prev() {index--;}

    public void ParseAll() {
      index = 0;
      Program = new Queue<ast.Stmt>();
      if(Tokens == null || Tokens.Length == 0) {
        throw new Exception("Cannot parse nothing...");
      }

      while (!EOT)
      {
        Program.Enqueue(ParseStmt());
      }
    }

    public void Expect(TokenType t)
    {
      if(CurrentToken.type != t)
        throw new Exception("Unexpected {0}, expected {1}".f(CurrentToken,t.ToString()));
    }

    public bool IsType(TokenType t) {
      return CurrentToken.type == t;
    }

    ast.Stmt ParseStmt()
    {
        Console.WriteLine("parsing: {0}".f(CurrentToken.ToString()));
        if(IsWord()) {
          return ParseWord();
        }
        else
          return ParseExpr();
    }

    ast.Stmt ParseWord()
    {
      string word = CurrentToken.value as string;
      if(word == "print") {
    		Next ();
        return new ast.Print();
      } else if (word == "push") {
        Next();
        return new ast.Push(ParseExpr());
      } else if (word == "pop") {
        Next ();
        return new ast.Pop();
      } else if (word == "cint") {
        Next ();
        return new ast.CInt();
      } else if (word == "cfloat") {
        Next ();
        return new ast.CFloat();
      } else if (word == "cbool") {
        Next ();
        return new ast.CBool();
      } else if (word == "cstr") {
        Next ();
        return new ast.CStr();
      } else if (word == "read") {
        Next ();
        return new ast.Read();
      } else if (word == "dup") {
        Next ();
        return new ast.Dup();
      } else if (word == "stloc") {
        Next();
        Expect(TokenType.Word);
        string key = CurrentToken.value as string;
        Next ();
        return new ast.StLoc(key);
      } else if (word == "ldloc") {
        Next();
        Expect(TokenType.Word);
        string key = CurrentToken.value as string;
        Next ();
        return new ast.LdLoc(key);
      } else if (word == "rmloc") {
        Next();
        Expect(TokenType.Word);
        string key = CurrentToken.value as string;
        Next ();
        return new ast.RmLoc(key);
      } else {
        throw new Exception("Unrecognized keyword: {0} at line {1}".f(word,CurrentToken.line));
      }
    }

    ast.Expr ParseExpr(bool isfac = false)
    {
      ast.Expr e = null;

      if(IsLit())
        e = ParseLit();
      else if (IsSymbol())
        e = ParseSymbol();
      else if (IsWord())
        e = new ast.VarRef(CurrentToken.value as string);
      else
        throw new Exception("Unexpected token: {0}".f(CurrentToken.ToString()));

      Next();
      if(EOT)
        return e;

      if(IsSymbol())
        return ParseArith(e,isfac);
      else
        return e;
    }

    ast.Expr ParseArith(ast.Expr first, bool isfac = false) {
      if (IsSymbol() && GetChar() == '*') {
        Next();
        return new ast.ArithExpr(first, ast.BinOp.Mul, ParseExpr(true));
      } else if (IsSymbol() && GetChar() == '/') {
        Next();
        return new ast.ArithExpr(first, ast.BinOp.Div, ParseExpr(true));
      } else if (IsSymbol() && GetChar() == '%') {
        Next();
        return new ast.ArithExpr(first, ast.BinOp.Mod, ParseExpr(true));
      } else if (IsSymbol() && GetChar() == '+' && !isfac) {
        Next();
        return new ast.ArithExpr(first, ast.BinOp.Plus, ParseExpr(true));
      } else if (IsSymbol() && GetChar() == '-' && !isfac) {
        Next();
        return new ast.ArithExpr(first, ast.BinOp.Min, ParseExpr(true));
      } else if (IsSymbol() && GetChar() == '=') {
        Next();
        if(IsSymbol() && GetChar() == '=') {
          Next();
          return new ast.BoolExpr(first, ast.BoolOp.Eq, ParseExpr());
        } else
          throw new Exception("WHAT THE HELL BRO!? {0}".f(Tokens[index + 1]));
      } else if (IsSymbol() && GetChar() == '<') {
        Next();
        if(IsSymbol() && GetChar() == '=') {
          Next();
          return new ast.BoolExpr(first,ast.BoolOp.Le,ParseExpr());
        } else
          return new ast.BoolExpr(first,ast.BoolOp.Lt,ParseExpr());
      } else if (IsSymbol() && GetChar() == '>') {
        Next();
        if(IsSymbol() && GetChar() == '=') {
          Next();
          return new ast.BoolExpr(first,ast.BoolOp.Ge,ParseExpr());
        } else
          return new ast.BoolExpr(first,ast.BoolOp.Gt,ParseExpr());
      } else if (IsSymbol() && GetChar() == '!') {
        Next();
        if(IsSymbol() && GetChar() == '=') {
          Next();
          return new ast.BoolExpr(first,ast.BoolOp.Ne,ParseExpr());
        } else
          throw new Exception("Unexpected {0}, expected '='".f(CurrentToken));
      }
      else
        throw new Exception("This is not an arithmetic expressing dumbass {0}".f(CurrentToken));
    }

    ast.Literal ParseLit()
    {
      if(CurrentToken.type == TokenType.Number) {
        return new ast.BaseLit<float>((float)CurrentToken.value);
      } else if (CurrentToken.type == TokenType.Str) {
        return new ast.BaseLit<string>(CurrentToken.value as string);
      } else if (CurrentToken.type == TokenType.Boolean) {
        return new ast.BaseLit<bool>((bool)CurrentToken.value);
      } else {
        throw new Exception("Someone broke logic, atempt to fix universe...");
      }
    }

    ast.Expr ParseSymbol() {
      if(GetChar() == '-') {
        Next();
        return new ast.ArithExpr(new ast.BaseLit<float>(0),ast.BinOp.Min,ParseExpr());
      } else if (GetChar() == '!') {
        Next();
        return new ast.BoolExpr(ParseExpr(),ast.BoolOp.Not,null);
      } else {
        throw new Exception("Unexpected symbol {0}".f(CurrentToken));
      }
    }

    char GetChar() {
      return (char)CurrentToken.value;
    }

    bool IsWord() {
      return CurrentToken.type == TokenType.Word;
    }

    bool IsSymbol() {
      return CurrentToken.type == TokenType.Symbol;
    }

    bool IsLit() {
      return CurrentToken.type == TokenType.Str || CurrentToken.type == TokenType.Number || CurrentToken.type == TokenType.Boolean;
    }
  }
}
