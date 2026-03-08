using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace dotnetapp.Models
{
   public class CookingClassRequest
    {
        [Key]
        public int CookingClassRequestId { get; set; }

        public int UserId { get; set; }

        [JsonIgnore]
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public int CookingClassId { get; set; }

        [JsonIgnore]
        [ForeignKey("CookingClassId")]
        public CookingClass? CookingClass { get; set; }

        public string RequestDate { get; set; }

        public string Status { get; set; }

        public string DietaryPreferences { get; set; }

        public string CookingGoals { get; set; }

        public string? Comments { get; set; }
    }
}