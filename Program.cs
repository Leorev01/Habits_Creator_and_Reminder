using HabitTrackerApp.Classes;
using HabitTrackerApp.Classes.Services;
using HabitTrackerApp.Services;

namespace HabitTrackerApp
{
    class Program
    {
        private static GoogleCalendarService _calendarService = new GoogleCalendarService();
        static List<Habit> habits = Storage.Load();
        static ReminderService reminderService = new ReminderService();

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            // Apply the saved theme when the application starts
            ThemeService.ApplySavedTheme();

            await LanguageService.LoadLanguagesAsync();
            await LocalizationService.InitializeAsync();

            DotNetEnv.Env.Load();

            Console.WriteLine(LocalizationService.GetString("WelcomeMessage"));

            while (true)
            {
                var menuItems = GetMainMenuItems();

                Console.WriteLine();
                Console.WriteLine(LocalizationService.GetString("ChooseOption"));
                Console.WriteLine();

                for (int i = 0; i < menuItems.Count; i++)
                {
                    var item = menuItems[i];
                    var title = LocalizationService.GetString(item.TitleKey);
                    var suffix = item.SuffixFactory?.Invoke();

                    if (string.IsNullOrWhiteSpace(suffix))
                        Console.WriteLine($"{i + 1}. {title}");
                    else
                        Console.WriteLine($"{i + 1}. {title} {suffix}");
                }

                Console.WriteLine();
                Console.Write(LocalizationService.GetString("EnterMenuOption"));
                var input = Console.ReadLine();

                if (int.TryParse(input, out int choice) && choice >= 1 && choice <= menuItems.Count)
                {
                    var selectedItem = menuItems[choice - 1];
                    await selectedItem.Action();

                    if (selectedItem.IsExit)
                        return;
                }
                else
                {
                    Console.WriteLine(LocalizationService.GetString("InvalidOption"));
                }
            }
        }

