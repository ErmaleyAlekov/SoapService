using Microsoft.EntityFrameworkCore;
using MySoapService.Products;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ServiceModel;

namespace MySoapService.Abstracts
{
    public abstract class Product
    {
        public Product(string n, string t, string st, string c, string g, int p)
        {
            Name = n;
            Type = t;
            Subtype = st;
            Company = c;
            Gender = g;
            Price = p;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public uint Id { get; set; }
        [Column("product_name")]
        public string Name { get; set; }

        public string Type { get; set; }

        public string Subtype { get; set; }
        public string Company { get; set; }
        public int Price { get; set; }
        public string Gender { get; set; }

    }

    public interface IDb<T> where T : class { 
        public DbSet<T> getAll();
        public T? getOne(uint id);
        public bool save(T entity);
    }

    [ServiceContract]
    public interface ISampleService
    {
        [OperationContract]
        public void test(string[] names, string[] values);

        [OperationContract]
        public object getProductsByClassName(string className);
        [OperationContract]
        public object getProduct(string className, uint id);
        [OperationContract]
        public object create(string className, string[] names, string[] values);
    }
}
