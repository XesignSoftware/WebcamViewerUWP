namespace WebcamViewerUWP {
    public class State {
        static State Instance;
        public static State GetInstance() {
            if (Instance == null) return new State();
            return Instance;
        }

        public State() {
            if (Instance != null) {
                Popups.TextMessageDialog("A State instance already exists", "Tried creating a new State when one already exists. \nThis is bad!");
                return;
            }
            Instance = this;
        }

        public double system_titlebar_height = 32;
        public double system_titlebar_reserved_width = 100; // TODO: fill in actual value for my system
        public double total_titlebar_height { get { return system_titlebar_height + TITLEBAR_AdditionalHeight; } }

        #region Variables (TODO: this probably is not the right place for them?)
        public bool TITLEBAR_AllowCustomTitleBar  = true;
        public double TITLEBAR_AdditionalHeight = 0.00;


        #endregion
    }
}
