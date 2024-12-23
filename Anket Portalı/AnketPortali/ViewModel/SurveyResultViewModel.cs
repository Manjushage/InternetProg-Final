using AnketPortali.Models;

public class SurveyResultViewModel
{
    public int SurveyId { get; set; }
    public string SurveyTitle { get; set; }
    public List<QuestionResultViewModel> Questions { get; set; }
}

public class QuestionResultViewModel
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; }
    public Dictionary<AnswerType, int> AnswerCounts { get; set; } // Her cevap için toplam sayı
    public List<UserAnswerViewModel> UserAnswers { get; set; }
}

public class UserAnswerViewModel
{
    public string UserName { get; set; }
    public AnswerType Answer { get; set; }
    public DateTime AnswerDate { get; set; }
} 