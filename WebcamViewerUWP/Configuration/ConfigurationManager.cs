using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

public class ConfigurationData
{
    public bool is_error;
    public string error;

    public string file_name;
    public string config_name;
    public Dictionary<string, Dictionary<string, string>> config_sections = new Dictionary<string, Dictionary<string, string>>();

    /// Sections:
    public void AddSection(string name)
    {
        if (!name.IsEmpty() && !config_sections.ContainsKey(name))
            config_sections.Add(name, new Dictionary<string, string>());
    }

    /// Variables:
    public static bool config_auto_section_creation = true;
    public bool AddVariable(string name, string value) => AddVariable(null, name, value);
    public bool AddVariable(string section, string name, string value)
    {
        // Default to global section if no section was given.
        if (section == null) section = ConfigurationManager.SECTION_GLOBAL;

        // Check whether section exists:
        if (!config_sections.ContainsKey(section))
        {
            if (!config_auto_section_creation)
            {
                //LogConsoleW("Config variable '%' requested section '%', but it did not exist. Policy does not allow automatic section creation. ('%')", name, section, nameof(config_auto_section_creation));
                return false;
            }

            //LogConsoleW("Config variable '%' requested section '%', but it did not exist. Section was automatically created due to policy ('%').", name, section, nameof(config_auto_section_creation));
            AddSection(section);
        }

        // Add variable to the appropriate section:
        config_sections[section].Add(name, value);

        return true;
    }

    public string GetVariable(string name)
    {
        // TODO: simplify!
        foreach (var section in config_sections)
        {
            foreach (var entry in section.Value)
            {
                if (entry.Key == name)
                    return entry.Value;
            }
        }

        //LogW("Could not find variable '%' ('%')".T(this), name, config_name);
        return null;
    }
    public string GetVariable(string section, string name) => config_sections[section][name];
}

public class ConfigurationManager
{
    public const string SECTION_GLOBAL = "Global";
    StorageFolder local_folder = ApplicationData.Current.LocalFolder;

    public async Task<ConfigurationData> CreateConfigFile(string file_name, bool force_write = false) => await CreateConfigFile(file_name, null, force_write);
    public async Task<ConfigurationData> CreateConfigFile(string file_name, string config_name = null, bool force_write = false)
    {
        if (config_name == null) config_name = file_name;

        StorageFile file = null;

        try { file = await local_folder.GetFileAsync(file_name); }
        catch { }

        if (file != null && !force_write) return new ConfigurationData() { is_error = true, error = "File already exists!" };
        else
        {
            // The file should exist now.
            try { file = await local_folder.CreateFileAsync(file_name); }
            catch { }
        }

        ConfigurationData config_data = new ConfigurationData() { file_name = file_name, config_name = config_name };

        // Write to file:
        {
            string text = "#name %\n".Parse(config_name) +
                          ": Testing\ntest1 1\ntest2 2\ntest3 3";
            string[] lines = text.Split('\n');
            await FileIO.WriteLinesAsync(file, lines);
        }

        //ContentDialog dialog = new ContentDialog() { Content = "local_folder path: %\nforce_write: %".ParseArgs(local_folder.Path, force_write), PrimaryButtonText = "OK" };
        //await dialog.ShowAsync();

        return config_data;
    }

    public async Task<ConfigurationData> ReadConfigFile(string file_name)
    {
        StorageFile file = null;
        try { file = await local_folder.GetFileAsync(file_name); } catch { }

        if (file == null) return new ConfigurationData() { is_error = true, error = "File does not exist!" };
        else
        {
            try { file = await local_folder.GetFileAsync(file_name); }
            catch (Exception e) { }
        }

        // Parse stuff
        ConfigurationData parsed_data = await ParseConfigFile(file);
        return parsed_data;

        //ConfigurationData data = new ConfigurationData() { file_name = file_name, is_error = true, error = "Not parsed!" };
        //return data;
    }

    /// Parsing:

