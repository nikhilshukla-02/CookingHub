using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnetapp.Data;
using dotnetapp.Exceptions;
using dotnetapp.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnetapp.Services
{
    public class CookingClassService
    {
        private readonly ApplicationDbContext _context;

        // Constructor MUST match: only ApplicationDbContext parameter
        public CookingClassService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CookingClass>> GetAllCookingClasses()
        {
            return await _context.CookingClasses.ToListAsync();
        }

        public async Task<CookingClass> GetCookingClassById(int cookingId)
        {
            return await _context.CookingClasses
                .FirstOrDefaultAsync(c => c.CookingClassId == cookingId);
        }

        public async Task<bool> AddCookingClass(CookingClass cooking)
        {
            var exists = await _context.CookingClasses
                .AnyAsync(c => c.ClassName == cooking.ClassName);

            if (exists)
            {
                throw new CookingClassException(
                    "Cooking class with the same name already exists");
            }

            _context.CookingClasses.Add(cooking);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCookingClass(int cookingId, CookingClass cooking)
        {
            var existingClass = await _context.CookingClasses
                .FirstOrDefaultAsync(c => c.CookingClassId == cookingId);

            if (existingClass == null)
            {
                return false;
            }

            var nameExists = await _context.CookingClasses
                .AnyAsync(c => c.ClassName == cooking.ClassName 
                    && c.CookingClassId != cookingId);

            if (nameExists)
            {
                throw new CookingClassException(
                    "Cooking class with the same name already exists");
            }

            existingClass.ClassName = cooking.ClassName;
            existingClass.CuisineType = cooking.CuisineType;
            existingClass.ChefName = cooking.ChefName;
            existingClass.Location = cooking.Location;
            existingClass.DurationInHours = cooking.DurationInHours;
            existingClass.Fee = cooking.Fee;
            existingClass.IngredientsProvided = cooking.IngredientsProvided;
            existingClass.SkillLevel = cooking.SkillLevel;
            existingClass.SpecialRequirements = cooking.SpecialRequirements;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCookingClass(int cookingId)
        {
            var cookingClass = await _context.CookingClasses
                .FirstOrDefaultAsync(c => c.CookingClassId == cookingId);

            if (cookingClass == null)
            {
                return false;
            }

            var isReferenced = await _context.CookingClassRequests
                .AnyAsync(r => r.CookingClassId == cookingId);

            if (isReferenced)
            {
                throw new CookingClassException(
                    "Cooking class cannot be deleted as it is referenced in a request");
            }

            _context.CookingClasses.Remove(cookingClass);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

