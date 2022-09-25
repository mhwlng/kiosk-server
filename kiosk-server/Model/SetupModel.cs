using System.ComponentModel.DataAnnotations;

namespace kiosk_server.Model
{
    public class SetupModel
    {
        [StringLength(256, ErrorMessage = "Url is too long.")]
        public string? Url { get; set; }
    }
}
