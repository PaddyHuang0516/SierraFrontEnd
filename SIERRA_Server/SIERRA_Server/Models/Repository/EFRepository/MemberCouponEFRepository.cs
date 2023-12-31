﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIERRA_Server.Models.DTOs.Promotions;
using SIERRA_Server.Models.EFModels;
using SIERRA_Server.Models.Exts;
using SIERRA_Server.Models.Infra.Promotions;
using SIERRA_Server.Models.Interfaces;
using System.Linq;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;

namespace SIERRA_Server.Models.Repository.EFRepository
{
    public class MemberCouponEFRepository : IMemberCouponRepository
	{
		private readonly AppDbContext _db;
        public MemberCouponEFRepository(AppDbContext db)
        {
            _db= db;
        }
        public async Task<IEnumerable<MemberCoupon>> GetUsableCoupon(int? MemberId)
		{
			var coupons = await _db.MemberCoupons.Include(mc => mc.Coupon)
										   .ThenInclude(c => c.DiscountGroup)
										   .ThenInclude(dg => dg.DiscountGroupItems)
										   .ThenInclude(dgi => dgi.Dessert)
										   .Where(mc => mc.MemberId == MemberId)
										   .Where(mc => mc.UseAt == null && mc.ExpireAt > DateTime.Now)
										   .Where(mc => mc.Coupon.StartAt == null || mc.Coupon.StartAt <= DateTime.Now)
                                           .OrderBy(mc=>mc.CreateAt)
										   .ToListAsync();
			return coupons;
		}

        public async Task<IEnumerable<MemberCoupon>> GetCouponCanNotUseNow(int? MemberId)
        {

            var coupons = await _db.MemberCoupons.Include(mc => mc.Coupon)
                                           .ThenInclude(c => c.DiscountGroup)
                                           .ThenInclude(dg => dg.DiscountGroupItems)
                                           .ThenInclude(dgi => dgi.Dessert)
                                           .Where(mc => mc.MemberId == MemberId)
                                           .Where(mc => mc.UseAt == null && mc.ExpireAt > DateTime.Now)
                                           .Where(mc => mc.Coupon.CouponCategoryId == 2 && mc.Coupon.StartAt > DateTime.Now)
                                           .OrderBy(mc => mc.CreateAt)
                                           .ToListAsync();
            return coupons;
        }

        public async Task<bool>  CheckCouponExist(string code)
        {
            if(await _db.Coupons.Where(c=>c.CouponCategoryId==4).AnyAsync(c=>c.CouponCode==code))
            {
                return  true;
            }
            else return false;
        }

        public async Task<ResultForCheck>CheckHaveSame(int memberId, string code)
        {
            var coupon =await _db.Coupons.Where(c=>c.CouponCategoryId==4).FirstAsync(c=>c.CouponCode==code);
            var couponId = coupon.CouponId;
            bool haveSame = await _db.MemberCoupons.Where(mc=>mc.MemberId==memberId)
                                                   .AnyAsync(c=>c.CouponId == couponId);
            return new ResultForCheck(couponId,haveSame);

        }

        public async Task<AddCouponResult> GetCouponByCode(MemberCouponCreateDto dto)
        {
            var coupon = _db.Coupons.Find(dto.CouponId);
            if(coupon == null)
            {
                return AddCouponResult.Fail("查無此優惠券");
            }
            else
            {
                var newMemberCoupon = new MemberCoupon();
                newMemberCoupon.MemberId = dto.MemberId;
                newMemberCoupon.CouponId = dto.CouponId;
                newMemberCoupon.CouponName = coupon.CouponName;
                newMemberCoupon.CreateAt = DateTime.Now;
                newMemberCoupon.ExpireAt = DateTime.Now.AddDays((double)coupon.Expiration);
                _db.MemberCoupons.Add(newMemberCoupon);
                await _db.SaveChangesAsync();
                return AddCouponResult.Success(newMemberCoupon.CouponName);
            }
        }

