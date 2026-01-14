using server.DTOs;
using server.Models;

namespace server.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAll();
        Task<CategoryResponseDto> GetById(int id);
        Task<CategoryResponseDto> AddCategory(CategoryCreateDto category);
        Task<bool> DeleteCategory(int id);
        Task<CategoryResponseDto> UpdateCategory(int categoryId, CategoryUpdateDto updateDto);
        //Task<Category> UpdateCategory(UpdateCategoryDto category);
    }
}
