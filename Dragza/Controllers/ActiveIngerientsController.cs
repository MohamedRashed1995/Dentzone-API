using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Domain.Models;
using Dragza.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dragza.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActiveIngerientsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public ActiveIngerientsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateActiveIngredient([FromBody] ActiveIngredientDto dto)
        {
            var ing = new ActiveIngredient
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
            };
             await _unitOfWork.ActiveIngredientRepository.AddAsync(ing);
            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction(nameof(GetIngredient), new { id = ing.Id }, ing);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetIngredient(Guid id)
        {
            var ing = await _unitOfWork.ActiveIngredientRepository.GetByIdAsync(id);
            return Ok(ing);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllIngredients()
        {
            var products = await _unitOfWork.ActiveIngredientRepository.GetAllAsync();
            return Ok(products);
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateIngredients(Guid id,[FromBody] ActiveIngredientDto dto)
        {
            var ing = await _unitOfWork.ActiveIngredientRepository.GetByIdAsync(id);
            if (ing == null)
            {
                return NotFound();
            }
            ing.Name = dto.Name;
             _unitOfWork.ActiveIngredientRepository.Update(ing);
            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction(nameof(GetIngredient), new { id = ing.Id }, ing);
        }

        [HttpDelete("{id}")]
        //[Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ing = await _unitOfWork.ActiveIngredientRepository.GetAllAsync(o => o.Id == id, include: q => q.Include(o => o.Products));
            var activeIngredients = ing.FirstOrDefault();
            if (activeIngredients == null)
            {
                return NotFound();
            }
            if (activeIngredients.Products.Count > 0)
            {
                throw new InvalidOperationException("Please Remove Active Ingredient Products");
            }
            activeIngredients.IsDeleted = true; // Soft delete
            _unitOfWork.ActiveIngredientRepository.Update(activeIngredients);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            return NoContent();
        }
    }
}
