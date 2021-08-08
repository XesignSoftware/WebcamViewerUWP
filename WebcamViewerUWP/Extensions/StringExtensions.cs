#define PARSE_USE_STRINGBUILDER
//#undef PARSE_USE_STRINGBUILDER

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

public static class StringExtensions
{
    #region Parsing
    public const char PARSE_PLACEHOLDER_CHAR = '%';
    public const bool PARSE_CHECK_PLACEHOLDER_COUNT = true;
    public const int PARSE_PARAMS_BUFFER = 4096;

    // Parse:
    /// <summary>Parses a string, replacing GLOBAL_ </summary>
    public static string Parse(this string text, params object[] args) => Parse(text, PARSE_PLACEHOLDER_CHAR, args);
    public static string Parse(this string text, char placeholder_char, params object[] args)
    {
        // Count the amount of placeholders:
        if (PARSE_CHECK_PLACEHOLDER_COUNT)
        {
            int placeholder_count = text.Count(c => c == placeholder_char);
            if (placeholder_count != args.Length) throw new Exception("Placeholder count and argument count do not match!" + $"  P: {placeholder_count} A: {args.Length}");
        }

#if PARSE_USE_STRINGBUILDER
        StringBuilder builder = new StringBuilder(text.Length + PARSE_PARAMS_BUFFER); // TODO: revise!
        int arg_i = -1;
        for (int i = 0; i < text.Length; ++i)
        {
            char c = text[i];

            if (c == placeholder_char)
                builder.Append(args[++arg_i]);
            else
                builder.Append(c);
        }

        return builder.ToString();
#else
        string s = "";
        int arg_i = -1;
        for (int i = 0; i < text.Length; ++i)
        {
            char c = text[i];

            if (c == placeholder_char)
                s += (args[++arg_i]);
            else
                s += c;
        }

        return s;
#endif

    }
    #endregion

    #region Formatting

    // Max lines:
    public static string MaxLines(this string text, int maxLines)
    {
        string[] lines = text.Split('\n');
        int lineCount = lines.Length - 1;
        if (lineCount > maxLines)
        {
            int lineDiff = Math.Abs(maxLines - lineCount);
            string[] newLines = new string[maxLines + 1]; // newline at end!
            for (int i = lineDiff; i < lineCount; i++) // Remove lines from start to keep max line count
                newLines[i - lineDiff] = lines[i];

            text = string.Join("\n", newLines);
        }
        return text;
    }

    // Monoscape:
    // Trebuchet MS: ~8.5
    public static string Monoscape(this string text, float width = 9.5f) =>
        $"<mspace={width}>{text}</mspace>";

    // Align a text by number of spaces.
    // Note! For best results, text should be monospace!
    public static string AlignSpaces(this string text, int total_text_length, int max)
    {
        int space_count = max - total_text_length;
        string s = "";
        for (int i = 0; i < space_count; i++)
            s += ' ';

        return s + text;

    }

    public static string AlignSpaces_New(this string text, int left_length, int right_target, bool after = false)
    {
        int space_count = right_target - text.Length;
        string s = "";
        for (; space_count > 0; --space_count) s += ' ';

        if (!after)
            return s + text;
        else
            return text + s;
    }
    #endregion

    #region Datatype-parsing
    public static bool ParseBool(this string text)
    {
        // Try parsing as boolean:
        bool bool_result = false;
        if (bool.TryParse(text, out bool_result)) return bool_result;

        // Try parsing as integer: 
        int int_result = -1;
        if (int.TryParse(text, out int_result)) return (int_result == 1);

        // Try parsing common bool-like strings:
        string[] s_true = { "enabled", "yes", "y", "t" };
        string[] s_false = { "disabled", "no", "n", "f" };

        if (s_true.Contains(text.ToLower())) return true;
        else if (s_false.Contains(text.ToLower())) return false;

        // Logger.LogWarning("could not parse the string '%' to boolean. Returning false.".T("StringExts"), text);
        return false;
    }
    public static int ParseInt(this string text) => int.Parse(text);
    public static float ParseFloat(this string text) => float.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);

    public static int[] ParseIntArray(this string text)
    {
        int cursor = 0;
        List<int> list = new List<int>();
        string temp_buffer = "";
        while (cursor < text.Length)
        {
            char c = text[cursor];
            if (char.IsDigit(c)) temp_buffer += c;

            if (c == ',' || c == ')' && temp_buffer != "")
            {
                list.Add(temp_buffer.ParseInt());
                temp_buffer = "";
            }

            ++cursor;
        }
        return list.ToArray();
    }

    public static float[] ParseFloatArray(this string text)
    {
        int cursor = 0;
        List<float> list = new List<float>();
        string temp_buffer = "";
        while (cursor < text.Length)
        {
            char c = text[cursor];
            if (char.IsDigit(c) || c == '.') temp_buffer += c;

            if (c == ',' || c == ')' && temp_buffer != "")
            {
                list.Add(temp_buffer.ParseFloat());
                temp_buffer = "";
            }

            ++cursor;
        }
        return list.ToArray();
    }
    #endregion

    // Type-method:
    public static string M(this string text, [CallerMemberName] string method_name = null) =>
        method_name + (!method_name.IsEmpty() ? "(): " : "") + text;
    public static string T(this string text, object type = null) =>
        type is null ? "" : ((type is string) ? type.ToString() : type.GetType().Name) + ": " + text;
    public static string TM(this string text, object type = null, [CallerMemberName] string method_name = null)
    {
        string s_type = type is null ? "" : ((type is string) ? type.ToString() : type.GetType().Name);
        string s_method = method_name.IsEmpty() ? ": " : ("/" + method_name + "(): ");
        return s_type + s_method + text;
    }

    public static string Reverse(this string text)
    {
        StringBuilder s = new StringBuilder(text); // TODO: probably shouldn't copy the text here.
        for (int i = 0, x = text.Length - 1; i < text.Length; ++i, --x)
            s[i] = text[x];

        return s.ToString();
    }

    public static string RemoveExt(this string text, string ext = null)
    {
        if (ext != null && ext != "") return text.Replace(ext, "");
        else if (text.Contains("."))
        {
            for (int i = text.Length - 1; i > 0; --i)
            {
                if (text[i] == '.')
                    return text.Substring(0, i);
            }
        }
        return text;
    }

    public static bool BeginsWith(this string text, string test, bool ignore_case = true) => text.StartsWith(test, ignore_case, CultureInfo.CurrentCulture);
    public static bool BeginsWith(string test, bool ignore_case, bool opmode_all = false, params string[] text)
    {
        foreach (string t in text)
        {
            bool value = t.BeginsWith(test, ignore_case);
            if (opmode_all && !value) return false;
            else if (!opmode_all && value) return true;
        }
        return false;
    }

    public static bool IsEmpty(this string text) => (text == null || text == "");
}

public static class CharExtensions
{
    public static bool IsWhitespace(this char c) => (c == ' ' || c == '\r' || c == '\n' || c == '\t');
    public static bool IsNewline(this char c) => (c == '\r' || c == '\n');
}