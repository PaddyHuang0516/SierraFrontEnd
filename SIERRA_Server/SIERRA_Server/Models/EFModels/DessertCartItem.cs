﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace SIERRA_Server.Models.EFModels
{
    public partial class DessertCartItem
    {
        public int Id { get; set; }
        public int SpecificationId { get; set; }
        public int DessertCartId { get; set; }
        public int DessertId { get; set; }
        public int Quantity { get; set; }
       

        public virtual Dessert Dessert { get; set; }
        public virtual DessertCart DessertCart { get; set; }
        public virtual Specification Specification { get; set; }
    }
}