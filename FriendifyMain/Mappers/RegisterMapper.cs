using AutoMapper;
using FriendifyMain.Models;
using FriendifyMain.ViewModels;

namespace FriendifyMain.Mappers
{
    public class RegisterMapper : Profile
    {
        public RegisterMapper()
        {
            CreateMap<RegisterViewModel, User>() // Define the mapping between RegisterViewModel and User models
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName.ToString())) // Map the FirstName property from source to destination and convert it to a string
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName.ToString())) // Map the LastName property from source to destination and convert it to a string
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username)) // Map the Username property from source to destination
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate.ToString("yyyy-MM-dd"))) // Map the BirthDate property from source to destination and convert it to a string
                .ForMember(dest => dest.Sex, opt => opt.MapFrom(src => src.Sex.ToString())) // Map the Sex property from source to destination and convert it to a string
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString())) // Map the Status property from source to destination and convert it to a string
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.ToString())); // Map the Email property from source to destination and convert it to a string
        }
    }
}
