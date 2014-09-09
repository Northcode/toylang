using System;
using System.Text;

namespace lex
{
  public static class Extentions
  {
    public static string f(this string that, params object[] args)
    {
      return String.Format(that,args);
    }
  }
}
