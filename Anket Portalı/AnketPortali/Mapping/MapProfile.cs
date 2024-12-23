using AnketPortali.ViewModel;
using AutoMapper;
using AnketPortali.Models;
using AnketPortali.ViewModel;

namespace AnketPortali.Mapping
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
           CreateMap<Category, CategoryViewModel>().ReverseMap();
            CreateMap<Survey, SurveyModel>().ReverseMap();

          
        }
    }
}
