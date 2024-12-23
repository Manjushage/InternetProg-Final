using AnketPortali.Models;
using AnketPortali.Repositories;
using AnketPortali.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace AnketPortali.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SurveyApplicationRepository _surveyApplicationRepository;
        private readonly SurveyRepository _surveyRepository;

        public HomeController(ILogger<HomeController> logger, SurveyApplicationRepository surveyApplicationRepository, SurveyRepository surveyRepository)
        {
            _logger = logger;
            _surveyApplicationRepository = surveyApplicationRepository;
            _surveyRepository = surveyRepository;
        }

        public async Task<IActionResult> Index()
        {
            var surveys = await _surveyRepository.GetAllAsync();
            return View(surveys);


        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ApplyToSurvey(int surveyId)
        {
            // Identity'den kullanıcı ID'sini al
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Kullanıcı kimliği bulunamadı!";
                return RedirectToAction("Index");
            }

            // Kullanıcının daha önce bu ankete başvurup başvurmadığını kontrol et
            var existingApplication = await _surveyApplicationRepository
                .GetAsync(sa => sa.SurveyId == surveyId && sa.UserId == userId);

            if (existingApplication != null)
            {
                TempData["Error"] = "Bu ankete zaten başvurdunuz!";
                return RedirectToAction("Index");
            }

            // Yeni başvuru oluştur
            var application = new SurveyApplication
            {
                SurveyId = surveyId,
                UserId = userId,
                AppliedDate = DateTime.Now
            };

            await _surveyApplicationRepository.AddAsync(application);
            TempData["Success"] = "Ankete başarıyla başvurdunuz!";
            return RedirectToAction("Index");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
