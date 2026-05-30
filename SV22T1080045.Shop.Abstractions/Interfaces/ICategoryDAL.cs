using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.Abstractions.Interfaces
{
    public interface ICategoryDAL
    {
        List<Category> ListCategories(int take = 0);
        Category? GetById(int id);
        int Add(Category category);
        bool Update(Category category);
        bool Delete(int id);
    }
}
