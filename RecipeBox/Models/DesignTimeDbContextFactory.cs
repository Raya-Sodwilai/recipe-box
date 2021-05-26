using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace RecipeBox.Models
{
  public class RecipeBoxContextFactory : IDesignTimeDbContextFactory<RecipeBoxContext>
  {
    RecipeBoxContext IDesignTimeDbContextFactory<RecipeBoxContext>.CreateDbContext(string[] args)
    {
      IConfigurationRoot configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

      var builder = new DbContextOptionsBuilder<RecipeBoxContext>();
      var connectionString = configuration.GetConnectionString("DefaultConnection");

      builder.UseMySql(configuration["ConnectionStrings:DefaultConnection"], ServerVersion.AutoDetect(configuration["ConnectionStrings:DefaultConnection"]));

      return new RecipeBoxContext(builder.Options);
    }
  }
}