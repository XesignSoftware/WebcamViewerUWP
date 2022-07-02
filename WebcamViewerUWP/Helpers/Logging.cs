namespace WebcamViewerUWP {
    public static class Logging {
        public static void log_append(string message) {
            AppVariables app_vars = AppVariables.GetInstance();
            if (!app_vars) {
                Popups.TextMessageDialog("Logging failed", "Could not find AppVariables instance.");
                return;
            }

            app_vars.console_text += message;
        }

        public static void log(string message) => log_append(message + '\n');
    }
}
