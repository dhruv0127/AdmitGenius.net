using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdmitGenius.Models
{
    public class ChatMessage
    {
        [Key]
        public Guid MessageId { get; set; } 
        public Guid UserId { get; set; }
        public string Sender { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }

        public string UserProfile { get; set; }

        public bool? ContentType { get; set; }


        // Navigation property to represent the associated User
        [ForeignKey("UserId")]
        public Users User { get; set; }

        public string groupName { get; set; }


    }
}
