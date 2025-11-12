# MunicipalityApp

 A modern WPF desktop application for reporting, viewing, and tracking municipal issues. Built with .NET 8, it features a polished UI, a splash/landing screen, toast notifications, in-memory data storage, and smart event recommendations.

---

## Table of Contents
- [Overview](#overview)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Build and Run](#build-and-run)
- [Application Tour](#application-tour)
- [Data & Architecture](#data--architecture)
- [Styling & Theming](#styling--theming)
- [Known Limitations](#known-limitations)
- [Troubleshooting](#troubleshooting)
- [Contributing](#contributing)
- [License](#license)

---

## Overview
MunicipalityApp enables citizens to report issues (e.g., water, electricity, roads), view announcements and local events, and track their submissions. Data is stored in-memory using thread-safe collections for demonstration and coursework purposes.

Entry point is configured via `App.xaml` with `StartupUri="LandingPage.xaml"`, which displays a splash experience before navigating into the main UI.

---

## Features
- **Report Issues**: Submit a title, category, description, and optional media attachment in `ReportIssueWindow`.
- **Progress Feedback**: Animated progress bar and contextual coloring during report composition.
- **Announcements & Events**: `AnnouncementsWindow` lists events generated with Bogus, supports search, location filters, and recommendations.
- **Status Reporting**: `StatusReportPage` (or `StatusReportWindow`) summarizes your submitted issues (newest first).
- **View Issues**: `ViewIssueWindow` displays issues in a data grid snapshot.
- **Toast Notifications**: Lightweight, timed notifications implemented in `ToastNotification`.
- **User Points & Badges**: `UserRepository.CurrentUser` accrues points for submissions and surfaces badge status in success toasts.
- **Polished UI**: Centralized theme colors, styles, and effects defined in `App.xaml` resources.

---

## Tech Stack
- **Framework**: .NET 8 (`net8.0-windows`) with WPF
- **IDE**: Visual Studio 2022+
- **OS**: Windows 10+
- **Packages**: `Bogus` for sample data generation

See project file: `MunicipalityApp/MunicipalityApp.csproj` for details.

---

## Project Structure
```text
MunicipalityApp/
├─ MunicipalityApp.sln
├─ README.md  ← You are here
└─ MunicipalityApp/
   ├─ App.xaml, App.xaml.cs                  ← Startup, global resources, theme
   ├─ LandingPage.xaml                       ← Splash/landing experience
   ├─ MainWindow.xaml(.cs)                   ← Main navigation menu
   ├─ AnnouncementsWindow.xaml(.cs)          ← Events list, search, filters, recommendations
   ├─ ReportIssueWindow.xaml(.cs)            ← Issue submission flow with progress bar
  ├─ StatusReportPage.xaml(.cs)             ← Summary list of submitted issues (Page-in-Frame recommended)
   ├─ ViewIssueWindow.xaml(.cs)              ← Data grid snapshot of issues
   ├─ ToastNotification.xaml(.cs)            ← In-app toast messages
   ├─ Converters/
   │  └─ StringNullOrEmptyToVisibilityConverter.cs
   ├─ Data/
   │  ├─ IssueRepository.cs                  ← Thread-safe in-memory store
   │  └─ UserRepository.cs                   ← Current user profile (points, badges)
   ├─ Models/
   │  ├─ Event.cs
   │  ├─ Issue.cs
   │  ├─ UserIssue.cs
   │  └─ UserProfile.cs
   ├─ Assets/                                 ← Images (e.g. SA-coat-of-arms.png)
   └─ MunicipalityApp.csproj
```

---

## Getting Started
- **Prerequisites**
  - [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
  - [Visual Studio 2022 or newer](https://visualstudio.microsoft.com/)
  - Windows 10 or newer

- **Clone**
  - Open this folder directly or clone into `c:\Users\<you>\source\repos\`.

---

## Build and Run
- **Visual Studio**
  - Open `MunicipalityApp.sln`.
  - Restore NuGet packages (Project > Restore NuGet Packages).
  - Build (Build > Build Solution or Ctrl+Shift+B).
  - Run (F5 or Debug > Start Debugging). Use Ctrl+F5 to run without debugger.

- **.NET CLI** (optional)
  - From `MunicipalityApp/` project directory:
    - `dotnet restore`
    - `dotnet build`
    - `dotnet run`

---

## Application Tour
- **LandingPage** (`LandingPage.xaml`)
  - Splash screen with animations and theming defined in `App.xaml`.

- **MainWindow** (`MainWindow.xaml.cs`)
  - Navigation buttons for Home, Report Issue, Announcements, Status Report, and Exit.
  - Uses `ToastNotification` for quick feedback.

- **ReportIssueWindow** (`ReportIssueWindow.xaml.cs`)
  - Fields: Title, Category, Description, optional media (`OpenFileDialog`).
  - Animated progress bar updates as fields are filled. Color shifts between Red, Orange, Green by completion.
  - On submit: creates a `UserIssue`, stores via `IssueRepository.AddIssue(issue)`, awards points with `UserRepository.CurrentUser.AddPoints(10)`, shows a success toast, and resets form.

- **AnnouncementsWindow** (`AnnouncementsWindow.xaml.cs`)
  - Populates sample `Event` data using `Bogus` with categories and locations.
  - Live search on title/category/location and ComboBox location filter.
  - Builds recommendations based on search history, category affinity, and priority, sorted by priority then date.

- **StatusReportWindow** (`StatusReportWindow.xaml.cs`)
  - Loads issues from `IssueRepository` and lists newest first via `GetIssuesNewestFirst()`.

- **ViewIssueWindow** (`ViewIssueWindow.xaml.cs`)
  - Displays a snapshot of `IssueRepository.Issues` in a `DataGrid`.

- **MessageCenterWindow** (optional utility)
  - Shows example in-app messages list.

---

## Data & Architecture
- **Repositories**
  - `IssueRepository` stores `UserIssue` items in multiple collections:
    - LinkedList for insertion order
    - Stack for recent issues (LIFO)
    - Queue for processing (FIFO)
    - All access guarded by a lock for thread-safety; exposes snapshots via `ToArray()`.
  - `UserRepository` exposes a static `CurrentUser` (`UserProfile`) to track points/badges.

- **Models**
  - `UserIssue`, `Issue`, `Event`, `UserProfile` represent domain entities.

- **Notifications**
  - `ToastNotification` is a lightweight `Window` positioned bottom-right and self-closes after 3 seconds using `DispatcherTimer`.

- **Converters**
  - `StringNullOrEmptyToVisibilityConverter` for watermark visibility in text boxes.

- **Persistence**
  - In-memory only. Restarting the app resets data. Suitable for demos and assignments; see [Known Limitations](#known-limitations).

---

## Styling & Theming
- Centralized in `App.xaml` resources:
  - Colors and brushes: `PrimaryDarkTealBrush`, `AccentYellowBrush`, etc.
  - Styles for buttons (`PrimaryButton`, `AccentButton`, `NavButtonStyle`, `ExitButtonStyle`) and list items.
  - Effects such as `DropShadowEffect`.
  - Vector-based `AppLogo` drawing.

---

## Known Limitations
- **No persistent storage**: All data resets on application restart.
- **No authentication**: `UserRepository.CurrentUser` assumes a single anonymous session.
- **Single-process scope**: Repositories are static and not shared across processes.

---

## Troubleshooting
- **Build fails on Windows target**: Ensure the project targets `net8.0-windows` and you have the .NET 8 SDK installed.
- **Images not loading**: Verify `SA-coat-of-arms.png` exists and is included as a Resource in the `.csproj`.
- **Bogus not restored**: Run `dotnet restore` or Restore NuGet Packages in Visual Studio.
- **App doesn’t start at landing page**: Confirm `StartupUri="LandingPage.xaml"` in `App.xaml`.
