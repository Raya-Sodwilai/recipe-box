using System.Collections.Generic;

namespace RecipeBox.Models
{
  public class Ingredient
  {
    public Ingredient()
    {
      this.Recipes = new HashSet<RecipeIngredient>();
    }

    public int IngredientId { get; set; }
    public string IngredientDescription { get; set; }
    public virtual ApplicationUser User { get; set; }
    public ICollection<RecipeIngredient> Recipes { get; }
  }
}