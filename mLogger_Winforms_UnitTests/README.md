# mLogger - Unit Tests

Unit testing suite for the mLogger_WinForms library, ensuring correctness and reliability of logging functionality across different sinks and scenarios.


## Overview


```
mLogger_WinForms_UnitTests/
├── src/
│ └── TextBoxSinkTests.cs
├── mLogger_WinForms_UnitTests.csproj
└── MSTestSettings.cs
```
## Testing

### TextBoxSinkTests

	- Constructor_StoresRichTextBox
	- Constructor_ThrowsArgumentNullException_WhenRichTextBoxIsNull
	- Write_AppendsOneLine
	- Write_AppendsSource
	- Write_AppendsMessage
	- Write_AppendsMultipleLines
	- ResetForTesting_ClearsText
	- PendingQueue_DequeuesPoroperly


### License

This project is licensed under the MIT License - see the LICENSE file for details.