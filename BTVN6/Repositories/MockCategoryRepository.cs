using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BTVN6.Models;

namespace BTVN6.Repositories
{
    public class MockCategoryRepository : ICategoryRepository
    {
        private List<Category> _categoryList;

        public MockCategoryRepository()
        {
            _categoryList = new List<Category>
            {
                new Category { Id = 1, Name = "Laptop" },
                new Category { Id = 2, Name = "Desktop" },
                new Category { Id = 3, Name = "Accessories" }
            };
        }

        public Task<IEnumerable<Category>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Category>>(_categoryList);
        }

        public Task<Category?> GetByIdAsync(int id)
        {
            var category = _categoryList.FirstOrDefault(c => c.Id == id);
            return Task.FromResult(category);
        }

        public Task AddAsync(Category category)
        {
            category.Id = _categoryList.Max(c => c.Id) + 1;
            _categoryList.Add(category);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Category category)
        {
            var index = _categoryList.FindIndex(c => c.Id == category.Id);
            if (index != -1)
            {
                _categoryList[index] = category;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            var category = _categoryList.FirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                _categoryList.Remove(category);
            }
            return Task.CompletedTask;
        }
    }
}
