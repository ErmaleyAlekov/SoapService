using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MySoapService.Abstracts;

namespace MySoapService.Products
{
    public class ProductDto : Product, IDb<ProductDto>
    {
        public ProductDto() : base("Small Skirt", "Cloth", "Skirt", "Nike", "Female", 0) { }
        public ProductDto(string n, string t, string st, string c, string g, int p) : base(n, t, st, c, g, p) { }

        public DbSet<ProductDto> getAll()
        {
            NewContext<ProductDto> DbContext = new NewContext<ProductDto>();
            return DbContext.Set<ProductDto>();
        }

        public ProductDto? getOne(uint id)
        {
            foreach (var item in getAll())
            {
                if (item.Id == id)
                    return item;
            }
            return null;
        }
        public bool save(ProductDto product)
        {
            try
            {
                using (NewContext<ProductDto> db = new NewContext<ProductDto>())
                {
                    db.Add(product);
                    db.SaveChanges();
                }
                return true;
            }
            catch {
                return false;
            }
        }
    }
    public class Cloth : Product ,IDb<Cloth>
    {
        [Column("Color")]
        public string Color { get; set; }
        public Cloth() : base("Red Skirt", "Cloth", "Skirt", "Nike", "Female", 0)
        {
            Color = "default";
        }
        public Cloth(string n, string t, string st, string c, string g, int p, string color) : base(n, t, st, c, g, p)
        {
            Color = color;
        }

        public DbSet<Cloth> getAll()
        {
            NewContext<Cloth> DbContext = new NewContext<Cloth>();
            return DbContext.Set<Cloth>();
        }

        public Cloth? getOne(uint id)
        {
            foreach (var item in getAll())
            {
                if (item.Id == id)
                    return item;
            }
            return null;
        }
        public bool save(Cloth product)
        {
            try
            {
                using (NewContext<Cloth> db = new NewContext<Cloth>())
                {
                    db.Add(product);
                    db.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

    }

    public class NewTestProduct : Product, IDb<NewTestProduct>
    {
        public string Description { get; set; }
        public NewTestProduct() : base("Red Skirt", "Cloth", "Skirt", "Nike", "Female", 0)
        {
            Description = "default";
        }

        public NewTestProduct(string n, string t, string st, string c, string g, int p, string des) : base(n, t, st, c, g, p)
        {
            Description = des;
        }

        public DbSet<NewTestProduct> getAll()
        {
            NewContext<NewTestProduct> DbContext = new NewContext<NewTestProduct>();
            return DbContext.Set<NewTestProduct>();
        }

        public NewTestProduct? getOne(uint id)
        {

            DbSet<NewTestProduct> all = getAll();
            foreach (var item in all)
            {
                if (item.Id == id)
                    return item;
            }
            return null;
        }

        public bool save(NewTestProduct product)
        {
            try
            {
                using (NewContext<NewTestProduct> db = new NewContext<NewTestProduct>())
                {
                    db.Add(product);
                    db.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
