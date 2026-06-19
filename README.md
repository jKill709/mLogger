# mLogger

A lightweight, thread-safe logging library written in C# with unit tests.

This project was extracted from a WinForms application and refactored into a reusable class library suitable for use in any .NET application (console apps, services, desktop apps, etc.).


## Overview

`mLogger` provides a simple logging API for writing structured log messages with different severity levels. It is designed to be minimal, fast, and easy to integrate.

The library supports:

- Log levels (e.g., DEBUG, INFO, WARN, ERROR, FATAL)
- Thread-safe logging
- File-based output
- Simple singleton-style access pattern


## Project Structure

```
mLogger/
│
├── mLogger/ # Class library project
│ ├── src/
│ │ └── mLogger.cs # Logger implementation
│ ├── mLogger.csproj
│ └── mLogger.slnx
│
└── mLogger_UnitTests/ # Unit test project
├── src/
│ └── LoggerTests.cs
└── mLogger_UnitTests.csproj
```


## Usage

### Basic Example

```csharp
Logger.Instance.Initialize(
    "MyApp",
    @"C:\Logs\MyApp"
);

Logger.Instance.Info("Startup", "Application started successfully.");
Logger.Instance.Warn("Config", "Using default configuration.");
Logger.Instance.Error("Database", "Connection failed.");
```

### Log Levels

Level	Description
DEBUG	Detailed information for debugging
INFO	General application flow
WARN	Potential issue or unexpected condition
ERROR	Recoverable error
FATAL	Critical failure

### Thread Safety

mLogger is designed to be thread-safe. Multiple threads can write logs concurrently without corrupting log output.

### Running Unit Tests

From the solution root:

dotnet test

Or run tests directly from Visual Studio Test Explorer.

## Development Notes

This project was refactored from a WinForms application. During extraction, UI-specific dependencies were removed to ensure:

No dependency on System.Windows.Forms
No reliance on application UI context
Clean separation of concerns
Reusability across different application types
Future Improvements

### Planned enhancements:

Async logging pipeline
JSON structured log output
Configurable output targets (console, file, network)
Dependency injection support

This project is provided as-is for portfolio and educational use.

## License

This project is licensed under the MIT License - see the LICENSE file for details.