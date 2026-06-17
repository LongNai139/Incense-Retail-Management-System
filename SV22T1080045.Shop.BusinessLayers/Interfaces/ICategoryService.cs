using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers.Interfaces
{
    public interface ICategoryService
    {
        List<Category> ListCategories(int take = 0);
        Category? GetById(int id);
    }
}
