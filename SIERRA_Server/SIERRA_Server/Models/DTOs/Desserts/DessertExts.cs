﻿using SIERRA_Server.Models.EFModels;

namespace SIERRA_Server.Models.DTOs.Desserts
{
    public static class DessertExts
    {
        public static DessertListDTO ToDListDto(this Dessert entity)
        {
            int unitPrice = entity.Specifications.FirstOrDefault()?.UnitPrice ?? 0;
            return new DessertListDTO {            
                Dessert=entity,
                DessertId = entity.DessertId,
                DessertImageName = entity.DessertImages.FirstOrDefault().DessertImageName,
                DessertName = entity.DessertName,
                UnitPrice = unitPrice,
            };
        }

        public static DessertsIndexDTO ToDIndexDto(this Dessert entity)
        {
            int unitPrice = entity.Specifications.FirstOrDefault()?.UnitPrice ?? 0;
            return new DessertsIndexDTO
            {
                DessertId = entity.DessertId,
                DessertImageName = entity.DessertImages.FirstOrDefault().DessertImageName,
                DessertName = entity.DessertName,
                UnitPrice = unitPrice,
            };
        }
        public static DessertDiscountDTO ToDDiscountDto(this DiscountGroup entity)
        {
            var dessertDiscountDTO = new DessertDiscountDTO
            {
                DessertId = 0, // Dummy value, as it's not directly available in DiscountGroup
                DessertImageName = "", // Dummy value, as it's not directly available in DiscountGroup
                DessertName = entity.DiscountGroupName, // Assuming DiscountGroupName maps to DessertName
                UnitPrice = 0, // Dummy value, as it's not directly available in DiscountGroup
                DiscountGroupId = entity.DiscountGroupId,
                Specification = null // Placeholder for Specification, we'll fill it later
            };

            // Assuming there's only one DiscountGroupItem per DiscountGroup for simplicity
            var discountGroupItem = entity.DiscountGroupItems.FirstOrDefault();

            if (discountGroupItem != null && discountGroupItem.Dessert != null)
            {
                dessertDiscountDTO.DessertId = discountGroupItem.Dessert.DessertId;
                dessertDiscountDTO.DessertName = discountGroupItem.Dessert.DessertName;
                dessertDiscountDTO.DessertImageName = discountGroupItem.Dessert.DessertImages.FirstOrDefault()?.DessertImageName;
                dessertDiscountDTO.UnitPrice = discountGroupItem.Dessert.Specifications.FirstOrDefault()?.UnitPrice ?? 0;

                dessertDiscountDTO.Specification = new Specification
                {
                    UnitPrice = discountGroupItem.Dessert.Specifications.FirstOrDefault()?.UnitPrice ?? 0,
                    Flavor = discountGroupItem.Dessert.Specifications.FirstOrDefault()?.Flavor,
                    Size = discountGroupItem.Dessert.Specifications.FirstOrDefault()?.Size
                };
            }

            return dessertDiscountDTO;
        }
    }
}