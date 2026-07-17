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
        - Supports color-coded header based on source
        - Supports color-coded messages based on log level
- Same whitelist/blacklist functionality for filtering sources as other sinks in the mLogger library


## Project Structure

```
mLogger_WinForms/
├── src/
│ └── Sinks_WinForms.cs # Logger implementation
└── mLogger_WinForms.csproj
```


## Usage

### Basic Example

VS generated forms will usually set each RichTextBox.text = "" during its InitializeComponent() method.  You must manually remove this line, or any logs written during startup are likely to be removed befor the control draws itself.

```csharp
/************************************
***     Basic Use                   *
************************************/
// Instantiate sinks and Logger
Logger logger = Logger.Instance;
logger.Instance.Initialize("MyApp");

RichTextBox tb = new RichTextBox();
RichTextBoxSink tbSink = new RichTextBoxSink(tb);

tbSink.AddSource("Startup", true, Color.LightBlue);
tbSink.AddSource("Config", false, Color.Magenta);
tbSink.AddSource("Database", true, new Color { A = 255, R = 128, G = 255, B = 128 } );

// Inject sink
logger.AddSink(tbSink);
                                                                                // Header Color       |    Message Color
                                                                                //   (Source)         | (Message's LogLevel)
logger.Info("Startup", "Application started successfully.");                    //   Light Blue       |        Black
logger.Debug("Startup_TimerModule", "timer readout:  63 seconds");              //   Light Blue       |        Blue
logger.Warn("Config", "Using default configuration.");                          //   Magenta          |        Orange
logger.Warn("Config_Parser", "Error during parsing.  Reverting to default");    //   Black (default)  |        Orange
logger.Error("Database", "Connection failed.");                                 //   Light Green      |        Red
logger.Info("NewSource", "Message from unregistered source");                   //   Black (default)  |        Black
```

### Log Levels
Level	Description                                        Color
DEBUG	Detailed information for debugging                  Blue
INFO	General application flow                            Black
WARN	Potential issue or unexpected condition             Orange
ERROR	Recoverable error                                   Red
FATAL	Critical failure                                    Dark Red

### Thread Safety

mLogger and RichTextBoxSink are designed to be thread-safe. Multiple threads can write logs concurrently without corrupting log output.

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