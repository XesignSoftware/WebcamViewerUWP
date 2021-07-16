using System;
using System.Runtime.CompilerServices;

public static class StringExtensions
{
    // Args parser:
    public static string Parse(this string text, params object[] args) => ParseArgs(text, args);
    public static string ParseArgs(this string text, params object[] args)
    {
        string s = "";

        int c = 0, arg_i = 0;
        for (; c < text.Length; ++c)
        {
            if (text[c] == '%')
            {
                if (c - 1 > 0 && text[c - 1] == '\\') continue;

                // TODO!: This does not work for some reason!
                if (args == null || args.Length == 0) Console.WriteLine("String includes argument placeholder(s), but no args were specified!".M());

                if (arg_i >= args.Length) Console.WriteLine($"There was no argument at {arg_i} - total count: {args.Length}", "Logger");
                else
                {
                    s += args[arg_i++];
                    continue;
                }
            }

            s += text[c];
        }

        return s;
    }

    //public static string Type(this string text, object type = null)
    public static string T(this string text, object type = null, bool standalone = true)
    {
        if (type != null)
            return $"{(type.GetType() == typeof(string) ? type : type.GetType().Name)}{(standalone ? ": " : "")}" + text;
        else return text;
    }

    //public static string Method(this string text, [CallerMemberName] string methodName = null)
    public static string M(this string text, [CallerMemberName] string methodName = null)
    {
        if (methodName != null)
            return $"{methodName}(): " + text;
        else return text;
    }

    //public static string TypeMethod(this string text, object type = null, [CallerMemberName] string methodName = null)
    public static string TM(this string text, object type = null, [CallerMemberName] string methodName = null)
    {
        if (type != null && (methodName == null || methodName == ""))
            return text.T(type);
        else if (type == null && (methodName != null && methodName != ""))
            return text.M(methodName);
        else if (type != null && (methodName != null && methodName != ""))
            return $"{T("", type, false)}/{methodName}():" + text;
        else
        { Console.WriteLine("WTF", "StringExts", "TypeMethod"); return text; }
    }

    // TODO: We should also consider spaces as empty, potentially. Perhaps controllable?
    public static bool IsEmpty(this string text) => (text == null || text == "");
}

public static class CharExtensions
{
    public static bool IsWhitespace(this char c) => (c == ' ' || c == '\r' || c == '\n' || c == '\t');
    public static bool IsNewline(this char c) => (c == '\r' || c == '\n');
}