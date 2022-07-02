using System;
using System.Collections.Generic;
using static WebcamViewerUWP.Logging;

namespace WebcamViewerUWP {
    public enum DebugConsoleCommandType { Function, Variable }

    public class DebugConsoleCommand {
        public string   name;
        public string[] aliases;
        public DebugConsoleCommandType type;

        public bool close_console_on_invoke = false;
        public string help_text;

        // for Function-type commands:
        public Action           action_empty;
        public Action<string[]> action_params;

        // for Variable-type commands:
        public Obj_Ref<object> obj_ref_boxed;
    }

    public class DebugConsoleCommand<T> : DebugConsoleCommand {
        // for Variable-type commands:
        public Obj_Ref<T> obj_ref;
        // public T get() => obj_ref.get();
        // public void set(ref T val) =>  obj_ref.set(ref val);
    }

    public class Obj_Ref<T> {
        public Obj_Ref(ref T obj) {
            this.obj = obj;
        }
        public T obj;
        public T get() => obj;
        public void set(ref T val) => obj = val;
    }

    // Commands registering:
    public partial class DebugConsoleCommands {
        public static implicit operator bool(DebugConsoleCommands it) => it != null;

        static DebugConsoleCommands instance;
        public static DebugConsoleCommands get_instance() {
            if (!instance) instance = new DebugConsoleCommands();
            return instance;
        }
        public DebugConsoleCommands() {
            if (instance) return;
            instance = this;

            register_default_commands();
        }

        public Dictionary<string, DebugConsoleCommand> commands = new Dictionary<string, DebugConsoleCommand>();

        // Function-type commands:
        public DebugConsoleCommand register_command(Action action_empty, string help_text = null, params string[] aliases) {
            if (commands.ContainsKey(action_empty.Method.Name)) {
                log($"Command '{action_empty.Method.Name}' is already registered.");
                return null;
            }
            foreach (string s in aliases) {
                if (commands.ContainsKey(s)) {
                    log($"Command '{s}' is already registered.");
                    return null;
                }
            }

            DebugConsoleCommand cmd = new DebugConsoleCommand();
            cmd.name = action_empty.Method.Name;
            cmd.aliases = aliases;
            cmd.help_text = help_text;
            cmd.action_empty = action_empty;

            commands.Add(cmd.name, cmd);
            foreach (string s in aliases) commands.Add(s, cmd);

            return cmd;
        }
        // TODO: Code duplication!!!
        public DebugConsoleCommand register_command(Action<string[]> action_params, string help_text = null, params string[] aliases) {
            if (commands.ContainsKey(action_params.Method.Name)) {
                log($"Command '{action_params.Method.Name}' is already registered.");
                return null;
            }
            foreach (string s in aliases) {
                if (commands.ContainsKey(s)) {
                    log($"Command '{s}' is already registered.");
                    return null;
                }
            }

            DebugConsoleCommand cmd = new DebugConsoleCommand();
            cmd.name = action_params.Method.Name;
            cmd.aliases = aliases;
            cmd.help_text = help_text;
            cmd.action_params = action_params;

            commands.Add(cmd.name, cmd);
            foreach (string s in aliases) commands.Add(s, cmd);

            return cmd;
        }

        // Variable-type commands:
        static Type[] allowed_types = { 
            typeof(int), typeof(float), typeof(double),
            typeof(string), typeof(bool)
        };
        public DebugConsoleCommand register_command<T>(ref T obj, string help_text = null, params string[] aliases) {
            string obj_name = obj.ToString();

            if (commands.ContainsKey(obj_name)) {
                log($"Command '{obj_name}' is already registered.");
                return null;
            }
            foreach (string s in aliases) {
                if (commands.ContainsKey(s)) {
                    log($"Command '{s}' is already registered.");
                    return null;
                }
            }

            Type obj_type = obj.GetType();
            bool success = false;
            foreach (Type t in allowed_types) if (obj_type == t) success = true;
            if (!success) {
                log($"Could not register command for object '{obj_name}' - type '{obj_type.Name}' is not supported.");
                return null;
            }

            DebugConsoleCommand<T> cmd = new DebugConsoleCommand<T>();
            cmd.name = obj.ToString();
            cmd.aliases = aliases;
            cmd.help_text = help_text;

            Obj_Ref<T> obj_ref = new Obj_Ref<T>(ref obj);
            cmd.obj_ref = obj_ref;
            cmd.obj_ref_boxed = obj_ref as Obj_Ref<object>;

            commands.Add(cmd.name, cmd);
            foreach (string s in aliases) commands.Add(s, cmd);

            return cmd;
        }
    }

    // Default commands:
    public partial class DebugConsoleCommands {
        public void register_default_commands() {
            register_command(help);
        }

        void help() {
            log("[help] Listing all commands...");
            foreach (DebugConsoleCommand cmd in commands.Values) {
                log($"  - {cmd.name}{(cmd.help_text == null ? null : $" - {cmd.help_text}")}");
            }
        }
    }

    // Commands parsing and interpretation:
    public partial class DebugConsoleCommands {
        public bool interpret_input(string input) {
            string[] tokens = input.Split(' ');

            string command = tokens[0];
            string[] args = null;
            if (tokens.Length > 1) {
                args = new string[tokens.Length - 1];
                for (int i = 1; i < tokens.Length; ++i) args[i-1] = tokens[i];
            }

            //Popups.TextContentDialog($"command: '{command}'\nargs: {(args == null ? "null" : string.Join(';', args))}");

            if (!commands.ContainsKey(command)) {
                log($"[console] could not find the command '{command}' or any of its aliases.");
                return false;
            }

            DebugConsoleCommand cmd = commands[command];

            if (cmd.type == DebugConsoleCommandType.Function) {
                if (cmd.action_empty != null) cmd.action_empty.Invoke();
                else if (cmd.action_params != null) cmd.action_params.Invoke(args);
            } else if (cmd.type == DebugConsoleCommandType.Variable) {
                if (args == null) log($"Value for variable '{cmd.name}': {cmd.obj_ref_boxed.get()}");
                else log("TODO TODO TODO");
            }

            if (cmd.close_console_on_invoke) DebugConsole.get_instance().close();

            return true;
        }
    }
}
