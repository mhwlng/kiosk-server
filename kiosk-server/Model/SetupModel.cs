using System.ComponentModel.DataAnnotations;

namespace kiosk_server.Model
{
    public class SetupModel
    {
        [StringLength(256, ErrorMessage = "Url is too long.")]
        public string? Url { get; set; }

        public double TotalMemory { get; set; }
        public double UsedMemory { get; set; }
        public double FreeMemory { get; set; }

        public double TotalDiskSpace { get; set; }
        public double AvailableDiskSpace { get; set; }

        public float CpuTemperature { get; set; }
        public string? ThrottledState { get; set; }


        public string? OsDescription { get; set; }
    }
}
