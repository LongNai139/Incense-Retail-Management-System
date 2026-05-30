using Microsoft.EntityFrameworkCore;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers.Implements
{
    public class CategoryDAL : ICategoryDAL
    {
        private readonly ShopDbContext _context;

        public CategoryDAL(ShopDbContext context)
        {
            _context = context;
        }

        public List<Category> ListCategories(int take = 0)
        {
            var query = _context.Categories
                .AsNoTracking()
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.CategoryName);

            if (take > 0)
                return query.Take(take).ToList();

            return query.ToList();
        }

        public Category? GetById(int id)
        {
            return _context.Categories
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == id && !c.IsDeleted);
        }

        public int Add(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return category.Id;
        }

        public bool Update(Category category)
        {
            _context.Categories.Update(category);
            return _context.SaveChanges() > 0;
        }

        public bool Delete(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id && !c.IsDeleted);
            if (category == null)
                return false;

            category.IsDeleted = true;
            return _context.SaveChanges() > 0;
        }
    }
}
