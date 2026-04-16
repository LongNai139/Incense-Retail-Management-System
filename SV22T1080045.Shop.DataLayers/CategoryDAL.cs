using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers
{
    public interface ICategoryDAL
    {
        List<Category> ListCategories(int take = 0);
    }

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
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.CategoryName);

            if (take > 0)
                return query.Take(take).ToList();

            return query.ToList();
        }
    }
}
