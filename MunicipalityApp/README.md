# MunicipalityApp

A WPF desktop application for reporting, viewing, and tracking municipal issues.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022 or newer](https://visualstudio.microsoft.com/)
- Windows 10 or newer

## How to Compile

1. Open the solution in Visual Studio.
2. Restore NuGet packages (__Project > Restore NuGet Packages__).
3. Build the solution (__Build > Build Solution__ or press `Ctrl+Shift+B`).

## How to Run

- Press `F5` or click __Debug > Start Debugging__ to launch the app.
- Alternatively, use __Debug > Start Without Debugging__ (`Ctrl+F5`).

## How to Use

### Main Features

1. Report an Issue
   - Click "Report an Issue" in the main menu.
   - Fill in the title, select a category, add a description, and optionally attach media.
   - Click "Submit Report" to send your issue.

2. View Issues
   - Click "View Issues" in the main menu.
   - The `ViewIssueWindow` displays a table of all reported issues, including their details and status.

3. Status Report
   - Click "Status Report" to see a summary of all issues and their current status.

4. Announcements
   - Click "Announcements" to view important municipal updates.

5. Other Features
   - Earn points and badges for reporting issues.

