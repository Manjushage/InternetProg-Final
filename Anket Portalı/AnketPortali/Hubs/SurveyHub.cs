using Microsoft.AspNetCore.SignalR;

namespace AnketPortali.Hubs
{
    public class SurveyHub : Hub
    {
        public async Task SendSurveyCompleted(string userName, int surveyId)
        {
            await Clients.All.SendAsync("ReceiveSurveyCompleted", userName, surveyId);
        }

        public async Task SendUserStartedSurvey(string userName, int surveyId)
        {
            await Clients.All.SendAsync("ReceiveUserStartedSurvey", userName, surveyId);
        }
    }
} 