using System.ComponentModel.DataAnnotations;

namespace kiosk_server.Model
{
    public class SetupModel
    {
        [Required]
        [StringLength(256, ErrorMessage = "Url is too long.")]
        public string? Url { get; set; }
    }
}