        static async Task ChangeLanguageAsync()
        {
            Console.WriteLine();
            Console.WriteLine(LocalizationService.GetString("MenuChangeLanguage") + ":");
            Console.WriteLine();

            var languages = LanguageService.GetLanguages();
            var auditResults = await LocalizationAuditService.AuditAllAsync();
            var auditMap = auditResults.ToDictionary(x => x.LanguageCode, x => x);

            int i = 1;
            foreach (var lang in languages)
            {
                string status = string.Empty;

                if (lang.Key != "en" && auditMap.TryGetValue(lang.Key, out var audit) && audit.NeedsUpdate)
                {
                    status = $"⚠️ [needs update: {audit.MissingCount + audit.EmptyCount}]";
                }

                Console.WriteLine($"{i}. {lang.Value} ({lang.Key}){status}");
                i++;
            }
            Console.WriteLine();

            Console.WriteLine();
            Console.Write(LocalizationService.GetString("SelectLanguage"));
            var input = Console.ReadLine();

            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= languages.Count)
            {
                string selectedCode = languages.ElementAt(choice - 1).Key;
                await LocalizationService.SetLanguageAsync(selectedCode);
                ThemeService.ApplySavedTheme();
                Console.WriteLine($"✅ {LocalizationService.GetString("LanguageChanged")}");
            }
            else
            {
                Console.WriteLine(LocalizationService.GetString("InvalidOption"));
            }
        }

        static void AddHabit()
        {
            Console.WriteLine();
            Console.Write(LocalizationService.GetString("EnterHabitName"));
            var name = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine(LocalizationService.GetString("HabitNameRequired"));
                return;
            }

            habits.Add(new Habit(name));
            Console.WriteLine(string.Format(LocalizationService.GetString("AddedHabit"), name));
            Storage.Save(habits);
        }

        static void ViewHabits()
        {
            Console.WriteLine();
            if (habits.Count == 0)
            {
                Console.WriteLine(LocalizationService.GetString("NoHabitsYet"));
                return;
            }

            Console.WriteLine(LocalizationService.GetString("YourHabits"));
            Console.WriteLine(new string('-', 50));

            for (int i = 0; i < habits.Count; i++)
            {
                var streak = habits[i].CurrentStreak;
                var maxStreak = habits[i].MaxStreak;
                var streakEmoji = streak > 0 ? "🔥" : "⭕";
                var line = string.Format(
                    LocalizationService.GetString("HabitLineFormat"),
                    i + 1,
                    habits[i].Name,
                    streak,
                    streakEmoji,
                    maxStreak
                );

                Console.WriteLine(line);
            }

            Console.WriteLine(new string('-', 50));
        }

        static void MarkHabitComplete()
        {
            ViewHabits();

            if (habits.Count == 0)
                return;

            Console.WriteLine();
            Console.Write(LocalizationService.GetString("SelectHabitComplete"));
            var input = Console.ReadLine();

            if (int.TryParse(input, out int index) && index >= 1 && index <= habits.Count)
            {
                var habit = habits[index - 1];
                habit.MarkComplete();
                Storage.Save(habits);
                Console.WriteLine(string.Format(LocalizationService.GetString("HabitMarkedComplete"), habit.Name, LocalizationService.GetString("Today")));
            }
            else
            {
                Console.WriteLine(LocalizationService.GetString("InvalidHabitNumber"));
            }
        }

        static void ViewHabitHistory()
        {
            ViewHabits();

            if (habits.Count == 0)
                return;

            Console.WriteLine();
            Console.Write(LocalizationService.GetString("SelectHabitHistory"));
            var input = Console.ReadLine();

            if (int.TryParse(input, out int index) && index >= 1 && index <= habits.Count)
            {
                Console.WriteLine();
                habits[index - 1].ShowHistory();
            }
            else
            {
                Console.WriteLine(LocalizationService.GetString("InvalidHabitNumber"));
            }
        }

        static void DeleteHabit()
        {
            ViewHabits();

            if (habits.Count == 0)
                return;

            Console.WriteLine();
            Console.Write(LocalizationService.GetString("SelectHabitDelete"));
            var input = Console.ReadLine();

            if (int.TryParse(input, out int index) && index >= 1 && index <= habits.Count)
            {
                var habitName = habits[index - 1].Name;

                Console.Write(string.Format(LocalizationService.GetString("ConfirmDelete"), habitName));
                var confirmation = Console.ReadLine()?.ToLower();

                if (confirmation == "y" || confirmation == "yes" || confirmation == LocalizationService.GetString("Yes").ToLower())
                {
                    habits.RemoveAt(index - 1);
                    Console.WriteLine(string.Format(LocalizationService.GetString("DeletedHabit"), habitName));
                    Storage.Save(habits);
                }
                else
                {
                    Console.WriteLine(LocalizationService.GetString("DeleteCancelled"));
                }
            }
            else
            {
                Console.WriteLine(LocalizationService.GetString("InvalidHabitNumber"));
            }
        }

        static void ResetHabit()
        {
            ViewHabits();

            if (habits.Count == 0)
                return;

            Console.WriteLine();
            Console.Write(LocalizationService.GetString("SelectHabitReset"));
            var input = Console.ReadLine();

            if (int.TryParse(input, out int index) && index >= 1 && index <= habits.Count)
            {
                var habitName = habits[index - 1].Name;

                Console.Write(string.Format(LocalizationService.GetString("ConfirmReset"), habitName));
                var confirmation = Console.ReadLine()?.ToLower();

                if (confirmation == "y" || confirmation == "yes" || confirmation == LocalizationService.GetString("Yes").ToLower())
                {
                    habits[index - 1].ResetCurrentStreak();
                    Console.WriteLine(string.Format(LocalizationService.GetString("ResetedHabit"), habitName));
                    Storage.Save(habits);
                }
                else
                {
                    Console.WriteLine(LocalizationService.GetString("ResetCancelled"));
                }
            }
            else
            {
                Console.WriteLine(LocalizationService.GetString("InvalidHabitNumber"));
            }
        }

        static void SetEmailReminders()
        {
            Console.WriteLine();
            Console.WriteLine(LocalizationService.GetString("ReminderTimePrompt"));
            Console.Write(LocalizationService.GetString("EnterTime"));
            var input = Console.ReadLine();

            if (!TimeSpan.TryParse(input, out TimeSpan reminderTime))
            {
                Console.WriteLine(LocalizationService.GetString("InvalidTimeFormat"));
                return;
            }

            try
            {
                reminderService.SendReminders(habits, reminderTime);
                Console.WriteLine(string.Format(LocalizationService.GetString("ReminderSet"), reminderTime));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ {LocalizationService.GetString("ReminderSetupFailed")}: {ex.Message}");
            }
        }

        static async Task SyncToGoogleCalendar()
        {
            Console.WriteLine();

            if (habits.Count == 0)
            {
                Console.WriteLine(LocalizationService.GetString("NoHabitsToSync"));
                return;
            }

            // Initialize Google Calendar only when needed
            if (!_calendarService.IsInitialized)
            {
                Console.WriteLine("🔐 " + LocalizationService.GetString("ConnectingToCalendar"));
                try
                {
                    bool initialized = await _calendarService.InitializeAsync();
                    if (!initialized)
                    {
                        Console.WriteLine("❌ " + LocalizationService.GetString("CalendarConnectionFailed"));
                        return;
                    }
                    Console.WriteLine("✅ " + LocalizationService.GetString("CalendarConnected"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ {LocalizationService.GetString("CalendarConnectionFailed")}: {ex.Message}");
                    return;
                }
            }

            Console.WriteLine(LocalizationService.GetString("CalendarSyncPrompt"));
            Console.Write(LocalizationService.GetString("EnterDateOrToday"));
            string input = Console.ReadLine();

            DateTime targetDate = DateTime.Today;
            if (!string.IsNullOrEmpty(input) && DateTime.TryParse(input, out DateTime parsed))
            {
                targetDate = parsed;
            }

            Console.WriteLine();
            Console.WriteLine(string.Format(LocalizationService.GetString("SyncingToCalendar"), targetDate.ToString("yyyy-MM-dd")));

            int successCount = 0;
            int failureCount = 0;

            // Show progress
            for (int i = 0; i < habits.Count; i++)
            {
                Console.Write($"\r{LocalizationService.GetString("SyncingHabit")} {i + 1}/{habits.Count}: {habits[i].Name}...");

                try
                {
                    bool success = await _calendarService.CreateHabitEventAsync(habits[i], targetDate);
                    if (success)
                    {
                        successCount++;
                    }
                    else
                    {
                        failureCount++;
                        Console.WriteLine($"\n❌ {LocalizationService.GetString("SyncFailedFor")} {habits[i].Name}");
                    }
                }
                catch (Exception ex)
                {
                    failureCount++;
                    Console.WriteLine($"\n❌ {LocalizationService.GetString("SyncFailedFor")} {habits[i].Name}: {ex.Message}");
                }

                // Small delay to prevent API rate limiting
                await Task.Delay(500);
            }

            Console.WriteLine();
            if (successCount > 0)
            {
                Console.WriteLine($"✅ {string.Format(LocalizationService.GetString("CalendarSyncComplete"), successCount, targetDate.ToString("yyyy-MM-dd"))}");
            }

            if (failureCount > 0)
            {
                Console.WriteLine($"⚠️ {string.Format(LocalizationService.GetString("CalendarSyncPartialFailure"), failureCount)}");
            }
        }

        static async Task ExitApplication()
        {
            Console.WriteLine();
            Console.WriteLine(LocalizationService.GetString("SavingData"));

            try
            {
                Storage.Save(habits);
                reminderService.StopReminders();
                Console.WriteLine(LocalizationService.GetString("DataSaved"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ {LocalizationService.GetString("SaveError")}: {ex.Message}");
            }

            Console.WriteLine(LocalizationService.GetString("GoodbyeMessage"));
            await Task.Delay(1000); // Brief pause for user to see the message
        }

        private static List<MenuItem> GetMainMenuItems()
        {
            return new List<MenuItem>
    {
        new MenuItem("MenuAddHabit", () =>
        {
            AddHabit();
            return Task.CompletedTask;
        }),

        new MenuItem("MenuViewHabits", () =>
        {
            ViewHabits();
            return Task.CompletedTask;
        }),

        new MenuItem("MenuMarkComplete", () =>
        {
            MarkHabitComplete();
            return Task.CompletedTask;
        }),

        new MenuItem("MenuViewHistory", () =>
        {
            ViewHabitHistory();
            return Task.CompletedTask;
        }),

        new MenuItem("MenuDeleteHabit", () =>
        {
            DeleteHabit();
            return Task.CompletedTask;
        }),

        new MenuItem("MenuResetHabit", () =>
        {
            ResetHabit();
            return Task.CompletedTask;
        }),

        new MenuItem("MenuSetReminders", () =>
        {
            SetEmailReminders();
            return Task.CompletedTask;
        }),

        new MenuItem("MenuChangeLanguage", () => ChangeLanguageAsync()),

        new MenuItem(
            "MenuToggleTheme",
            () =>
            {
                ThemeService.ToggleTheme();
                return Task.CompletedTask;
            },
            () => ThemeService.GetThemeIcon()
        ),

        new MenuItem(
            "MenuSyncCalendar",
            () => SyncToGoogleCalendar(),
            () => _calendarService.IsInitialized ? "✅" : "🔒"
        ),

        new MenuItem("MenuExit", () => ExitApplication(), isExit: true)
    };
        }
    }
}