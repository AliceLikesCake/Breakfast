using BreakfastOrderSystem.Site.Models.Repositories;
using BreakfastOrderSystem.Site.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace BreakfastOrderSystem.Site.Models.Services
{
    public class MemberService
    {
        private readonly MemberRepository _memberRepository;

        public MemberService()
        {
            _memberRepository = new MemberRepository();
        }

        public List<MemberDetailVm> GetMemberDetails()
        {
            var members = _memberRepository.GetMembers();
            return members.Select(m => new MemberDetailVm
            {
                MemberName = m.Name,
                Email = m.Account,
                Phone = m.Phone,
                Point = m.Points,
                RegisterDate = m.RegistrationDate,
            }).ToList();
        }

        // 獲得黑名單
        public List<MemberDetailVm> GetBlacklist()
        {
            var members = _memberRepository.GetBlacklistedMembers();
            return members.Select(m => new MemberDetailVm
            {
                MemberName = m.Name,
                Email = m.Account,
                Phone = m.Phone,
                Point = m.Points,
                RegisterDate = m.RegistrationDate,
            }).ToList();
        }

        // 解除封鎖會員
        public void UnblockMember(string email)
        {
            _memberRepository.UnblockMember(email);
        }
    }
}