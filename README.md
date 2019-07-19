## Synopsis

Raspberry PI library for controlling GPIO inputs and outputs with minimal interface.

## Code Example

```csharp
IGpio gpio = new FileGpio();

gpio[2] = true; // sets GPIO 2 to HIGH
gpio[5] = false; // sets GPIO 5 to LOW

var b = gpio[4]; // gets value of GPIO 4`
```

Complete interface is:

```csharp
public interface IGpio
{
    /// <summary>
    /// Sets PIN to high (true) or low (false).
    /// </summary>
    /// <param name="gpioPinNumber">Pin to set or get value from. Must be between 1 and 32, inclusive.</param>
    /// <returns>Pin value.</returns>
    bool this[int gpioPinNumber] { get;set; }
}
```

## Motivation

Initially developed for home automation after several bad experiences with other libraries (failures on signals etc.). 
This one is absolutely simple and has minimal dependencies. Tasks related to switching PIN mode are handled internally. Also
concurrency should be quite well handled BUT singleton instance of FileGpio() class should be created (use dependency injection, please).

## Installation

Use NuGet.

## API Reference

Look at code example should be enough.

## Tests

Describe and show how to run the tests with code examples.

## Contributors

Every contributor is welcome here. But keep it simple.

## License

I like MIT licence for my work, so this one will be used.
