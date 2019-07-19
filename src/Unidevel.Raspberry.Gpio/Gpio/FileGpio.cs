using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace Unidevel.Raspberry.Gpio
{
    public class FileGpio : IGpio, IDisposable
    {
        public bool this[int gpioPinNumber] { get => read(gpioPinNumber); set => write(gpioPinNumber, value); }

        public FileGpio(ILogger<FileGpio> logger = null)
        {
            this.logger = logger;
        }

        public void Dispose()
        {
            var pins = gpioPinNumberInstances.Keys;
            gpioPinNumberInstances = null;

            if (pins != null) foreach (var pin in pins) unexportPin(pin);
        }

        private enum Direction
        {
            Undefined,
            Input,
            Output
        }

        private class PinInstance
        {
            public int Pin { get; set; }
            public Direction Direction { get; set; }
        }

        private const string gpioPath = "/sys/class/gpio/";
        private readonly ILogger<FileGpio> logger;
        private ConcurrentDictionary<int, PinInstance> gpioPinNumberInstances = new ConcurrentDictionary<int, PinInstance>();

        private bool read(int gpioPinNumber)
        {
            var pinInstance = ensurePin(gpioPinNumber);

            lock (pinInstance)
            {
                ensureDirection(pinInstance, Direction.Input);

                var value = read($"{gpioPath}gpio{gpioPinNumber}/value");
                return (value.Length > 0 && value[0] == '1');
            }
        }

        private void write(int gpioPinNumber, bool value)
        {
            var pinInstance = ensurePin(gpioPinNumber);

            lock (pinInstance)
            {
                ensureDirection(pinInstance, Direction.Output);

                write($"{gpioPath}gpio{gpioPinNumber}/value", value ? "1" : "0");
            }
        }

        private PinInstance ensurePin(int gpioPinNumber)
        {
            if (gpioPinNumberInstances == null) throw new ObjectDisposedException("Unable to access GPIO through disposed object. Use it as singleton.");
            if ((gpioPinNumber < 1) || (gpioPinNumber > 32)) throw new ArgumentOutOfRangeException("GPIO pin number must be between 1-32.");

            return gpioPinNumberInstances.GetOrAdd(gpioPinNumber, p => { exportPin(gpioPinNumber); return new PinInstance() { Pin = gpioPinNumber, Direction = Direction.Undefined }; });
        }

        private void ensureDirection(PinInstance pinInstance, Direction direction)
        {
            if (pinInstance.Direction != direction)
            {
                switchDirection(pinInstance.Pin, direction);
                pinInstance.Direction = direction;
            }
        }

        private void exportPin(int gpioPinNumber)
        {
            if (!Directory.Exists($"{gpioPath}gpio{gpioPinNumber}")) write($"{gpioPath}export", $"{gpioPinNumber}");            
        }
        
        private void unexportPin(int gpioPinNumber)
        {
            if (Directory.Exists($"{gpioPath}gpio{gpioPinNumber}")) write($"{gpioPath}unexport", $"{gpioPinNumber}");
        }

        private void switchDirection(int gpioPinNumber, Direction direction)
        {
            string directionString;

            switch (direction)
            {
                case Direction.Input:
                    directionString = "in";
                    break;
                case Direction.Output:
                    directionString= "out";
                    break;
                default:
                    throw new NotSupportedException($"Unable to set PIN direction to other then {nameof(Direction.Input)} or {nameof(Direction.Output)}.");
            }

            write($"{gpioPath}gpio{gpioPinNumber}/direction", directionString);
        }

        private void write(string path, string value)
        {
            logger?.LogDebug($"GPIO write {path} << {value}");
            File.WriteAllText(path, value);
        }

        private string read(string path)
        {
            var value = File.ReadAllText(path);
            logger?.LogDebug($"GPIO read {path} >> {value}");
            return value;
        }
    }
}
