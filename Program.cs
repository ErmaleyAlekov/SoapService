using Microsoft.Extensions.DependencyInjection.Extensions;
using SoapCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using MySoapService.Products;
using MySoapService.Abstracts;


namespace MySoapService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var app = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://localhost:8080")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Soap>()
                .ConfigureLogging(x =>
                {
                    x.AddDebug();
                    x.AddConsole();
                })
                .Build();
            app.Run();
        }
    }

    public static class Constants
    {
        public static string? connection = WebApplication.CreateBuilder().Configuration.GetConnectionString("OracleConnection") != null ?
            WebApplication.CreateBuilder().Configuration.GetConnectionString("OracleConnection") : @"User Id=system;Password=123;Data Source = 192.168.1.82:1521/XE";
    }

    public class Soap
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSoapCore();
            services.TryAddTransient<ISampleService, SampleService>();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints => {
                endpoints.UseSoapEndpoint<ISampleService>("/MyService.svc", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);
                endpoints.UseSoapEndpoint<ISampleService>("/MyService.asmx", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);
            });
        }
    }
    public class NewContext<T> : DbContext where T : class
    {
        public NewContext()
        {
            try
            {
                Database.EnsureDeleted();
                Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseOracle(Constants.connection == null ? @"User Id=system;Password=123;Data Source = 192.168.1.82:1521/XE" : Constants.connection);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<T>().UseTpcMappingStrategy();
        }

    }
    public class SampleService : ISampleService
    {
        public SampleService()
        {
            
        }

        /*public List<ProductDto> GetProductsByProperty(string propName,string property)
        {
            List<ProductDto> result = new List<ProductDto>();
            foreach (var item in this.AppContext.Set<ProductDto>())
            {
                Type type = item.GetType();
                PropertyInfo[] properties = type.GetProperties();
                foreach (var item1 in properties)
                {
                    if (item1.Name.ToLower() == propName.ToLower())
                    {
                        var val = item1.GetValue(item, null);
                
                        if (val?.ToString() == property)
                            result.Add(item);
                    }                       
                }
            }
            return result;
        }*/

        public object create(string className,string[] names, string[] values)
        {
            Type? type;
            object? obj;
            getTypeAndObject(className, out type, out obj);
            if (values.Length == names.Length && type != null && obj != null)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    PropertyInfo? mi = type.GetProperty(names[i]);
                    if (mi != null)
                    {
                        if (mi.PropertyType != values[i].GetType())
                            mi.SetValue(obj, Convert.ChangeType(values[i], mi.PropertyType));
                        else
                            mi.SetValue(obj, values[i]);
                    }
                }
                MethodInfo? m = type.GetMethod("save");
                object? res;
                if (m != null)
                {
                    res= m.Invoke(obj, new object[] { obj });
                    if (res != null && (bool)res == true)
                        return true;
                    else
                        return false;
                }
            }
            return "Wrong params data";
        }

        public void test(string[] names, string[] values)
        {
            int i = 0;
        }

        public object getProductsByClassName(string name)
        {
            try
            {
                Type? type;
                object? obj;
                getTypeAndObject(name,out type,out obj);
                if (obj != null && type != null && type.Name != "System")
                {
                    MethodInfo? mi = type.GetMethod("getAll");
                    if (mi != null)
                        obj = mi.Invoke(obj,null);
                    if (obj != null)
                        return obj;
                }
                return "Cannot find products by this name " + name;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public object getProduct(string className, uint id)
        {
            try
            {
                Type? type;
                object? obj;
                getTypeAndObject(className, out type, out obj);
                if (obj != null && type != null && type.Name != "System")
                {
                    MethodInfo? mi = type.GetMethod("getOne");
                    if (mi != null)
                        obj = mi.Invoke(obj, new object[] {id});
                    if (obj != null)
                        return obj;
                }
                return "Cannot find product";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public Type[] getAllProductTypes()
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                  .Where(t => string.Equals(t.Namespace, "MySoapService.Products", StringComparison.Ordinal))
                  .ToArray();
        }

        public List<string> getAllProductTypeNames()
        {
            List<string> result = new();
            foreach (var item in getAllProductTypes())
            {
                result.Add(item.Name);
            }
            return result;
        }
        public void getTypeAndObject(string name, out Type? t, out object? obj)
        {
            try
            {
                foreach (var item in getAllProductTypes())
                {
                    if (item.Name == name)
                    {
                        t= Type.GetType("MySoapService.Products." + name);
                        if (t != null)
                        {
                            obj = Activator.CreateInstance(t);
                            return;
                        }
                    }
                }
                t = Type.GetType("System");
                obj = Activator.CreateInstance(t);
            }
            catch
            {
                t = Type.GetType("System");
                obj = Activator.CreateInstance(t);
            }
        }


        public string AddProduct<T> (T entity) where T : class
        {
            try
            {
                using (NewContext<T> DbContext = new NewContext<T>())
                {
                    DbContext.Add(entity);
                    DbContext.SaveChanges();
                    return "OK";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ex.Message;
            }
        }
    }
}