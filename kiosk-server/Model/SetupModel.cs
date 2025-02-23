using kiosk_server.Metrics;

namespace kiosk_server.Model
{
    public class SetupModel
    {
        public DiskMetrics Disk { get; set; } = null!;
        public TemperatureMetrics Temperature { get; set; } = null!;
        public MemoryMetrics Memory { get; set; } = null!;
        public CpuMetrics Cpu { get; set; } = null!;
        
    }
}

