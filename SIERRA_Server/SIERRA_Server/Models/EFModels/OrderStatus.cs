﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace SIERRA_Server.Models.EFModels
{
    public partial class OrderStatus
    {
        public OrderStatus()
        {
            DessertOrders = new HashSet<DessertOrder>();
            LessonOrders = new HashSet<LessonOrder>();
        }

        public int OrderStatusId { get; set; }
        public string StatusName { get; set; }

        public virtual ICollection<DessertOrder> DessertOrders { get; set; }
        public virtual ICollection<LessonOrder> LessonOrders { get; set; }
    }
}