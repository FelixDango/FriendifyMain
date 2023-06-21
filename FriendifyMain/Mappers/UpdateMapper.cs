using AutoMapper;
using FriendifyMain.Models;
using FriendifyMain.ViewModels;

namespace FriendifyMain.Mappers
{
    public class UpdateMapper : Profile
    {
        public UpdateMapper()
        {
            CreateMap<UpdateViewModel, User>() // Define the mapping between UpdateViewModel and User models
                .ForMember(dest => dest.UserName, opt => opt.Condition(src => src.Username != null && src.Username.Replace(" ", "") != "")) // Only map the Username property if it's not null
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username)) // Map the Username property from source to destination
                .ForMember(dest => dest.FirstName, opt => opt.Condition(src => src.FirstName != null && src.FirstName.Replace(" ", "") != "")) // Only map the FirstName property if it's not null
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName)) // Map the FirstName property from source to destination
                .ForMember(dest => dest.LastName, opt => opt.Condition(src => src.LastName != null && src.LastName.Replace(" ", "") != "")) // Only map the LastName property if it's not null
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName)) // Map the LastName property from source to destination
                .ForMember(dest => dest.BirthDate, opt => opt.Condition(src => src.BirthDate != null)) // Only map the BirthDate property if it's not null
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate)) // Map the BirthDate property from source to destination
                .ForMember(dest => dest.Email, opt => opt.Condition(src => src.Email != null && src.Email.Replace(" ", "") != "")) // Only map the Email property if it's not null
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email)) // Map the Email property from source to destination
                .ForMember(dest => dest.Sex, opt => opt.Condition(src => src.Sex != null)) // Only map the Sex property if it's not null
                .ForMember(dest => dest.Sex, opt => opt.MapFrom(src => src.Sex)) // Map the Sex property from source to destination
                .ForMember(dest => dest.Status, opt => opt.Condition(src => src.Status != null)) // Only map the Status property if it's not null
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status)) // Map the Status property from source to destination
                .ForMember(dest => dest.Biography, opt => opt.Condition(src => src.Biography != null && src.Biography.Replace(" ", "") != "")) // Only map the Biography property if it's not null
                .ForMember(dest => dest.Biography, opt => opt.MapFrom(src => src.Biography)); // Map the Biography property from source to destination
        }
    }
}
