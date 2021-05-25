using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecipeBox.Models
{
  public class Recipe
  {
    public Recipe()
    {
      this.Ingredients = new HashSet<RecipeIngredient>();
    }

    public int RecipeId { get; set; }
    [Display(name = "Recipe Name")]
    public string RecipeName { get; set; }
    [Display(name = "Recipe Instructions")]
    public string RecipeInstructions { get; set; }
    [Display(name = "Recipe Rating")]
    public int RecipeRating { get; set; }
    public virtual ApplicationUser User { get; set; }
    public virtual ICollection<RecipeIngredient> Ingredients { get; set; }
  }
}