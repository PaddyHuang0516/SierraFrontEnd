﻿using Humanizer;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using SIERRA_Server.Models.DTOs.Members;
using SIERRA_Server.Models.EFModels;
using SIERRA_Server.Models.Infra;
using SIERRA_Server.Models.Interfaces;

namespace SIERRA_Server.Models.Repository.EFRepository
{
    public class MemberEFRepository : IMemberRepository
    {
        private readonly AppDbContext _db;
        public MemberEFRepository(AppDbContext db)
        {
            _db = db;
        }

        public Member? GetMemberByUsername(string username)
        {
            return _db.Members.FirstOrDefault(m => m.Username == username);
        }
        public Member? GetMemberById(int memberId)
        {
            return _db.Members.FirstOrDefault(m => m.Id == memberId);
        }

        public int GetMemberIdByUsername(string username)
        {
            var member = _db.Members.FirstOrDefault(m => m.Username == username);
            if (member != null) return member.Id;
            return -1;
        }

        public bool isAccountExist(string username)
        {
            return _db.Members.Any(m => m.Username == username);
        }

        public bool isAccountExist(int memberId)
        {
            return _db.Members.Any(m => m.Id == memberId);
        }

        public void PostMember(RegisterDTO dto)
        {
            Member member = new Member
            {
                Username = dto.Username,
                Email = dto.Email,
                EncryptedPassword = dto.EncryptedPassword,
                IsConfirmed = dto.IsConfirmed,
                ConfirmCode = dto.ConfirmCode,
            };

            _db.Members.Add(member);
            _db.SaveChanges();
        }

        public void ActiveRegister(Member member)
        {
            _db.SaveChanges();
        }
    }
}