        public async Task<IEnumerable<MemberCouponHasUsedDto>> GetCouponHasUsed(int? memberId)
        {
            var coupons =await _db.MemberCoupons.Include(mc=>mc.Coupon)
                                                .ThenInclude(c=>c.DiscountGroup)
										        .ThenInclude(dg => dg.DiscountGroupItems)
										        .ThenInclude(dgi => dgi.Dessert)
												.Where(mc => mc.MemberId == memberId)
                                                .Where(mc=>mc.UseAt!=null)
                                                .ToListAsync();
                                                
            var result = coupons.Where(mc =>((TimeSpan)(DateTime.Now - mc.UseAt)).TotalDays < 30)
								.Select(mc => mc.ToMemberCouponHasUsedDto());

			return result;
        }
        public async Task<DessertCart> GetDessertCart(int memberId)
        {
            var member = await _db.Members.FindAsync(memberId);
            var memberName = member.Username;
            var cart = await _db.DessertCarts.Include(dc => dc.DessertCartItems).ThenInclude(dci => dci.Dessert).ThenInclude(d => d.Discounts)
                                             .Include(dc=>dc.DessertCartItems).ThenInclude(dci=>dci.Specification)   
                                             .FirstOrDefaultAsync(dc => dc.Username == memberName);
            return cart;
        }

        public async Task<IEnumerable<Coupon>> GetPromotionCoupons()
        {
            var coupons =await _db.Promotions.Include(p=>p.Coupon).ThenInclude(c=>c.DiscountGroup).ThenInclude(d=>d.DiscountGroupItems).ThenInclude(dgi=>dgi.Dessert)
                                             .Where(p=>p.CouponId!=null)
                                             .Where(p=>p.LaunchAt<DateTime.Now&&p.EndAt>DateTime.Now)
                                             .Select (p=>p.Coupon)
                                             .Distinct()
                                             .ToListAsync();
            return coupons;
        }

        public async Task<IEnumerable<int>> GetAllMemberPromotionCoupon(int memberId)
        {
            var coupons = await _db.MemberCoupons.Include(mc=>mc.Coupon)
                                                 .ThenInclude(c=>c.Promotions)
                                                 .Where(mc=>mc.MemberId==memberId)
                                                 .Where(mc=>mc.Coupon.CouponCategoryId==2)
                                                 .Select(mc=>mc.CouponId)
                                                 .ToListAsync();
            return coupons;
        }
        public  bool IsMemberExist(int memberId)
        {
            var result =  _db.Members.Any(m=>m.Id==memberId);
            return result;
        }
		public  bool IsMemberCouponExist(int memberCouponId)
		{
			var result =  _db.MemberCoupons.Any(m => m.MemberCouponId == memberCouponId);
			return result;
		}
		public  bool IsThisMemberHaveThisCoupon(int memberId, int memberCouponId)
		{
            var coupon = _db.MemberCoupons.Find(memberCouponId);
            if (coupon.MemberId == memberId)
            {
                return true;
            }
            else return false;
		}
        public async Task<Coupon> GetMemberCouponById(int memberCouponId)
        {
            var coupon = await _db.MemberCoupons.Include(mc=>mc.Coupon)
                                                .ThenInclude(c=>c.DiscountGroup)
                                                .ThenInclude(d=>d.DiscountGroupItems)
                                                .ThenInclude(dgi=>dgi.Dessert)
                                                .FirstAsync(mc=>mc.MemberCouponId== memberCouponId);
            return coupon.Coupon;
        }
        public bool HasCouponBeenUsed(int memberCouponId)
        {
            var result = _db.MemberCoupons.Find(memberCouponId).UseAt==null? false: true;
            return result;
        }
        public bool IsPromotionCouponAndReady(int memberCouponId)
        {
            var coupon = _db.MemberCoupons.Include(mc=>mc.Coupon).First(mc=>mc.MemberCouponId == memberCouponId);
            if (coupon.Coupon.CouponCategoryId == 2)
            {
                if (coupon.Coupon.StartAt < DateTime.Now && coupon.Coupon.EndAt > DateTime.Now)
                {
                    return true;
                }else return false;
            }
            else
            {
                return true;
            }
                                          
        }

		public void RecordCouponInCart(DessertCart cart, int memberCouponId, int result)
		{
            var findCart = _db.DessertCarts.Find(cart.Id);
            findCart.MemberCouponId = memberCouponId;
            findCart.DiscountPrice = result;
            _db.SaveChanges();
		}

		public async Task<IEnumerable<CouponSetting>> GetPrizes()
		{
            var prizes = await _db.CouponSettings.Where(cs => cs.Qty != null && cs.CouponId!=null)
                                                 .ToListAsync();
            return prizes;
		}

