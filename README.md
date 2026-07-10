# mLogger

A lightweight, thread-safe logging library written in C# with unit tests.

This project was extracted from a WinForms application and refactored into a reusable class library suitable for use in any .NET application (console apps, services, desktop apps, etc.).


## Overview

`mLogger` provides a simple logging API for writing structured log messages with different severity levels, to multiple log sinks. It is designed to be minimal, fast, and easy to integrate.

The library supports:

- Log levels (e.g., DEBUG, INFO, WARN, ERROR, FATAL)
- Thread-safe logging
- Built in sinks
    - Text file
    - Console output
    - Entries Stored in RAM
- Additional Sinks available mLogger_WinForms
    - TextBox
- Simple singleton-style access pattern


## Project Structure

```
mLogger/
│
├── mLogger/ # Class library project
│ ├── src/
│ │ ├── mLogger.cs # Logger implementation
│ │ └── Sinks.cs # Sinks interface and implementations
│ ├── mLogger.csproj
│ └── mLogger.slnx
│
├── mLogger_UnitTests/ # Unit test project
├── src/
│ ├── InMemorySinkTests.cs
│ ├── LoggerTests.cs
│ └── TextFileSinkTests.cs
├── mLogger_UnitTests.csproj
│
├── mLogger_WinForms/ # WinForms Class library project
│ ├── src/
│ │ └── Sinks_WinForms.cs # Logger implementation
│ └── mLogger_WinForms.csproj
│
├── mLogger_WinForms_UnitTests/ # Unit test project
├── src/
│ └── TextBoxSinkTests.cs
├── mLogger_WinForms_UnitTests.csproj
└── MSTestSettings.cs
```


## Usage

### Basic Example

```csharp
TextFileSink textSink = new ("C:/Logs/", "MyApp");
MemorySink memSink = new ();
Logger.Instance.Initialize("MyApp");
Logger.AddSink(textSink);
Logger.AddSink(memSink);

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

## License

This project is licensed under the MIT License - see the LICENSE file for details.