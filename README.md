# DateTime Handy Conversions
An extension class for DateTime and DateTimeOffset to make conversions easy

The class contains some handy methods to convert between DateTime and DateTimeOffset or changing a date-time time zone.
If you are working on a project which employs both DateTime and DateTimeOffset it is very useful. 


## Usage
If you want to get a time offset by time zone's name
```csharp
var pacificOffset = DateExtension.GetTimeZoneOffset("Pacific Standard Time");    // Retuens -8
```



For convering a date and time to different time zone, use one of the `convertByTimeZone()` methods
```csharp
var currentUtc = DateTimeOffset.Parse("2022-11-06 08:00 +00:00");
var dateInLosAngles = DateExtensions.ConvertByTimeZone(currentUtc, "Pacific Standard Time");
```

You can find the time difference between 2 time zones easily:
```csharp
var utcDiffToPacific = DateExtensions.GetTimeZoneOffset("UTC", "Pacific Standard Time");
```
