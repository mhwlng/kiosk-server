using System.ComponentModel.DataAnnotations;

namespace kiosk_server.Model
{
    public class SetupModel
    {
        [StringLength(256, ErrorMessage = "Url is too long.")]
        public string? Url { get; set; }


        public DiskMetrics DiskMetrics { get; set; } = default!;
        public TemperatureMetrics TemperatureMetrics { get; set; } = default!;
        public MemoryMetrics MemoryMetrics { get; set; } = default!;
        public CpuMetrics CpuMetrics { get; set; } = default!;
        
    }
}