    public static async Task<ConfigurationData> ParseConfigFile(StorageFile file)
    {
        string text = await FileIO.ReadTextAsync(file);

        //ContentDialog dialog = new ContentDialog() { Title = "Contents of %".ParseArgs(file.Name), Content = text, PrimaryButtonText = "OK" };
        //await dialog.ShowAsync();

        ConfigurationData data = new ConfigurationData() { file_name = file.Name };

        // Tokens:
        ConfigTokenizer tokenizer = new ConfigTokenizer(text);
        List<Token> tokens = new List<Token>();

        while (!tokenizer.end_of_file)
        {
            Token token = tokenizer.GetToken();
            tokens.Add(token);
        }

        // Interpretation:
        string last_section = SECTION_GLOBAL; // TODO: consider making this a const!
        for (int i = 0; i < tokens.Count; ++i)
        {
            Token token = tokens[i];

            switch (token.Type)
            {
                case Token_Type.Meta_Identifier:
                    {
                        string s = token.Value;
                        bool value = s[0] != '!';
                        if (!value) s.Substring(1, s.Length - 1);

                        if (s == "name") { token = tokens[++i]; if (token.Type == Token_Type.Identifier || token.Type == Token_Type.String) data.config_name = token.Value; }
                        // TODO: more! (?)

                        break;
                    }
                case Token_Type.Section:
                    {
                        string s = token.Value;
                        if (s.IsEmpty()) s = SECTION_GLOBAL;

                        data.AddSection(s);
                        last_section = s;
                        break;
                    }
                case Token_Type.Identifier:
                    {
                        string name = token.Value;
                        // Advance to the next token.
                        if ((i + 1) < tokens.Count) token = tokens[++i];

                        string value = null;
                        if (token.Type == Token_Type.Identifier || token.Type == Token_Type.Number || token.Type == Token_Type.String)
                            value = token.Value;
                        else if (token.Type == Token_Type.OpenParen)
                        {
                            string s = "(";
                            while (token.Type != Token_Type.CloseParen)
                            {
                                token = tokens[++i]; // TODO: should we bounds-check everywhere?
                                if (token.Type == Token_Type.Identifier || token.Type == Token_Type.Number || token.Type == Token_Type.String)
                                    s += token.Value + ", ";
                            }
                            s = s.Remove(s.Length - 2, 2) + ')';

                            value = s;
                            //Logger.LogConsoleW("datauration: Multi-values are not yet supported! ('%')", file_name);
                        }

                        data.AddVariable(last_section, name, value);
                        break;
                    }
            } // switch
        } // for

        return data;
    }
}

public enum Token_Type { EndOfFile, Unknown, Meta_Identifier, Comment, Section, Identifier, Number, OpenParen, CloseParen, String }
public class Token
{
    public Token(Token_Type type = Token_Type.Unknown) { Type = type; }
    public Token(Token_Type type, string value) { Type = type; Value = value; }

    public Token_Type Type;
    public string Value;
}
public class ConfigTokenizer
{
    public ConfigTokenizer(string text) { Text = text; c = text[0]; }

    public string Text;
    public char c;
    public int cursor;
    public bool end_of_file;

    public char Advance(bool return_next = true)
    {
        if (cursor + 1 < Text.Length)
            c = Text[++cursor];
        else { end_of_file = true; return '\0'; }

        if (return_next) return c;
        else if (cursor > 0) return Text[cursor - 1];
        else return '\0';
    }

    public void Backwards()
    {
        if (cursor > 0)
            c = Text[--cursor];
    }
    public char Peek()
    {
        char c = Advance();
        Backwards();
        return c;
    }

    bool include_string_quotes = false;
    public Token GetToken()
    {
        Token token = null;
        if (end_of_file) return token;

        while (!end_of_file && c.IsWhitespace()) Advance();
        switch (c)
        {
            case '/':
                {
                    token = new Token(Token_Type.Comment);
                    while (end_of_file && (c != '\r' && c != '\n'))
                    {
                        token.Value += c; // Include comment marks
                        Advance();
                    }
                    break;
                }
            case '#':
                {
                    token = new Token(Token_Type.Meta_Identifier);
                    Advance(); // Skip '#'
                    if (c == ' ') Advance(); // Skip spaces
                    while (!end_of_file && /*!c.IsNewline()*/ !c.IsWhitespace())
                        token.Value += Advance(false);
                    break;
                }
            case ':':
                {
                    token = new Token(Token_Type.Section);
                    Advance();
                    while (!end_of_file && !Peek().IsNewline())
                        token.Value += Advance();
                    break;
                }
            case '(': token = new Token(Token_Type.OpenParen); break;
            case ')': token = new Token(Token_Type.CloseParen); break;
            case '"':
                {
                    token = new Token(Token_Type.String);
                    int pos = 0;
                    while (!end_of_file)
                    {
                        if (c == '"')
                        {
                            if (include_string_quotes) token.Value += Advance(false);
                            else Advance();

                            if (pos > 0) break;
                        }
                        else
                            token.Value += Advance(false);

                        ++pos;
                    }
                    break;
                }
            case char x when (char.IsLetter(x)):
                {
                    string ident = "";
                    while (!end_of_file && !c.IsWhitespace() && c != ')')
                    {
                        ident += c;
                        Advance();
                    }
                    token = new Token(Token_Type.Identifier, ident);

                    if (c == ')') Backwards();
                    break;
                }
            case char x when (char.IsDigit(x) || x == '-' || x == '.'):
                {
                    string text = "";
                    while (!end_of_file && !c.IsWhitespace() && c != ',' && c != ')')
                    {
                        text += c;
                        Advance();
                    }
                    token = new Token(Token_Type.Number, text);

                    if (c == ')') Backwards(); // // Traverse -1 to Leave ')' to be parsed
                    break;
                }
            default:
                token = new Token(Token_Type.Unknown); break;
        }

        Advance();
        return token;
    }

}