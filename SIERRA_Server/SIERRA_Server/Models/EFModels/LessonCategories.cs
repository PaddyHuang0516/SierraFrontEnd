﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace SIERRA_Server.Models.EFModels
{
    public partial class LessonCategories
    {
        public LessonCategories()
        {
            Lessons = new HashSet<Lessons>();
        }

        public int LessonCategoryId { get; set; }
        public string LessonCategoryName { get; set; }

        public virtual ICollection<Lessons> Lessons { get; set; }
    }
}