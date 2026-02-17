# Avalonia TODO Application

A full-featured task management web application built with **Avalonia UI 11.3 WebAssembly** and **C#/.NET 9**. Runs entirely in the browser as a static site.

**Live demo:** Deployed via Vercel with GitHub Actions CI/CD.

## Features

### Authentication
- Login with password validation (8+ chars, uppercase, number, special character)
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
- Responsive layout for mobile, tablet, and desktop viewports
- Professional UI with consistent spacing and typography

## Tech Stack

| Layer          | Technology                                |
|----------------|------------------------------------------|
| Framework      | Avalonia UI 11.3.12                      |
| Language       | C# / .NET 9                              |
| MVVM           | CommunityToolkit.Mvvm 8.2.2              |
| DI             | Microsoft.Extensions.DependencyInjection  |
| Charts         | LiveCharts2 (SkiaSharp)                   |
| Icons          | Material.Icons.Avalonia                   |
| Platform       | WebAssembly (browser-wasm)                |
| CI/CD          | GitHub Actions                            |
| Hosting        | Vercel (static files)                     |

## Architecture

```
TodoApp.sln
├── src/TodoApp.Core/           # Domain models, enums, service interfaces
├── src/TodoApp.Infrastructure/ # Mock service implementations, JSON context
├── src/TodoApp.Shared/         # ViewModels, Views (AXAML), Converters, Styles, DI
└── src/TodoApp.Browser/        # WASM entry point, index.html
```

Clean MVVM with Dependency Injection. All business logic lives in ViewModels and Services; Views are pure XAML with data-binding.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- `wasm-tools` workload:
  ```bash
  dotnet workload install wasm-tools
  ```
- A static file server (e.g. `http-server` via npm)

## Run Locally

Avalonia WASM apps must be **published** and served as static files. `dotnet run` is not supported for the browser target.

**1. Publish the app:**

```bash
dotnet publish src/TodoApp.Browser/TodoApp.Browser.csproj -c Release
```

**2. Serve the output with any static file server:**

```bash
npx http-server src/TodoApp.Browser/bin/Release/net9.0-browser/publish/wwwroot -p 8080 --cors -c-1
```

**3. Open in browser:**

Navigate to `http://127.0.0.1:8080`

> **Tip:** After making changes, re-run `dotnet publish` and hard-refresh the browser (`Ctrl+Shift+R`).

## Build for Production

```bash
dotnet publish src/TodoApp.Browser/TodoApp.Browser.csproj -c Release
```

Output directory: `src/TodoApp.Browser/bin/Release/net9.0-browser/publish/wwwroot/`

## Deploy to Vercel

Deployment is automated via **GitHub Actions** (not Vercel's built-in build):

1. Push to the `main` branch
2. GitHub Actions builds the .NET WASM app and deploys the static output to Vercel
3. COEP/COOP headers are configured automatically for `SharedArrayBuffer` support (required by SkiaSharp)

**Required GitHub secrets:**
- `VERCEL_TOKEN`
- `VERCEL_ORG_ID`
- `VERCEL_PROJECT_ID`

See `.github/workflows/deploy-vercel.yml` for the full pipeline.

## Login Credentials

This is a mock authentication system. Use **any username** with a password that meets the requirements:
- At least 8 characters
- Contains an uppercase letter
- Contains a number
- Contains a special character

Example: `Admin` / `Password1!`

## Project Structure

| Project | Purpose |
|---------|---------|
| `TodoApp.Core` | Domain models (`TodoItem`, `UserCredentials`), enums (`TodoStatus`, `TodoPriority`), service interfaces |
| `TodoApp.Infrastructure` | `MockAuthService`, `MockTodoService`, `StatisticsService`, `BrowserLocalStorageService` (JS interop), `TodoJsonContext` (source-gen JSON) |
| `TodoApp.Shared` | ViewModels, AXAML Views, Converters, Styles, DI registration (`ServiceCollectionExtensions`) |
| `TodoApp.Browser` | WASM entry point, `index.html`, browser-specific config |

## License

MIT

