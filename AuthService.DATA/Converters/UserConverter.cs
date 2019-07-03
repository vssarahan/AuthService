using AuthService.DATA.Dto;
using AuthService.DATA.Enteties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AuthService.DATA.Converters
{
    public static class UserConverter
    {
        public static User Convert(UserDto item)
        {
            return new User
            {
                Email = item.Email,
                UserName = item.Email,
                Id = item.Id,
                Name = item.Name,
                Surname = item.Surname
            };
        }

        public static UserDto Convert(User item)
        {
            return new UserDto
            {
                Id = item.Id,
                Email = item.Email,
                Name = item.Name,
                Surname = item.Surname
            };
        }

        public static List<User> Convert(List<UserDto> items)
        {
            return items.Select(u => Convert(u)).ToList();
        }

        public static List<UserDto> Convert(List<User> items)
        {
            return items.Select(u => Convert(u)).ToList();
        }
    }
}
