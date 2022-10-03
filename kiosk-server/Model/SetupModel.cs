using System.ComponentModel.DataAnnotations;
using kiosk_server.Metrics;

namespace kiosk_server.Model
{
    public class SetupModel
    {
        public DiskMetrics DiskMetrics { get; set; } = default!;
        public TemperatureMetrics TemperatureMetrics { get; set; } = default!;
        public MemoryMetrics MemoryMetrics { get; set; } = default!;
        public CpuMetrics CpuMetrics { get; set; } = default!;
        
    }
}

