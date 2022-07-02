using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using static WebcamViewerUWP.Logging;

namespace WebcamViewerUWP {
    public partial class ConfigFile {
        public const string CONFIG_FileExtension = ".conf";

        public string name;
        public string file_name;

        public ConfigFile(string file_name) {
            new Action(async () => await read_local_file(file_name))();
        }

        async Task<bool> read_local_file(string file_name) {
            if (!file_name.Contains(CONFIG_FileExtension)) file_name += CONFIG_FileExtension;
            log($"[config] Loading file: '{file_name}'");

            StorageFolder local_folder = ApplicationData.Current.LocalFolder;
            StorageFile file = null;
            try {
                file = await local_folder.GetFileAsync(file_name);
            } catch (Exception ex) {
                Popups.TextContentDialog("Failed to load configuration file", $"Failed file name: '{file_name}'\nError: {ex.Message}");
                return false;
            }

            List<Token> tokens = await lex_file(file);

            Popups.TextContentDialog("[config] Success", $"Loaded {file_name}.\nFull path: {file.Path}");
            return true;
        }


    }

    public partial class ConfigFile {
        public enum Token_Type {
            UNKNOWN,
            Identifier, Number, Comment, String,
            OpenParen, OpenBrace,
            CloseParen, CloseBrace,
            Colon, Semicolon,
        }
        public struct Token {
            public Token_Type type;
            public string     value;
            public (int row, int col) parsed_at;
        }

        public async Task<List<Token>> lex_file(StorageFile file) {
            log($"[config] Attempting to lex file '{file.Name}'...");

            var lines = await File.ReadAllLinesAsync(file.Path);
            if (lines == null || lines.Length == 0) {
                Popups.TextContentDialog("Empty file", $"The configuration file is empty.\nFile path: {file.Path}");
                return null;
            }

            for (int i = 0; i < lines.Length; ++i) {
                string line = lines[i];
                log($"  - line at {i}: '{line}'");

                for (int x = 0; x < line.Length; ++x) {
                    char c = line[x];
                    // ...
                }
            }

            log("[config] Lexing successful!");

            return null;
        }
    }
}
