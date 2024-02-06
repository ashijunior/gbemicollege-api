using AutoMapper;
using gbemi.Models;

namespace gbemi.Mapper
{
    public class UserProfile : Profile
    {
        public UserProfile() {
                CreateMap<User, SigninDTO>();
                //CreateMap<User, EmailModel>();
                //CreateMap<User, ResetPasswordDTO>();
                CreateMap<User, LoginDTO>();
                CreateMap<SigninDTO, User>();
                //CreateMap<SigninDTO, ResetPasswordDTO>();
                //CreateMap<SigninDTO, EmailModel>();
                CreateMap<SigninDTO, LoginDTO>();
                CreateMap<LoginDTO, User>();
                CreateMap<LoginDTO, SigninDTO>();
                //CreateMap<LoginDTO, ResetPasswordDTO>();
                //CreateMap<LoginDTO, EmailModel>();
                //CreateMap<ResetPasswordDTO, User>();
                //CreateMap<ResetPasswordDTO, EmailModel>();
                //CreateMap<ResetPasswordDTO, LoginDTO>();
                //CreateMap<ResetPasswordDTO, SigninDTO>();
        //        CreateMap<EmailModel, ResetPasswordDTO>();
        //        CreateMap<EmailModel, LoginDTO>();
        //        CreateMap<EmailModel, SigninDTO>();
        //        CreateMap<EmailModel, User>();
        }
    }
}
