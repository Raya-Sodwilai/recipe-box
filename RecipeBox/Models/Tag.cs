using System.Collections.Generic;

namespace RecipeBox.Models
{
  public class Tag
  {
    public Tag()
    {
      this.Recipes = new HashSet<TagRecipe>();
    }

    public int TagId { get; set; }
    public string TagDescription { get; set; }
    public virtual ApplicationUser User { get; set; } 

    public ICollection<TagRecipe> Recipes { get;}
  }
}