# mLogger - Unit Tests

Unit testing suite for the mLogger library, ensuring correctness and reliability of logging functionality across different sinks and scenarios.


## Overview


```
mLogger_UnitTests/
├── src/
│ ├── InMemorySinkTests.cs
│ ├── LoggerTests.cs
│ └── TextFileSinkTests.cs
└── mLogger_UnitTests.csproj
```
## Testing

### InMemorySinkTests

	- WriteLine_ShouldStoreFormattedMessage
	- WriteHeading_ShouldStoreFormattedMessage
	- Reset_ShouldClearLogs


### LoggerTests

	- Info_ShouldSendEntryToSink
	- WriteLine_ShouldStoreFormattedMessage
	- WriteHeading_ShouldStoreFormattedMessage
	- WriteSeparator_ShouldStoreSeparator
	- Log_ShouldUseCorrectLevel
	- MultipleSinks_ShouldReceiveMessage
	- RemoveSink_ShouldStopReceivingMessages
	- BlacklistPattern_ShouldHaveNoFalsePositives
	- BlacklistPattern_ShouldHaveNoFalseNegatives
	- WhitelistPattern_ShouldHaveNoFalsePositives
	- WhitelistPattern_ShouldHaveNoFalseNegatives
	- BlacklistSourceWithoutModules_ShouldHaveNoFalsePositives
	- BlacklistSourceWithoutModules_ShouldHaveNoFalseNegatives
	- WhitelistSourceWithoutModules_ShouldHaveNoFalsePositives
	- WhitelistSourceWithoutModules_ShouldHaveNoFalseNegatives
	- BlacklistSourceWithModules_ShouldHaveNoFalsePositives
	- BlacklistSourceWithModules_ShouldHaveNoFalseNegatives
	- WhitelistSourceWithModules_ShouldHaveNoFalsePositives
	- WhitelistSourceWithModules_ShouldHaveNoFalseNegatives
	- useList_ShouldEnableBlackListWhenTrue
	- useList_ShouldDisableBlackListWhenFalse
	- useList_ShouldEnableWhitelistWhenTrue
	- useList_ShouldDisableWhitelistWhenFalse

### TextFileSinkTests

	- Write_ShouldCreateFile
	- Write_ShouldWriteMessage
	- WriteHeading_ShouldWriteHeaderFormatting
	- Reset_ShouldClearFile


### License

This project is licensed under the MIT License - see the LICENSE file for details.