﻿using SIERRA_Server.Models.DTOs.Peomotions;
using SIERRA_Server.Models.Interfaces;

namespace SIERRA_Server.Models.Services
{
    public class MemberCouponService
	{
		private IMemberCouponRepository _repo;

        public MemberCouponService(IMemberCouponRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<MemberCouponDto>> GetUsableCoupon(int? MemberId)
        {
            var coupons = await _repo.GetUsableCoupon(MemberId);
            return coupons;
        }

        public async Task<IEnumerable<MemberCouponCanNotUseDto>> GetCouponCanNotUseNow(int? MemberId)
        {
            var coupons = await _repo.GetCouponCanNotUseNow(MemberId);
            return coupons;
        }

        public async Task<string> GetCouponByCode(int? memberId, string code)
        {
            var isExist = await _repo.CheckCouponExist(code);
            if (!isExist)
            {
                return "查無此優惠碼";
            }
            else
            {
                var result =await _repo.CheckHaveSame((int)memberId, code);
                if (result.HaveSame)
                {
                    return "已領取過此優惠券";
                }
                else
                {
                    MemberCouponCreateDto dto = new MemberCouponCreateDto();
                    dto.CouponId = result.CouponId;
                    dto.MemberId = (int)memberId;
                    return await _repo.GetCouponByCode(dto);
                }
            }
            
        }
    }
}