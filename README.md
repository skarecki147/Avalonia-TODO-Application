# Avalonia TODO Application

A full-featured task management web application built with **Avalonia UI WebAssembly** and **C#/.NET 9**.

## Features

### Authentication
- Login with full password validation (8+ chars, uppercase, number, special character)
- Show/hide password toggle
- "Remember me" with localStorage persistence
- Session auto-restore on page reload
- Logout with session cleanup

### Task Dashboard
- Full CRUD: Create, Read, Update, Delete tasks
- Paginated task list (10 per page)
- Real-time search filtering
- Sort by priority, due date, status, title, or creation date
- Filter by status and priority
- Bulk selection with multi-delete and priority change
- Move tasks to top/bottom for reordering
- Undo delete with snackbar notification
- Overdue task highlighting
- Empty state screen

### Statistics & Analytics
- Metric cards: total, completed, in-progress, overdue, completion rate
- Pie chart for status distribution (LiveCharts2)
- Pie chart for priority distribution
- Bar chart for weekly activity
- Completion progress bar

### UX & Theming
- Dark/Light theme toggle with localStorage persistence
- Global notification system (snackbar-style)
- Keyboard shortcut: Ctrl+N for new task
- Responsive layout
- Professional UI with consistent spacing and typography

## Tech Stack

| Layer          | Technology                              |
|----------------|----------------------------------------|
| Framework      | Avalonia UI 11.2.3                     |
| Language       | C# / .NET 9                            |
| MVVM           | CommunityToolkit.Mvvm                  |
| DI             | Microsoft.Extensions.DependencyInjection |
| Charts         | LiveCharts2 (SkiaSharp)                |
| Platform       | WebAssembly (browser-wasm)             |
| Hosting        | Vercel (static files)                  |

## Architecture

```
TodoApp.sln
├── src/TodoApp.Core/           # Domain: Models, Enums, Interfaces
├── src/TodoApp.Infrastructure/ # Service implementations (mock)
├── src/TodoApp.Shared/         # ViewModels, Views, Styles, Converters
└── src/TodoApp.Browser/        # WASM entry point + index.html
```

Clean MVVM with Dependency Injection. All business logic in ViewModels/Services, views are pure XAML data-binding.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- `wasm-tools` workload:
  ```bash
  dotnet workload install wasm-tools
  ```

## Run Locally

```bash
cd src/TodoApp.Browser
dotnet run
```

Then open the URL shown in the terminal (e.g. `http://localhost:5000`).

## Build for Production

```bash
dotnet publish src/TodoApp.Browser/TodoApp.Browser.csproj -c Release
```

Output: `src/TodoApp.Browser/bin/Release/net9.0-browser/publish/wwwroot/`

## Deploy to Vercel

1. Push this repo to GitHub
2. Import the repo in [Vercel](https://vercel.com)
3. Vercel will use `vercel.json` to build and deploy automatically
4. The COOP/COEP headers are configured for SharedArrayBuffer support (WASM threading)

## Login Credentials

This is a mock authentication system. Use **any username** with a password that meets the requirements:
- At least 8 characters
- Contains an uppercase letter
- Contains a number
- Contains a special character

Example: `Admin` / `Password1!`

## Project Structure Details

| Project | Purpose |
|---------|---------|
| `TodoApp.Core` | Domain models (`TodoItem`, `UserCredentials`), enums (`TodoStatus`, `TodoPriority`), service interfaces |
| `TodoApp.Infrastructure` | `MockAuthService`, `MockTodoService`, `StatisticsService`, `BrowserLocalStorageService` (JS interop) |
| `TodoApp.Shared` | ViewModels, AXAML Views, Converters, Styles, DI registration |
| `TodoApp.Browser` | WASM entry point, `index.html`, browser-specific config |

## License

MIT

