using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

namespace FPOSDB.Extensions
{
    public static class StringExtensions
    {
        public static string ToLiteral(this string input)
        {
            return Microsoft.CodeAnalysis.CSharp.SymbolDisplay.FormatLiteral(input, false);
        }
        public static string ReplaceEscapedCharactors(this string input)
        {
            return input.Replace("\\n", "\n").Replace("\\r", "\r");
        }
    }
}
