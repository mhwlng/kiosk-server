using System.ComponentModel.DataAnnotations;

namespace kiosk_server.Model
{
    public class SetupModel
    {
        [StringLength(256, ErrorMessage = "Url is too long.")]
        public string? Url { get; set; }

        public double Total { get; set; }
        public double Used { get; set; }
        public double Free { get; set; }

        public double TotalSize { get; set; }
        public double AvailableFreeSpace { get; set; }


        public string? OsDescription { get; set; }
    }
}