		public async Task<bool> HasPlayedGame(int memberId)
		{
            var member = await _db.Members.FindAsync(memberId);
            return (bool)member.DailyGamePlayed;
		}
		public async Task<string> AddCouponAndRecordMemberPlay(int memberId, int resultCouponId)
		{
			using (var transaction = _db.Database.BeginTransaction())
            {
				var coupon = await _db.Coupons.FindAsync(resultCouponId);
				var member = await _db.Members.FindAsync(memberId);
				var newMemberCoupon = new MemberCoupon()
				{
					MemberId = memberId,
					CouponId = coupon.CouponId,
					CouponName = coupon.CouponName,
					CreateAt = DateTime.Now,
					ExpireAt = DateTime.Now.AddDays((double)coupon.Expiration)
				};
				_db.MemberCoupons.Add(newMemberCoupon);
				member.DailyGamePlayed = true;
				await _db.SaveChangesAsync();
				await transaction.CommitAsync();
				return newMemberCoupon.CouponName;
			}
			
		}

        public async Task<Coupon> FindCoupon(int couponId)
        {
            var coupom = await _db.Coupons.Include(c=>c.DiscountGroup).FirstOrDefaultAsync(c=>c.CouponId==couponId);
            return coupom;
        }
        public async Task<CouponSetting[]> GetWeeklyGameCouponSettings()
        {
            var couponSettings =await _db.CouponSettings.Where(cs=>cs.CouponType==4)
                                                        .OrderBy(cs=>cs.CouponSettingId)
                                                        .ToArrayAsync();
            return couponSettings;

		}
        public async Task<bool> HasPlayedWeeklyGame(int memberId)
        {
            var member = await _db.Members.FindAsync(memberId);
            return (bool)member.WeeklyGamePlayed;
        }
        public async Task AddCouponAndRecordMemberPlayWeeklyGame(int memberId, Coupon coupon)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                var member =await _db.Members.FindAsync(memberId);
                var newMemberCoupon = new MemberCoupon()
                {
                    MemberId = memberId,
                    CouponId = coupon.CouponId,
                    CouponName = coupon.CouponName,
                    CreateAt = DateTime.Now,
                    ExpireAt = DateTime.Now.AddDays((double)coupon.Expiration)
                };
                await _db.MemberCoupons.AddAsync(newMemberCoupon);
                member.WeeklyGamePlayed = true;
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
        }

		public async Task<IEnumerable<DiscountGroupItem>> FindSuggestProduct(int discountGroupId)
		{
			var desserts = await _db.DiscountGroupItems.Include(dgi => dgi.Dessert).ThenInclude(d=>d.DessertImages)
				                                      .Include(dgi => dgi.Dessert).ThenInclude(d=>d.Specifications)
				                                      .Where(dgi=>dgi.DiscountGroupId == discountGroupId)
                                                      .ToListAsync();
            return desserts;
		}

        public async Task<bool> CancelUsingCoupon(int memberId)
        {
            var member = await _db.Members.FindAsync(memberId);
            var memberName = member.Username;
            var cart = _db.DessertCarts.Where(c=>c.Username== memberName).FirstOrDefault();
            if(cart==null) return false;
            else
            {
                cart.DiscountPrice = null;
                cart.MemberCouponId = null;
                await _db.SaveChangesAsync();
                return true;
            }

        }

        public async Task<object?> GetUsingCoupon(int memberId)
        {
            var member = await _db.Members.FindAsync(memberId);
            var memberName = member.Username;
            var cart = _db.DessertCarts.Include(c=>c.MemberCoupon).Where(c => c.Username == memberName).FirstOrDefault();
            if(cart==null) return null;
            else
            {
                if (cart.MemberCouponId == null) return null;
                else
                {
                    return new { MemberCouponId = cart.MemberCouponId, DiscountPrice = cart.DiscountPrice };
                }
            }
        }

		public void LetMembersCanPlayDailyGame()
		{
            var members = _db.Members;
            foreach(var member in members)
            {
                member.DailyGamePlayed = false;
            }
            _db.SaveChanges();
		}

        public void LetMembersCanPlayWeeklyGame()
        {
            var members = _db.Members;
            foreach (var member in members)
            {
                member.WeeklyGamePlayed = false;
            }
            _db.SaveChanges();
        }

        public IEnumerable<Member> GetBirthdayMemberInThisMonth()
        {
            var month = DateTime.Now.Month;
            var members = _db.Members.Where(m=>m.Birth!=null).Where(m=>m.Birth.Value.Month==month);
            return members;
        }

        public Coupon GetBirthdayCoupon()
        {
            var coupon = _db.CouponSettings.Include(s=>s.Coupon).Where(s=>s.CouponType==1).First().Coupon; 
            return coupon;
        }

        public void AddBirthdayCoupons(IEnumerable<MemberCoupon> memberCoupons)
        {
            foreach(var memberCoupon in memberCoupons)
            {
                _db.MemberCoupons.Add(memberCoupon);
            }
            _db.SaveChanges();
        }
    }
}
