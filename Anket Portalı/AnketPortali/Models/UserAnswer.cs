namespace AnketPortali.Models
{
    public class UserAnswer : BaseEntity
    {
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        
        public string UserId { get; set; }
        public virtual AppUser User { get; set; }
        
        public AnswerType AnswerValue { get; set; }
        public DateTime AnswerDate { get; set; }
    }
}
