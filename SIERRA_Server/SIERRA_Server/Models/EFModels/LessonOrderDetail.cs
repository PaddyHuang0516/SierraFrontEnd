﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace SIERRA_Server.Models.EFModels
{
    public partial class LessonOrderDetail
    {
        public int Id { get; set; }
        public int LessonOrderId { get; set; }
        public int LessonId { get; set; }
        public string LessonTitle { get; set; }
        public int NumberOfPeople { get; set; }
        public int LessonUnitPrice { get; set; }
        public int Subtotal { get; set; }

        public virtual Lesson Lesson { get; set; }
        public virtual LessonOrder LessonOrder { get; set; }
    }
}