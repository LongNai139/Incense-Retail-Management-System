using SV22T1080045.Shop.DataLayers;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers
{
    public interface ICategoryService
    {
        List<Category> ListCategories(int take = 0);
    }

    public class CategoryService : ICategoryService
    {
        private readonly ICategoryDAL _categoryDAL;

        public CategoryService(ICategoryDAL categoryDAL)
        {
            _categoryDAL = categoryDAL;
        }

        public List<Category> ListCategories(int take = 0)
        {
            return _categoryDAL.ListCategories(take);
        }
    }
}
