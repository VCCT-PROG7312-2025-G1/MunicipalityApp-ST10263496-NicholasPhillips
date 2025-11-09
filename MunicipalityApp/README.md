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
   - Use the "Search by ID" box to paste a request GUID (shown in the list) and press Find to jump to it.
   - Press "Refresh Indexes" to rebuild advanced data structure indexes.
   - Each item shows: `Id`, `Title`, `Category`, `ReportedDate`, `Status`, and a `Progress` bar.

4. Announcements
   - Click "Announcements" to view important municipal updates.

5. Other Features
   - Earn points and badges for reporting issues.

## Data Structures in Service Request Status

To achieve efficient organisation and retrieval, `StatusReportWindow` uses indexes built by `Services/ServiceRequestIndex.cs` over in-memory issues from `Data/IssueRepository.cs`.

- **Binary Search Tree (BST)** (`DataStructures/BinarySearchTree.cs`)
  - Keyed by `Guid Id` for O(log N) average lookups when searching requests by unique identifier.
  - Used by `ServiceRequestIndex.TryFindById()`.

- **AVL Tree** (`DataStructures/AvlTree.cs`)
  - Balanced BST keyed by a composite time key of `ReportedDate` and `Id`.
  - Provides in-order traversal by reported date for near-real-time sorted listings.

- **Red-Black Tree** (`DataStructures/RedBlackTree.cs`)
  - Balanced tree keyed by `Title` to demonstrate alternative ordered indexing and fast retrieval by title.

- **Min-Heap** (`DataStructures/MinHeap.cs`)
  - Orders items by `Progress` (then `ReportedDate`) to fetch the lowest-progress (earliest-stage) requests efficiently.
  - Demonstrates prioritisation for escalation and triage views.

- **Graph + Traversal + MST** (`DataStructures/Graph.cs`)
  - Nodes represent categories present in the dataset; edges connect consecutive categories for demo purposes.
  - Supports BFS traversal and a Prim-based Minimum Spanning Tree for showcasing graph operations.

### Where it is used

- **Repository:** `Data/IssueRepository.cs` stores `UserIssue` items and exposes `Issues` and `GetIssuesNewestFirst()`.
- **Index:** `Services/ServiceRequestIndex.cs` builds the above structures for lookups and traversals.
- **UI:** `StatusReportWindow.xaml` binds to `UserIssue` items and uses search/refresh handlers in `StatusReportWindow.xaml.cs`.

## Notes

- Each new `UserIssue` is assigned a unique `Id` automatically (`Models/UserIssue.cs`).
- `Progress` starts at `0` and can be updated by future workflow steps to reflect lifecycle progress.

