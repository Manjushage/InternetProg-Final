using AnketPortali.Repositories;
using AnketPortali.ViewModel;
using AutoMapper;
using AnketPortali.Models;
using AnketPortali.Repositories;
using AnketPortali.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.SignalR;
using AnketPortali.Hubs;

namespace AnketPortali.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SurveyController : Controller
    {
        private readonly SurveyRepository _surveyRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly QuestionRepository _questionRepository;
        private readonly AnswersRepository _answersRepository;
        private readonly SurveyApplicationRepository _surveyApplicationRepository;
        private readonly UserAnswerRepository _userAnswerRepository;
        private readonly IMapper _mapper;
        private readonly INotyfService _notyfService;
        private readonly IHubContext<SurveyHub> _hubContext;

        public SurveyController(SurveyRepository surveyRepository, CategoryRepository categoryRepository, QuestionRepository questionRepository, AnswersRepository answersRepository, SurveyApplicationRepository surveyApplicationRepository, UserAnswerRepository userAnswerRepository, IMapper mapper, INotyfService notyfService, IHubContext<SurveyHub> hubContext)
        {
            _surveyRepository = surveyRepository;
            _categoryRepository = categoryRepository;
            _questionRepository = questionRepository;
            _answersRepository = answersRepository;
            _surveyApplicationRepository = surveyApplicationRepository;
            _userAnswerRepository = userAnswerRepository;
            _mapper = mapper;
            _notyfService = notyfService;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var surveys = await _surveyRepository.GetAllSurvey();
            return View(surveys);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(SurveyModel model)
        {
            if (ModelState.IsValid)
            {
                var survey = _mapper.Map<Survey>(model);
                survey.Created = DateTime.Now;
                survey.Updated = DateTime.Now;
                await _surveyRepository.AddAsync(survey);
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var survey = await _surveyRepository.GetByIdAsync(id);
            if (survey == null)
            {
                return NotFound();
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(survey);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SurveyModel model)
        {
            if (ModelState.IsValid)
            {
                var survey = _mapper.Map<Survey>(model);
                await _surveyRepository.UpdateAsync(survey);
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _surveyRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> AddQuestion(int surveyId)
        {
            // Var olan sorular� getiriyoruz
            var questions = await _questionRepository.GetQuestionsBySurveyIdAsync(surveyId);

            var model = new AddQuestionViewModel
            {
                SurveyId = surveyId,
                Questions = questions.Select(q => new QuestionViewModel
                {
                    Id = q.Id,
                    QuestionText = q.QuestionText
                }).ToList()
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddQuestion(AddQuestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Yeni soru ekliyoruz
                var question = new Question
                {
                    QuestionText = model.QuestionText,
                    SurveyId = model.SurveyId,
                    Answers = Enum.GetValues(typeof(AnswerType))
                                  .Cast<AnswerType>()
                                  .Select(a => new Answer { AnswerValue = a })
                                  .ToList()
                };

                await _questionRepository.AddAsync(question);
            }

            // Yeniden sorular� getirip sayfaya g�nderiyoruz
            var questions = await _questionRepository.GetQuestionsBySurveyIdAsync(model.SurveyId);

            model.Questions = questions.Select(q => new QuestionViewModel
            {
                Id = q.Id,
                QuestionText = q.QuestionText
            }).ToList();

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteQuestion(int questionId, int surveyId)
        {
            // Soruyu siliyoruz
            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question != null)
            {
                await _questionRepository.DeleteAsync(question.Id);
            }

            // Silme i�leminden sonra formu yeniden y�klemek i�in gerekli sorular� al�yoruz
            var questions = await _questionRepository.GetQuestionsBySurveyIdAsync(surveyId);

            var model = new AddQuestionViewModel
            {
                SurveyId = surveyId,
                Questions = questions.Select(q => new QuestionViewModel
                {
                    Id = q.Id,
                    QuestionText = q.QuestionText
                }).ToList()
            };

            return View("AddQuestion", model); // Ayn� sayfay� y�kle
        }
        [AllowAnonymous]
        [Authorize]
        public async Task<IActionResult> TakeSurvey(int surveyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.Identity.Name;

            // Identity'den kullanıcı ID'sini al
            if (string.IsNullOrEmpty(userId))
            {
                _notyfService.Error("Kullanıcı kimliği bulunamadı!");
                return RedirectToAction("Index", "Home");
            }

            // Kullanıcının daha önce bu ankete başvurup başvurmadığını kontrol et
            var existingApplication = await _surveyApplicationRepository
                .GetAsync(sa => sa.SurveyId == surveyId && sa.UserId == userId);

            if (existingApplication != null)
            {
                _notyfService.Warning("Bu ankete zaten katıldınız!");
                return RedirectToAction("Index", "Home");
            }

            // Anketin sorularını getir
            var questions = await _questionRepository.GetQuestionsBySurveyIdAsync(surveyId);

            if (questions == null || !questions.Any())
            {
                _notyfService.Warning("Anket soruları bulunamadı, anket hazırlama aşamasında.");
                return RedirectToAction("Index", "Home");
            }

            ViewBag.SurveyId = surveyId;
            await _hubContext.Clients.All.SendAsync("ReceiveUserStartedSurvey", userName, surveyId);
            return View(questions);
        }
           [AllowAnonymous]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SubmitAnswers(Dictionary<int, Answer> answers, int surveyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.Identity.Name;

            if (string.IsNullOrEmpty(userId))
            {
                _notyfService.Error("Kullanıcı kimliği bulunamadı!");
                return RedirectToAction("Index", "Home");
            }

            if (answers == null || !answers.Any())
            {
                _notyfService.Error("Geçerli cevaplar girilmelidir.");
                return BadRequest();
            }

            // Kullanıcının daha önce bu ankete katılıp katılmadığını kontrol et
            var existingApplication = await _surveyApplicationRepository
                .GetAsync(sa => sa.SurveyId == surveyId && sa.UserId == userId);

            if (existingApplication != null)
            {
                _notyfService.Warning("Bu ankete zaten katıldınız!");
                return RedirectToAction("Index", "Home");
            }

            // Yeni başvuru oluştur
            await _surveyApplicationRepository.AddAsync(new SurveyApplication
            {
                SurveyId = surveyId,
                UserId = userId,
                AppliedDate = DateTime.Now
            });

            // Cevapları kaydet
            foreach (var answer in answers)
            {
                var userAnswer = new UserAnswer
                {
                    QuestionId = answer.Key,
                    UserId = userId,
                    AnswerValue = answer.Value.AnswerValue,
                    AnswerDate = DateTime.Now
                };

                await _userAnswerRepository.AddAsync(userAnswer); // UserAnswer tablosuna ekleme
            }

            _notyfService.Success("Anketi başarıyla yanıtladınız.");
            await _hubContext.Clients.All.SendAsync("ReceiveSurveyCompleted", userName, surveyId);
            return RedirectToAction("Index", "Home");
        }


    }
} 