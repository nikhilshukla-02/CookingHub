using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetapp.Models
{
    public class CookingClass
    {
        public int CookingClassId{get;set;}
        public string ClassName{get;set;}
        public string CuisineType{get;set;}
        public string ChefName{get;set;}
        public string Location{get;set;}
        public int DurationInHours{get;set;}
        public decimal Fee {get;set;}
        public string IngredientsProvided{get;set;}
        public string SkillLevel{get;set;}
        public string SpecialRequirements{get;set;}

    }
}