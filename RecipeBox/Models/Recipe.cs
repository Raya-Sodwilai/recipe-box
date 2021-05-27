using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecipeBox.Models
{
  public class Recipe
  {
    public Recipe()
    {
      this.Ingredients = new HashSet<RecipeIngredient>();
      // this.Tags = new HashSet<TagRecipe>();
    }

    public int RecipeId { get; set; }
    [Display(Name = "Recipe Name")]
    public string RecipeName { get; set; }
    [Display(Name = "Recipe Instructions")]
    public string RecipeInstructions { get; set; }
    [Display(Name = "Recipe Rating")]
    public int RecipeRating { get; set; }
    public virtual ApplicationUser User { get; set; }
    public virtual ICollection<RecipeIngredient> Ingredients { get; set; }
    // public virtual ICollection<TagRecipe> Tags { get; set; }
  }
}