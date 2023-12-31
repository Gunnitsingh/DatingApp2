﻿using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void UpdateUser (AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserById(int id);
        Task<AppUser> GetUserByNameAsync(string username);
        Task <IEnumerable<MemberDto>> GetMembersAsync();

        Task<MemberDto> GetMemberByUsernameAsync(string username);
      
    }
}
