using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using TheGenesis.Core.Classes.Datas;

namespace TheGenesis.Core.Utils
{
    public static class SysUtils
    {
        public static readonly string PlatformName = GetPlatformName();
        public static readonly string SystemArch = GetSystemArch();

        public static string GetPlatformName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return "osx";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return "linux";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return "windows";
            return "unknown";
        }

        /// <summary>
        /// 获取系统位数
        /// </summary>
        /// <returns></returns>
        public static string GetSystemArch() => Environment.Is64BitOperatingSystem ? "64" : "32";

        public static MemoryMetrics GetMemoryMetrics()
        {
            MemoryMetrics metrics;
            if (OperatingSystem.IsWindows())
            {
                using var process = Process.Start(new ProcessStartInfo()
                {
                    FileName = "wmic",
                    Arguments = "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value",
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                });

                process.WaitForExit();

                var lines = process.StandardOutput.ReadToEnd().Trim().Split("\n");
                var freeMemoryParts = lines[0].Split("=", StringSplitOptions.RemoveEmptyEntries);
                var totalMemoryParts = lines[1].Split("=", StringSplitOptions.RemoveEmptyEntries);

                var total = Math.Round(double.Parse(totalMemoryParts[1]) / 1024, 0);
                var free = Math.Round(double.Parse(freeMemoryParts[1]) / 1024, 0);

                metrics = new MemoryMetrics
                {
                    Total = total,
                    Free = free,
                    Used = total - free
                };
            }
            else
            {
                using var process = Process.Start(new ProcessStartInfo("free -m")
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"free -m\"",
                    RedirectStandardOutput = true
                });

                process.WaitForExit();

                var lines = process.StandardOutput.ReadToEnd().Split("\n");
                var memory = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);

                metrics = new MemoryMetrics
                {
                    Total = double.Parse(memory[1]),
                    Used = double.Parse(memory[2]),
                    Free = double.Parse(memory[3])
                };
            }
            return metrics;
        }

        public static (int, int) CalculateJavaMemory(int min = 512)
        {
            var metrics = GetMemoryMetrics();
            var willUsed = metrics.Free * 0.6;
            var max = willUsed < min ? min : Convert.ToInt32(willUsed);
            return (max, min);
        }
    }
}
