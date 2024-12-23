using AnketPortali.Models;

namespace AnketPortali.Models
{
    public class SurveyApplication : BaseEntity
    {
    

        public int SurveyId { get; set; }
        public Survey Survey { get; set; }

        public string UserId { get; set; } // Kullanıcıyı tanımlamak için
        public AppUser User { get; set; }

        public DateTime AppliedDate { get; set; }
    }
}
