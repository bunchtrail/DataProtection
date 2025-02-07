using System;

namespace LicenseCore.Models
{
    public class HardwareInfo
    {
        public string CpuId { get; set; }
        public string MacAddress { get; set; }
        public string DiskSerialNumber { get; set; }
        public string MotherboardSerialNumber { get; set; }
        public string BiosSerialNumber { get; set; }
        public string WindowsId { get; set; }

        public override string ToString()
        {
            return $"{CpuId}|{MacAddress}|{DiskSerialNumber}|{MotherboardSerialNumber}|{BiosSerialNumber}|{WindowsId}";
        }
    }
} 