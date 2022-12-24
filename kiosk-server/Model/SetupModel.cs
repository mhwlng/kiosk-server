using System.ComponentModel.DataAnnotations;
using kiosk_server.Metrics;

namespace kiosk_server.Model
{
    public class SetupModel
    {
        public DiskMetrics Disk { get; set; } = default!;
        public TemperatureMetrics Temperature { get; set; } = default!;
        public MemoryMetrics Memory { get; set; } = default!;
        public CpuMetrics Cpu { get; set; } = default!;
        
    }
}

