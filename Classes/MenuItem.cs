namespace HabitTrackerApp.Classes
{
    public class MenuItem
    {
        public string TitleKey { get; }
        public Func<Task> Action { get; }
        public Func<string>? SuffixFactory { get; }
        public bool IsExit { get; }

        public MenuItem(
            string titleKey,
            Func<Task> action,
            Func<string>? suffixFactory = null,
            bool isExit = false)
        {
            TitleKey = titleKey;
            Action = action;
            SuffixFactory = suffixFactory;
            IsExit = isExit;
        }
    }
}