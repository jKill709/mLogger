# mLogger

A lightweight, thread-safe logging library written in C# to extend the mLogger library so that it can be used with WinForms applications, complete with unit tests.

This project was extracted from a WinForms application and refactored into a reusable class library suitable for use in any .NET application (console apps, services, desktop apps, etc.).


## Overview

`mLogger_WinForms` provides logging sinks for WinForms applications. It is designed to be minimal, fast, and easy to integrate.

The library supports:

- Log levels (e.g., DEBUG, INFO, WARN, ERROR, FATAL)
- Thread-safe logging
- Built in sinks
    - RichTextBoxSink for logging to a RichTextBox control
- Same whitelist/blacklist functionality for filtering sources


## Project Structure

```
mLogger_WinForms/
├── src/
│ └── Sinks_WinForms.cs # Logger implementation
└── mLogger_WinForms.csproj
```


## Usage

### Basic Example

```csharp
/************************************
***     Basic Use                   *
************************************/
// Instantiate sinks and Logger
Logger logger = Logger.Instance;
logger.Instance.Initialize("MyApp");

RichTextBox tb = new RichTextBox();
RichTextBoxSink tbSink = new RichTextBoxSink(tb);

// Inject sink
logger.AddSink(tbSink);

logger.Info("Startup", "Application started successfully.");
logger.Warn("Config", "Using default configuration.");
logger.Error("Database", "Connection failed.");
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