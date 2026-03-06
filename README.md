Document Manager
WPF + C# - Document Manager (MVVM)
Goal
Build a WPF application using MVVM architecture to manage documents added by the user.
1) Main Window
The application will contain a TabControl with two tabs:
Tab 1 - Documents: Displays all documents added to the system. Each document should show: Name, Full Path,
Extension/Type, Size, Created/Modified (if available), and AddedAt.
Action: 'Add' button to open a new window.
Tab 2 - History: Displays a history of files loaded into the system. Managed via JSON, loaded on startup, and updated
with every file load.
Action: 'Clear History' button to delete history and reset the JSON file.
2) Add Document Window
Includes:
- Browse button for file selection.
- Drag & Drop area (Drag file here).
After selection, collect and display: Name, Type, Size, Path, and any other relevant info.
Buttons: Confirm (adds document) and Cancel (closes window).
Validation: If the file type is unsupported, show an error and disable Confirm.
3) Configuration File
A JSON configuration file defining at least:
- Supported extensions.
- Central storage directory path for the application's documents.
4) File Saving (Important)
Every file added is copied to the defined storage directory. The app manages files from this directory only. On startup,
the system scans this directory to load and display documents.
5) Persistency
Save info in local JSON:
- Documents: List of documents in the system.
- History: List of files added over time.
Data is loaded from JSON on app launch.
6) Logging
Implement a logging system that writes to a file. Actions include: App started, Documents loaded, Add window opened,
File selected/dropped, Document added, Unsupported file type, Clear history, Errors.

Document Manager
Log must include: Timestamp and Level (Info/Warning/Error).
7) Real-Time File System Monitoring
Implement a background component or service that monitors the Central Storage directory for external changes:
- Change Detection: The app must detect in real-time if a file is added or deleted directly via the OS (e.g., Windows
Explorer) rather than the app UI.
- UI Update: When a change is detected, the document list in Tab 1 must update automatically without manual refresh.
- Thread Safety: Ensure UI updates are handled safely (Thread-safe) to prevent application crashes (e.g., using the
Dispatcher).
- Logging: Any external change detected (addition/deletion) must be logged in the system log (as per Section 6).
8) Technical Requirements
Use MVVM principles, no business logic in code-behind, use Commands for actions. Separate into layers: Views,
ViewModels, Models, Services. Ensure proper error handling and clean, readable code.
