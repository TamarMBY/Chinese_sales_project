using server.DTOs;
using server.Interfaces;
using server.Models;
using server.Repositories;

namespace server.Services
{
    public class CategoryService : ICategoryService

    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<IEnumerable<CategoryResponseDto>> GetAll()
        {
            var categories = await _categoryRepository.GetAll();
            return categories.Select(MapToResponeseDto);
        }

        public async Task<CategoryResponseDto> GetById(int id)
        {
            var category = await _categoryRepository.GetById(id);
            return category != null ? MapToResponeseDto(category) : null;
        }
        public async Task<CategoryResponseDto> AddCategory(CategoryCreateDto categoryDto)
        {
            var category = new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description,
            };
            var createdCategory = await _categoryRepository.AddCategory(category);
            return MapToResponeseDto(createdCategory);
        }

        public async Task<bool> DeleteCategory(int id)
        {
            return await _categoryRepository.DeleteCategory(id);
        }
        public async Task<CategoryResponseDto> UpdateCategory(int categoryId,CategoryUpdateDto categoryDto)
        {
            var existingCategory = await _categoryRepository.GetById(categoryId);
            if (existingCategory == null)
                return null;
            existingCategory.Name = categoryDto.Name;
            var updatedCategory = await _categoryRepository.UpdateCategory(existingCategory);
            return MapToResponeseDto(updatedCategory);
        }
        private static CategoryResponseDto MapToResponeseDto(Category category)
        {
            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
            };
        }
    }
}
