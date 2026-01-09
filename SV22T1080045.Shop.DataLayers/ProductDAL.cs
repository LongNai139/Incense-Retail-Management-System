using Dapper;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers
{
    public class ProductDAL : BaseDAL
    {
        public ProductDAL(string connectionString) : base(connectionString) { }

        // Lấy danh sách danh mục (Để hiển thị Dropdown lọc)
        public List<Category> GetCategories()
        {
            using var conn = OpenConnection();
            return conn.Query<Category>("SELECT * FROM Categories").ToList();
        }

        // Tìm kiếm và Lọc sản phẩm
        public List<Product> ListProducts(string searchValue = "", int categoryID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            using var conn = OpenConnection();

            // Kỹ thuật nối chuỗi SQL động
            var sql = "SELECT * FROM Products WHERE 1=1";
            var parameters = new DynamicParameters();

            // Lọc theo tên
            if (!string.IsNullOrEmpty(searchValue))
            {
                sql += " AND ProductName LIKE @Search";
                parameters.Add("@Search", $"%{searchValue}%");
            }

            // Lọc theo loại
            if (categoryID > 0)
            {
                sql += " AND CategoryID = @CategoryID";
                parameters.Add("@CategoryID", categoryID);
            }

            // Lọc theo giá
            if (minPrice > 0)
            {
                sql += " AND Price >= @MinPrice";
                parameters.Add("@MinPrice", minPrice);
            }
            if (maxPrice > 0)
            {
                sql += " AND Price <= @MaxPrice";
                parameters.Add("@MaxPrice", maxPrice);
            }

            sql += " ORDER BY ProductID DESC"; // Sản phẩm mới nhất lên đầu

            return conn.Query<Product>(sql, parameters).ToList();
        }

        // Lấy chi tiết 1 sản phẩm
        public Product? GetProduct(int id)
        {
            using var conn = OpenConnection();
            var sql = "SELECT * FROM Products WHERE ProductID = @Id";
            return conn.QueryFirstOrDefault<Product>(sql, new { Id = id });
        }
    }
}