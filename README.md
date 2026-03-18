# 📅 Habit Tracker App

A comprehensive C# console application to help you build and maintain daily habits. Track streaks, view progress history, receive daily email reminders, and sync your habits with Google Calendar for seamless integration across all your devices.

---

## ✨ Features

- ✅ Add, reset, and delete daily habits
- 📈 View current streaks and habit history
- ✅ Mark habits as completed
- ⏰ Schedule **daily email reminders**
- 📅 **Google Calendar integration** — sync habits as all-day events
- 💾 Data persistence using local file storage
- 🎨 **Dark Mode support** (see [THEME_SERVICE.md](THEME_SERVICE.md))
- 🌐 **Multi-language support** with runtime language switching
- 🔍 **Localization audit support** to highlight languages that need updates

---

## 🚀 Getting Started

### 🔧 Requirements

- [.NET 6+ SDK](https://dotnet.microsoft.com/en-us/download)
- A Gmail account with [App Passwords enabled](https://support.google.com/accounts/answer/185833) (for email reminders)
- A Google account for Calendar integration (optional)

---

## 📥 Installation

1. **Clone this repository**

```bash
git clone https://github.com/Leorev01/Habits_Builder.git
cd Habits_Builder
```

2. **Restore dependencies**

```bash
dotnet restore
```

3. **Build the application**

```bash
dotnet build
```

---

## ⚙️ Configuration

### 📧 Email Setup (Optional)

1. **Create a `.env` file** in the project root:

```env
GOOGLE_APP_EMAIL=your-email@gmail.com
GOOGLE_APP_PASSWORD=your-app-password
```

2. **Enable App Passwords** in your Gmail account:
   - Go to [Google Account Settings](https://myaccount.google.com/)
   - Security → 2-Step Verification → App passwords
   - Generate a new app password for "Habit Tracker"

### 📅 Google Calendar Setup (Optional)

#### Quick Setup with Mock File

1. **Create a mock `credentials.json` file** in your project root:

```json
{
  "installed": {
    "client_id": "YOUR_CLIENT_ID_HERE.apps.googleusercontent.com",
    "project_id": "your-project-id",
    "auth_uri": "https://accounts.google.com/o/oauth2/auth",
    "token_uri": "https://oauth2.googleapis.com/token",
    "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
    "client_secret": "YOUR_CLIENT_SECRET_HERE",
    "redirect_uris": ["http://localhost"]
  }
}
```

2. **Replace the mock values with your actual credentials**

#### Detailed Google Cloud Console Setup

1. **Go to Google Cloud Console**
   - Visit [Google Cloud Console](https://console.cloud.google.com/)
   - Sign in with your Google account

2. **Create a new project**
   - Click **New Project**
   - Name it **Habit Tracker**
   - Click **Create**

3. **Enable Google Calendar API**
   - Go to **APIs & Services → Library**
   - Search for **Google Calendar API**
   - Click **Enable**

4. **Create OAuth 2.0 credentials**
   - Go to **APIs & Services → Credentials**
   - Click **Create Credentials** → **OAuth client ID**
   - Configure the OAuth consent screen (External user type)
   - Fill the required fields:
     - App name: **Habit Tracker**
     - User support email: your email
     - Add your email to test users
   - Choose **Desktop application** as application type
   - Name it **Habit Tracker Desktop**
   - Click **Create**

5. **Download and replace credentials**
   - Click the download button next to your OAuth client
   - Replace the mock `credentials.json` file with the downloaded file
   - Or copy the values from the downloaded file into your mock file:
     - `client_id`: replace `YOUR_CLIENT_ID_HERE`
     - `client_secret`: replace `YOUR_CLIENT_SECRET_HERE`
     - `project_id`: replace `your-project-id`

6. **First-time authentication**
   - When you first use the calendar sync feature, a browser window will open
   - Sign in to your Google account
   - Grant permission to access your calendar
   - The app will save your authentication tokens for future use

---

## 🏃‍♂️ Running the Application

```bash
dotnet run
```

---

## 📖 Usage

### Main Menu Options

1. **Add Habit** — create a new daily habit
2. **View Habits** — see all habits with current streaks
3. **Mark Habit Complete** — mark a habit as completed for today
4. **View Habit History** — see completion history for a specific habit
5. **Delete Habit** — remove a habit permanently
6. **Reset Habit** — reset a habit streak and history
7. **Set Email Reminders** — schedule daily email notifications
8. **Change Language** — switch between available localizations
9. **Toggle Theme** — switch between light and dark mode
10. **Sync Google Calendar** — create calendar events for your habits
11. **Exit** — save data and quit

### 🌍 Language Menu

The language selection menu only shows languages that have a matching localization file.

If a localization is behind the English master file, the menu may show an indicator such as:

```text
⚠️ [needs update: N]
```

This helps identify translations that still require missing keys or updates.

### 📅 Calendar Integration

- **Sync Today**: press Enter when prompted for a date
- **Sync Specific Date**: enter a date in format `yyyy-mm-dd` (for example, `2025-01-07`)
- **All-Day Events**: habits appear as all-day events in Google Calendar
- **Smart Reminders**: automatic popup and email reminders
- **Duplicate Prevention**: the app avoids duplicate events for the same habit/date
- **Color Coding**: habit events appear in green for easier identification

### 🔒 Security Notes

- Your `credentials.json` file contains sensitive information
- Never share this file publicly
- The file is automatically excluded from version control via `.gitignore`
- Authentication tokens are stored locally in `token.json`

---

## 🗂️ Project Structure

```text
HabitTrackerApp/
├── Classes/
│   ├── Habit.cs                       # Core habit model
│   ├── Storage.cs                     # Data persistence
│   ├── ReminderService.cs             # Email reminders
│   └── Services/
│       ├── GoogleCalendarService.cs   # Calendar integration
│       ├── LanguageService.cs         # Language discovery and filtering
│       ├── LocalizationAuditService.cs# Localization audit and status checks
│       ├── LocalizationService.cs     # Localization loading and fallback
│       └── ThemeService.cs            # Theme switching
├── Resources/
│   ├── Languages.json                 # Known language codes and display names
│   └── Localizations/
│       ├── en.json                    # English (master / fallback)
│       ├── fr.json                    # French
│       ├── it.json                    # Italian
│       ├── ru.json                    # Russian
│       └── uk.json                    # Ukrainian
├── credentials.json                   # Google API credentials (not in repo)
├── .env                               # Email configuration (not in repo)
├── habits.json                        # Habit data storage
├── token.json                         # Google auth tokens (auto-generated)
└── Program.cs                         # Main application entry point
```

---

## 🌍 Localization

This project supports multiple languages with dynamic runtime switching.

### Currently shipped localizations

- 🇺🇸 English (default)
- 🇫🇷 French
- 🇮🇹 Italian
- 🇷🇺 Russian
- 🇺🇦 Ukrainian

### How localization availability works

- `Resources/Languages.json` stores known language codes and display names
- `Resources/Localizations/{code}.json` stores the actual translation file for that language
- A language appears in the app only when both the code and the matching localization file exist

Because `Languages.json` contains a broader catalog of languages, in many cases adding a new localization only requires creating:

```text
Resources/Localizations/{code}.json
```

If the language code is not listed yet, add it to `Resources/Languages.json` first.

For the full workflow, see [LOCALIZATION_GUIDE.md](LOCALIZATION_GUIDE.md).

---

## 🎨 Themes

- **Light Mode**: default bright theme
- **Dark Mode**: easier on the eyes in low-light usage
- **Dynamic Switching**: toggle themes at runtime
- **Persistent Settings**: your theme preference is saved

See [THEME_SERVICE.md](THEME_SERVICE.md) for more details.

---

## 📝 Contributing

Please read our [contribution guidelines](CONTRIBUTING.md) to learn how to propose changes and report issues.

---

## 🐛 Troubleshooting

### Common Issues

**Calendar sync fails with "Invalid time zone"**
- Ensure you're using the latest version of the app
- Check your internet connection

**Email reminders not working**
- Verify your `.env` file has correct Gmail credentials
- Ensure App Passwords are enabled in Gmail

**"OAuth client was not found" error**
- Download a fresh `credentials.json` from Google Cloud Console
- Ensure Google Calendar API is enabled in your project
- Make sure you replaced the mock values with your actual credentials

**"Access denied" error**
- Add your email as a test user in the OAuth consent screen
- Or publish your app (not recommended for personal use)

**Mock `credentials.json` not working**
- Replace `YOUR_CLIENT_ID_HERE` and `YOUR_CLIENT_SECRET_HERE` with actual values
- Ensure the project ID matches your Google Cloud project

**A language does not appear in the menu**
- Make sure the language code exists in `Resources/Languages.json`
- Make sure a matching file exists in `Resources/Localizations`
- Make sure the file name exactly matches the language code
- Make sure the JSON is valid

### Support

If you encounter issues:
1. Check the troubleshooting section above
2. Review the [Issues](https://github.com/Leorev01/Habits_Builder/issues) page
3. Create a new issue with detailed error information

---

## 📄 License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- Google Calendar API for seamless calendar integration
- .NET Community for excellent documentation and support
- Contributors who helped improve this application

---

**Happy habit building! 🎯**

