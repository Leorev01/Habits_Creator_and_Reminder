# Localization Guide

This guide explains how localization works in **Habit Tracker** and how to add a new language correctly.

---

## Overview

The localization system uses **two sources of information**:

1. `Resources/Languages.json` — the catalog of known languages and their display names.
2. `Resources/Localizations/{code}.json` — the actual translation files.

A language is considered **available in the application** only when **both** of the following are true:

- the language code exists in `Languages.json`
- a matching localization file exists in `Resources/Localizations`

### Example

If `Languages.json` contains:

```json
{
  "en": "English",
  "uk": "Українська",
  "pl": "Polski"
}
```

and the `Resources/Localizations` folder contains:

- `en.json`
- `uk.json`

then the application will show only:

- English
- Українська

`Polski` will **not** appear in the language menu until `pl.json` is added.

---

## File Roles

### `Resources/Languages.json`

This file is the **catalog of known languages**.

It defines:

- the language code
- the display name shown in the UI

Example:

```json
{
  "en": "English",
  "uk": "Українська",
  "fr": "Français",
  "de": "Deutsch",
  "pl": "Polski"
}
```

### `Resources/Localizations/{code}.json`

Each file in this folder contains the actual translated strings for one language.

Examples:

- `en.json`
- `uk.json`
- `fr.json`
- `de.json`

The file name must match the language code from `Languages.json`.

---

## How Language Discovery Works

At startup, the application:

1. loads all known languages from `Resources/Languages.json`
2. scans `Resources/Localizations` for available `.json` files
3. matches file names to language codes
4. builds the final language list using only matching entries

This means:

- if a language exists in `Languages.json` but its localization file is missing, it is ignored
- if a localization file exists but its code is missing from `Languages.json`, it is ignored
- only languages that exist in both places are shown in the language menu and used by localization audit tools

---

## How to Add a New Language

In most cases, adding a new language is simple because `Resources/Languages.json` already contains a broad catalog of known language codes and display names.

That means for many languages, the only thing you need to add is the localization file:

- `Resources/Localizations/{code}.json`

### Standard workflow

### Step 1. Check whether the language already exists in `Languages.json`

Before editing anything, check `Resources/Languages.json`.

If the language code is already listed there, you do **not** need to modify `Languages.json`.
You can go straight to creating the localization file.

Example:

If `pl` already exists in `Languages.json`, then adding Polish only requires:

- `Resources/Localizations/pl.json`

### Step 2. Add the language to `Languages.json` only if it is missing

If the language code is not listed yet, add it manually with its native display name.

Example:

```json
{
  "pl": "Polski"
}
```

Use valid language codes whenever possible.

Recommended examples:

- `en` — English
- `uk` — Ukrainian
- `fr` — French
- `de` — German
- `pl` — Polish
- `it` — Italian
- `es` — Spanish
- `pt-BR` — Portuguese (Brazil)
- `zh-CN` — Chinese (Simplified)
- `zh-TW` — Chinese (Traditional)

> Note: use `uk` for Ukrainian, not `ua`.

### Step 3. Create the localization file

Create a new file in `Resources/Localizations` named after the language code.

Example:

- `Resources/Localizations/pl.json`

### Step 4. Copy the English template

Use `Resources/Localizations/en.json` as the source template.

Copy all keys from `en.json` and translate only the values.

### Step 5. Run the application

If both the code and the file are present:

- the language will appear in the language menu automatically
- the language will be included in localization audit checks

---

## Required Format for Localization Files

Localization files must be JSON dictionaries in this format:

```json
{
  "WelcomeMessage": "📅 Welcome to Habit Tracker!",
  "ChooseOption": "Choose an option:",
  "MenuAddHabit": "1. Add Habit"
}
```

Rules:

- each key must be unique
- keys must match the keys used in `en.json`
- values must be translated strings
- keep placeholders such as `{0}`, `{1}`, `{0:T}`, `{0:F1}` unchanged unless formatting must be adjusted carefully
- keep escape sequences and line breaks valid

---

## English as the Master Localization

`en.json` is the **master localization file**.

It is used as:

- the reference template for all other languages
- the fallback source when a key is missing in another language
- the base file for localization auditing

Because of this:

- `en.json` should always exist
- all new keys should be added to `en.json` first
- other localization files should then be updated to match it

---

## Fallback Behavior

When the application requests a localized string:

1. it first checks the currently selected language
2. if the key is missing, it falls back to English
3. if the key is missing in English too, the key name itself may be shown

This allows the application to keep working even when translations are incomplete.

---

## What Happens in Common Scenarios

### Case 1 — code exists and file exists

- `Languages.json` contains `pl`
- `Resources/Localizations/pl.json` exists

Result:
- Polish appears in the menu
- Polish is treated as a supported language

### Case 2 — code exists but file is missing

- `Languages.json` contains `pl`
- `Resources/Localizations/pl.json` does not exist

Result:
- Polish does not appear in the menu
- Polish is not treated as available yet

### Case 3 — file exists but code is missing

- `Resources/Localizations/pl.json` exists
- `Languages.json` does not contain `pl`

Result:
- Polish does not appear in the menu
- the language is ignored until it is added to `Languages.json`
- the application may generate an internal warning for developers

---

## Best Practices

- Keep `Languages.json` as the single source for display names.
- Keep `en.json` complete and up to date.
- Add new keys to English first.
- Update other languages after English changes.
- Use native language names in `Languages.json` where possible.
- Avoid changing localization keys unless necessary.
- Do not remove keys from translated files unless they were removed from `en.json`.

---

## Recommended Workflow for Adding a Language

1. Check whether the language code already exists in `Resources/Languages.json`
2. If the code is missing, add the language code and display name
3. Copy `Resources/Localizations/en.json`
4. Save it as `Resources/Localizations/{code}.json`
5. Translate all values
6. Run the app
7. Open the language menu and verify the new language appears
8. Run localization audit tools if available

---

## Recommended Workflow for Updating Translations

When new UI strings are added:

1. update `en.json`
2. run localization audit
3. update other language files
4. verify fallback behavior for any missing keys

---

## Troubleshooting

### A language does not appear in the menu

Check the following:

- is the language code present in `Resources/Languages.json`?
- does the matching file exist in `Resources/Localizations`?
- does the file name exactly match the language code?
- is the JSON valid?

### A language file exists but is ignored

Most likely causes:

- the code is missing from `Languages.json`
- the file name does not match the expected code
- the code uses the wrong format

Example:

- correct: `uk.json`
- incorrect: `ua.json`

### Strings appear in English instead of the selected language

This usually means:

- the key is missing in the selected localization file
- the app has fallen back to `en.json`

---

## Notes

- `Languages.json` defines **known languages**, not automatically active languages.
- The presence of a localization file determines whether a language is actually available.
- Because `Languages.json` already contains a broad catalog of known languages, in most cases adding `Resources/Localizations/{code}.json` is enough to activate a new language.
- You only need to edit `Languages.json` when the required language code is not listed yet.

---

## Summary

A language is available only when:

- it is defined in `Resources/Languages.json`
- it has a matching file in `Resources/Localizations/{code}.json`

That keeps the language menu, localization loading, and audit tools consistent.

