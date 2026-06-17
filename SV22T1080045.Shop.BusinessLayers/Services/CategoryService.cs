using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryDAL _categoryDal;

        public CategoryService(ICategoryDAL categoryDal)
        {
            _categoryDal = categoryDal;
        }

        public List<Category> ListCategories(int take = 0)
            => _categoryDal.ListCategories(take);

        public Category? GetById(int id)
            => _categoryDal.GetById(id);
    }
}
