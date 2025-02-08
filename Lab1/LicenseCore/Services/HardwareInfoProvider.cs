using System;
using System.Management;
using LicenseCore.Interfaces;
using LicenseCore.Models;

namespace LicenseCore.Services
{
    public class HardwareInfoProvider : IHardwareInfoProvider
    {
        public HardwareInfo GetHardwareInfo()
        {
            return new HardwareInfo
            {
                CpuId = GetCpuId(),
                MacAddress = GetMacAddress(),
                DiskSerialNumber = GetDiskSerialNumber(),
                MotherboardSerialNumber = GetMotherboardSerialNumber(),
                BiosSerialNumber = GetBiosSerialNumber(),
                WindowsId = GetWindowsId()
            };
        }

        private string GetCpuId()
        {
            try
            {
                var cpuInfo = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor");
                foreach (var item in cpuInfo.Get())
                {
                    return item["ProcessorId"]?.ToString() ?? "";
                }
            }
            catch
            {
                // В случае ошибки возвращаем пустую строку
            }
            return "";
        }

        private string GetMacAddress()
        {
            try
            {
                var networkInfo = new ManagementObjectSearcher("SELECT MACAddress FROM Win32_NetworkAdapter WHERE PhysicalAdapter=True");
                foreach (var item in networkInfo.Get())
                {
                    var mac = item["MACAddress"]?.ToString();
                    if (!string.IsNullOrEmpty(mac))
                        return mac;
                }
            }
            catch
            {
                // В случае ошибки возвращаем пустую строку
            }
            return "";
        }

        private string GetDiskSerialNumber()
        {
            try
            {
                var diskInfo = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_DiskDrive");
                foreach (var item in diskInfo.Get())
                {
                    return item["SerialNumber"]?.ToString() ?? "";
                }
            }
            catch
            {
                // В случае ошибки возвращаем пустую строку
            }
            return "";
        }

        private string GetMotherboardSerialNumber()
        {
            try
            {
                var motherboardInfo = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard");
                foreach (var item in motherboardInfo.Get())
                {
                    return item["SerialNumber"]?.ToString() ?? "";
                }
            }
            catch
            {
                // В случае ошибки возвращаем пустую строку
            }
            return "";
        }

        private string GetBiosSerialNumber()
        {
            try
            {
                var biosInfo = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BIOS");
                foreach (var item in biosInfo.Get())
                {
                    return item["SerialNumber"]?.ToString() ?? "";
                }
            }
            catch
            {
                // В случае ошибки возвращаем пустую строку
            }
            return "";
        }

        private string GetWindowsId()
        {
            try
            {
                var windowsInfo = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_OperatingSystem");
                foreach (var item in windowsInfo.Get())
                {
                    return item["SerialNumber"]?.ToString() ?? "";
                }
            }
            catch
            {
                // В случае ошибки возвращаем пустую строку
            }
            return "";
        }
    }
}