﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using SimpleLed;

namespace Source.MadMeters
{
    public class MadMetersProvider : ISimpleLed
    {
        private ControlDevice.LedUnit[] leds = new ControlDevice.LedUnit[15];
        PerformanceCounter cpuCounter;
        PerformanceCounter ramCounter;


        public void Dispose()
        {

        }

        public void Configure(DriverDetails driverDetails)
        {
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        }

        public List<ControlDevice> GetDevices()
        {
            for (int i = 0; i < 15; i++)
            {
                leds[i] = new ControlDevice.LedUnit
                {
                    Data = new ControlDevice.LEDData { LEDNumber = i },
                    LEDName = "LED " + (i + 1)
                };
            }

            return new List<ControlDevice>
            {
                new ControlDevice
                {
                    LEDs = leds,
                    DeviceType = DeviceTypes.Effect,
                    Name = "CPU Usage",
                    Driver = this,
                    ProductImage = Assembly.GetExecutingAssembly().GetEmbeddedImage("Source.MadMeters.madmeter.png")

                }
            };
        }

        public void Push(ControlDevice controlDevice)
        {

        }

        private float cpu = 0;
        private float cpulerp = 0;
        DateTime lastRun=DateTime.MinValue;
        public void Pull(ControlDevice controlDevice)
        {
            if ((DateTime.Now - lastRun).TotalMilliseconds > 200)
            {
                cpu = cpuCounter.NextValue();
                lastRun = DateTime.Now;
            }

            if (cpulerp < cpu)
            {
                cpulerp = cpulerp + ((cpu - cpulerp) * 0.2f);
            }

            if (cpulerp > cpu)
            {
                cpulerp = cpulerp - ((cpulerp - cpu) * 0.2f);
            }

            float cpupoop = (cpulerp / 100f);
            int idx = (int)(cpupoop * 15);

            
            for (int i = 0; i < 15; i++)
            {
                if (i < idx)
                {
                    leds[i].Color = new LEDColor((byte) (255 * (cpupoop)),  (byte) (255 * (1f-cpupoop)),0);
                }
                else
                {
                    leds[i].Color = new LEDColor(0, 0, 0);
                }
            }

        }

        public DriverProperties GetProperties()
        {
            return new DriverProperties
            {
                SupportsPull = true,
                SupportsPush = false,
                IsSource = true,
                Id = Guid.Parse("99440d02-8ca3-4e35-a9a3-88b024cc0e2d"),
                Author = "mad ninja",
                Blurb = "Monitor your system with your RGB fans and LED strips!",
                CurrentVersion = new ReleaseNumber(1,0,0,4),
                GitHubLink = "https://github.com/SimpleLed/Source.MadMeters",
                IsPublicRelease = true
            };
        }

        public T GetConfig<T>() where T : SLSConfigData
        {
            return null;
        }

        public void PutConfig<T>(T config) where T : SLSConfigData
        {

        }

        public string Name()
        {
            return "MadMeters";
        }

        public event EventHandler DeviceRescanRequired;
    }
}
