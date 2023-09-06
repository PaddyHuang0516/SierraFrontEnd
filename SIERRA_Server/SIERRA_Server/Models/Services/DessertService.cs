﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SIERRA_Server.Models.DTOs;
using SIERRA_Server.Models.DTOs.Desserts;
using SIERRA_Server.Models.EFModels;
using SIERRA_Server.Models.Interfaces;
using System.Drawing.Printing;
using System.Threading.Tasks;

namespace SIERRA_Server.Models.Services
{
    public class DessertService
    {
        private IDessertRepository _repo;
        private IDessertDiscountRepository _discountrepo;

        public DessertService(IDessertRepository repo)
        {
            _repo = repo;
           
        }
        public DessertService(IDessertDiscountRepository discountrepo)
        {
          
            _discountrepo = discountrepo;
        }
        public async Task<List<DessertListDTO>> GetHotProductsAsync()
        {
            var hotdessert = await _repo.GetHotProductsAsync();
            return hotdessert;
        }
        public async Task<List<DessertsIndexDTO>> GetPresents()
        {
            var presents = await _repo.GetPresents();
            return presents;
        }
        public async Task<List<DessertsIndexDTO>> GetLongCake()
        {
            var longCake = await _repo.GetLongCake();
            return longCake;
        }
        public async Task<List<DessertsIndexDTO>> GetSnack()
        {
            var snack = await _repo.GetSnack();
            return snack;
        }
        public async Task<(List<DessertsIndexDTO> Desserts, int TotalPages)> GetMoldCake(int page = 1, int pageSize = 3)
        {
            var moldCake = await _repo.GetMoldCake();
            int totalMoldCake = moldCake.Count;
            int totalPages = (int)Math.Ceiling(totalMoldCake / 6.0); // 每六个甜点为一页
            moldCake = moldCake.Skip(pageSize * (page - 1)).Take(pageSize).ToList();
                   

            return (dessertsList, totalPages); // 返回甜点列表和总页数
        }
        public async Task<List<DessertsIndexDTO>> GetRoomTemperature()
        {
            var roomTemperature = await _repo.GetRoomTemperature();
            return roomTemperature;
        }
        public async Task<List<DessertDiscountDTO>> GetChocoDiscountGroups()
        {
            var chocoDiscount = await _discountrepo.GetDiscountGroupsByGroupId(6);
            return chocoDiscount;
        }
        public async Task<List<DessertDiscountDTO>> GetStrawberryDiscountGroups()
        {
            // 呼叫 DiscountGroupId 是 7 的 repository 
            var strawberryDiscount = await _discountrepo.GetDiscountGroupsByGroupId(7);
            return strawberryDiscount;
        }
        public async Task<List<DessertDiscountDTO>> GetMochaDiscountGroups()
        {
            //  呼叫 DiscountGroupId 是 8 的 repository  
            var mochaDiscount = await _discountrepo.GetDiscountGroupsByGroupId(8);
            return mochaDiscount;
        }
        public async Task<List<DessertDiscountDTO>> GetTaroDiscountGroups()
        {
            //  呼叫 DiscountGroupId 是 9 的 repository  
            var taroDiscount = await _discountrepo.GetDiscountGroupsByGroupId(9);
            return taroDiscount;
        }
        public async Task<List<DessertDiscountDTO>> GetAlcoholDiscountGroups()
        {
            //  呼叫 DiscountGroupId 是  10 的 repository 
            var alcoholDiscount = await _discountrepo.GetDiscountGroupsByGroupId(10);
            return alcoholDiscount;
        }

        public async Task<List<DessertDiscountDTO>> GetSuggestDiscountGroups(int dessertId)
        {
            var discountGroups = await _discountrepo.GetDiscountGroups();

            foreach (var discountGroup in discountGroups)
            {
                var dessertsInGroup = await _discountrepo.GetDiscountGroupsByGroupId(discountGroup.DiscountGroupId);

                if (dessertsInGroup.Any(dessert => dessert.DessertId == dessertId))
                {
                    return dessertsInGroup;
                }
            }

            // Default behavior if no matching dessert is found in any discount group
            // You might want to handle this case differently based on your requirements
            return await _discountrepo.GetTopSales();
        }
    }
}