using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using AnketPortali.Repositories;
using AnketPortali.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AnketPortali.Models;
using Microsoft.AspNetCore.Authorization;

namespace AnketPortali.Controllers
{
    [Authorize(Roles="Admin")]
    public class AdminController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly INotyfService _notyfService;
        private readonly IMapper _mapper;
        private readonly SurveyRepository _surveyRepository;
        private readonly QuestionRepository _questionRepository;
        private readonly AnswersRepository _answersRepository;
        private readonly UserAnswerRepository _userAnswerRepository;
        private readonly UserManager<AppUser> _userManager;

        public AdminController(UserRepository userRepository, INotyfService notyfService, IMapper mapper,
            SurveyRepository surveyRepository,
            QuestionRepository questionRepository,
            AnswersRepository answersRepository,
            UserAnswerRepository userAnswerRepository,
            UserManager<AppUser> userManager)
        {
            _userRepository = userRepository;
            _notyfService = notyfService;
            _mapper = mapper;
            _surveyRepository = surveyRepository;
            _questionRepository = questionRepository;
            _answersRepository = answersRepository;
            _userAnswerRepository = userAnswerRepository;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> UserList()
        {
            var users = await _userRepository.GetAllAsync();
          
            return View(users);
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userRepository.DeleteAsync(id);
            if (result)
            {
                _notyfService.Success("Kullanıcı başarıyla silindi");
            }
            else
            {
                _notyfService.Error("Kullanıcı silinirken bir hata oluştu");
            }
            return RedirectToAction(nameof(UserList));
        }

        public async Task<IActionResult> SurveyResults(int surveyId)
        {
            var survey = await _surveyRepository.GetSurveyById(surveyId);
            if (survey == null) return NotFound();

            var questions = await _questionRepository.GetQuestionsBySurveyIdAsync(surveyId);
            var model = new SurveyResultViewModel
            {
                SurveyId = surveyId,
               
                Questions = new List<QuestionResultViewModel>()
            };

            foreach (var question in questions)
            {
                var answers = await _userAnswerRepository.GetAllUserAsync(a => a.QuestionId == question.Id);
                var questionResult = new QuestionResultViewModel
                {
                    QuestionId = question.Id,
                    QuestionText = question.QuestionText,
                    AnswerCounts = new Dictionary<AnswerType, int>(),
                    UserAnswers = new List<UserAnswerViewModel>()
                };

                

                foreach (var answer in answers)
                {
                    // Cevap sayılarını hesapla
                    if (!questionResult.AnswerCounts.ContainsKey(answer.AnswerValue))
                        questionResult.AnswerCounts[answer.AnswerValue] = 0;
                    questionResult.AnswerCounts[answer.AnswerValue]++;
                    
                    // Kullanıcı cevaplarını ekle
                    var user = await _userManager.FindByIdAsync(answer.UserId);
                    questionResult.UserAnswers.Add(new UserAnswerViewModel
                    {
                        UserName = user?.UserName ?? "Anonim",
                        Answer = answer.AnswerValue,
                        AnswerDate = answer.Created
                    });
                }

                model.Questions.Add(questionResult);
            }

            return View(model);
        }

        public async Task<IActionResult> AllSurveyResults()
        {
            var surveys = await _surveyRepository.GetAllAsync();
            var surveyResults = new List<SurveyResultViewModel>();

            foreach (var survey in surveys)
            {
                var questions = await _questionRepository.GetQuestionsBySurveyIdAsync(survey.Id);
                var questionResults = new List<QuestionResultViewModel>();

                foreach (var question in questions)
                {
                    var answers = await _userAnswerRepository.GetAllUserAsync(a => a.QuestionId == question.Id);
                    var questionResult = new QuestionResultViewModel
                    {
                        QuestionId = question.Id,
                        QuestionText = question.QuestionText,
                        AnswerCounts = new Dictionary<AnswerType, int>(),
                        UserAnswers = new List<UserAnswerViewModel>()
                    };

                    if (!answers.Any())
                    {
                        questionResult.AnswerCounts[AnswerType.Orta] = 0; // Cevap yoksa 0 olarak ayarla
                        questionResult.AnswerCounts[AnswerType.İyi] = 0; // Cevap yoksa 0 olarak ayarla
                        questionResult.AnswerCounts[AnswerType.Kötü] = 0; // Cevap yoksa 0 olarak ayarla
                        questionResult.AnswerCounts[AnswerType.Çokİyi] = 0; // Cevap yoksa 0 olarak ayarla
                    }

                    // Cevapları kontrol et
                
                    foreach (var answer in answers)
                    {
                        // Kullanıcı null değilse devam et
                        var user = await _userManager.FindByIdAsync(answer.UserId);
                        if (user != null)
                        {
                            // Cevap sayılarını hesapla
                            if (!questionResult.AnswerCounts.ContainsKey(answer.AnswerValue))
                                questionResult.AnswerCounts[answer.AnswerValue] = 0;
                            questionResult.AnswerCounts[answer.AnswerValue]++;

                            // Kullanıcı cevaplarını ekle
                            questionResult.UserAnswers.Add(new UserAnswerViewModel
                            {
                                UserName = user.UserName,
                                Answer = answer.AnswerValue,
                                AnswerDate = answer.Created
                            });
                        }

                    }

                    questionResults.Add(questionResult);
                }

                surveyResults.Add(new SurveyResultViewModel
                {
                    SurveyId = survey.Id,
                    SurveyTitle = survey.Title,
                    Questions = questionResults
                });
            }

            return View("AllSurveyResults", surveyResults);
        }
    }
}
