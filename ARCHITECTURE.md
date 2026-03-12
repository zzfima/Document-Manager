# Document Manager - Architecture Summary
## School Project for Eden - Beit Sefer Darski

## Overview
A WPF desktop application for managing documents using the **MVVM (Model-View-ViewModel)** architectural pattern.

---

## Project Structure

```
DocumentManager/
├── Models/                 # Data classes
│   ├── Document.cs         # Document information
│   ├── HistoryEntry.cs     # History record
│   └── TemplateDocument.cs  # Base class for Document and HistoryEntry
│
├── Services/               # Business logic layer
│   ├── IConfigService.cs / ConfigService.cs      # Reads appsettings.json
│   ├── ILoggingService.cs / LoggingService.cs    # Writes to log file
│   ├── IDocumentService.cs / DocumentService.cs  # Manages documents
│   ├── IHistoryService.cs / HistoryService.cs    # Manages history
│   ├── IFileWatcherService.cs / FileWatcherService.cs  # Monitors folder
│   └── ServiceLocator.cs   # Dependency injection container
│
├── ViewModels/             # Presentation logic (no UI code here)
│   ├── ViewModelBase.cs    # Base class with INotifyPropertyChanged
│   ├── MainViewModel.cs    # Logic for main window
│   └── AddDocumentViewModel.cs  # Logic for add document window
│
├── Views/                  # UI layer (XAML + minimal code-behind)
│   ├── MainWindow.xaml/.cs
│   └── AddDocumentWindow.xaml/.cs
│
├── Converters/             # Value converters for XAML bindings
│   └── StringToVisibilityConverter.cs
│
├── App.xaml/.cs            # Application entry point
└── appsettings.json        # Configuration file
```

---

## Requirements Covered

| # | Requirement | Implementation |
|---|-------------|----------------|
| 1 | TabControl with Documents + History | MainWindow.xaml - TabControl |
| 2 | Add Document with Browse + Drag&Drop | AddDocumentWindow.xaml |
| 3 | JSON configuration file | appsettings.json |
| 4 | File copying to storage directory | DocumentService.cs |
| 5 | JSON persistency | documents.json, history.json |
| 6 | Logging with timestamp + level | LoggingService.cs |
| 7 | Real-time file monitoring | FileWatcherService.cs |
| 8 | MVVM pattern with Commands | ViewModels + MvxCommand |
| 9 | Status bar with timestamps | MainWindow.xaml - StatusBar |

---

## MVVM Pattern Explained

```
┌─────────────────────────────────────────────────────────────┐
│                         VIEW                                 │
│  MainWindow.xaml / AddDocumentWindow.xaml                   │
│  - XAML markup (buttons, lists, etc.)                       │
│  - Data binding: {Binding PropertyName}                     │
│  - Command binding: Command="{Binding CommandName}"         │
└─────────────────────────────────────────────────────────────┘
                              ↕ Data Binding
┌─────────────────────────────────────────────────────────────┐
│                      VIEWMODEL                               │
│  MainViewModel.cs / AddDocumentViewModel.cs                 │
│  - Properties (Documents, History, FileName, etc.)          │
│  - Commands (AddDocumentCommand, ClearHistoryCommand)       │
│  - Calls Services to do actual work                         │
└─────────────────────────────────────────────────────────────┘
                              ↕ Method calls
┌─────────────────────────────────────────────────────────────┐
│                       SERVICES                               │
│  DocumentService, HistoryService, LoggingService, etc.      │
│  - Business logic (copy files, save JSON, write logs)       │
│  - No UI code here                                          │
└─────────────────────────────────────────────────────────────┘
```

---

## Key Concepts

### 1. Data Binding
```xml
<!-- In XAML -->
<TextBlock Text="{Binding FileName}"/>
<Button Command="{Binding AddDocumentCommand}"/>
<ListView ItemsSource="{Binding Documents}"/>
```
When ViewModel property changes → UI updates automatically.

### 2. Commands (Instead of Click Events)
```csharp
// In ViewModel
public IMvxCommand AddDocumentCommand { get; }
AddDocumentCommand = new MvxCommand(OpenAddDocumentWindow);
```
Buttons bind to Commands, not event handlers.

### 3. ObservableCollection
```csharp
public ObservableCollection<Document> Documents { get; set; }
Documents.Add(newDocument);  // UI updates automatically!
```

### 4. Dependency Injection
```csharp
// Services are passed to ViewModel constructor
public MainViewModel(IDocumentService documentService, ILoggingService loggingService)
{
    _documentService = documentService;
    _loggingService = loggingService;
}
```

### 5. Status Bar Bindings
```xml
<!-- In MainWindow.xaml -->
<StatusBar>
    <TextBlock Text="Created At: "/>
    <TextBlock Text="{Binding CreatedAt}" FontWeight="Bold"/>
    <Separator/>
    <TextBlock Text="Modified At: "/>
    <TextBlock Text="{Binding ModifiedAt}" FontWeight="Bold"/>
</StatusBar>
```

### 6. Thread Safety (Dispatcher)
```csharp
// FileWatcher runs on background thread
// Check if already on UI thread before invoking
if (Application.Current.Dispatcher.CheckAccess())
{
    HandleFileChange(e);  // Already on UI thread
}
else
{
    Application.Current.Dispatcher.Invoke(() => HandleFileChange(e));
}
```

### 7. File Change Types
```csharp
public enum FileChangeType
{
    Created,
    Deleted,
    Renamed
}
```

---

## Data Flow Example: Adding a Document

```
1. User clicks "Add Document" button
      ↓
2. Button Command="{Binding AddDocumentCommand}" triggers
      ↓
3. MainViewModel.OpenAddDocumentWindow() runs
      ↓
4. Event raised → MainWindow opens AddDocumentWindow
      ↓
5. User selects file → AddDocumentViewModel.SelectedFilePath set
      ↓
6. User clicks "Confirm" → ConfirmCommand triggers
      ↓
7. Event raised → MainViewModel.AddDocument(filePath) called
      ↓
8. DocumentService.AddDocument() copies file + saves JSON
      ↓
9. Documents.Add() → UI list updates automatically
```

---

## Files and Their Purpose

| File | Purpose |
|------|---------|
| `appsettings.json` | Configuration (storage path, allowed extensions) |
| `documents.json` | List of documents in the system |
| `history.json` | History of all file operations |
| `Logs/app_*.log` | Application log file |

---

## Dependencies

- **.NET 8.0 Windows** - Framework
- **MvvmCross** - MVVM framework (Commands, ViewModelBase)
- **Newtonsoft.Json** - JSON file reading/writing

---

## Design Patterns Used

| Pattern | Where Used | Explanation |
|---------|------------|-------------|
| **MVVM** | Entire app | Separates UI from logic |
| **Observer** | PropertyChanged | UI reacts to data changes |
| **Command** | Buttons | Actions without code-behind |
| **Dependency Injection** | ServiceLocator | Services created in one place |
| **Singleton** | Services | One instance shared everywhere |

---

## Summary

This application demonstrates clean MVVM architecture:
- **Views** = What user sees (XAML)
- **ViewModels** = Logic and data for views (C#)
- **Models** = Data structures (C#)
- **Services** = Business operations (C#)

No business logic in code-behind files - everything goes through ViewModels and Services!
