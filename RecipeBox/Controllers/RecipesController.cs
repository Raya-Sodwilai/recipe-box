using Microsoft.AspNetCore.Mvc;
using RecipeBox.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace RecipeBox.Controllers
{
  [Authorize]
  public class RecipesController : Controller
  {
    private readonly RecipeBoxContext _db;
    private readonly UserManager<ApplicationUser> _userManager; 

    public RecipesController(UserManager<ApplicationUser> userManager, RecipeBoxContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    public async Task<ActionResult> Index()
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var userRecipes = _db.Recipes.OrderByDescending(recipe => recipe.RecipeRating).ToList();
      return View(userRecipes);
    }

    public async Task<ActionResult> Create()
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      // ViewBag.TagId = new MultiSelectList(_db.Tags, "TagId", "TagDescription");
      ViewBag.IngredientId = new MultiSelectList(_db.Ingredients, "IngredientId", "IngredientDescription");
      ViewBag.CurrentUser = userId;
      return View();
    }

    [HttpPost]
    public async Task<ActionResult>  Create(Recipe recipe, int IngredientId)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      recipe.User = currentUser;
      _db.Recipes.Add(recipe);
      if (IngredientId != 0)
      {
          _db.RecipeIngredient.Add(new RecipeIngredient() { IngredientId = IngredientId, RecipeId = recipe.RecipeId });  
      }
      // if (TagId.Count != 0)
      // {
      //   foreach (int Tag in TagId)
      //   {
      //     _db.TagRecipe.Add(new TagRecipe() { TagId = Tag, RecipeId = recipe.RecipeId });
      //   }   
      // }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public async Task<ActionResult> Details(int id)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var thisRecipe = _db.Recipes
        .Include(recipe => recipe.User)
        .Include(recipe => recipe.Ingredients)
        .ThenInclude(join => join.Ingredient)
        // .Include(recipe => recipe.Tags)
        // .ThenInclude(join => join.Tag)
        .FirstOrDefault(recipe => recipe.RecipeId == id);
      ViewBag.currentUser = userId;
      return View(thisRecipe);
    }

    public ActionResult Edit(int id)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var thisRecipe = _db.Recipes.Include(recipe => recipe.User).FirstOrDefault(recipe => recipe.RecipeId == id);
      if (userId != thisRecipe.User.Id) 
      {
        return RedirectToAction("Details", new {id = id});
      }
      return View(thisRecipe);
    }

    [HttpPost]
    public ActionResult Edit(Recipe recipe)
    {
      _db.Entry(recipe).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    // public ActionResult AddTag(int id)
    // {
    //   var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    //   var thisRecipe = _db.Recipes.FirstOrDefault(recipes => recipes.RecipeId == id);
    //   if (userId != thisRecipe.User.Id) 
    //   {
    //     return RedirectToAction("Details", new {id = id});
    //   }
    //   ViewBag.TagId = new SelectList(_db.Tags, "TagId", "TagDescription");
    //   return View(thisRecipe);
    // }

    // [HttpPost]
    // public ActionResult AddTag(Recipe recipe, int TagId)
    // {
    //   TagRecipe join = _db.TagRecipe.FirstOrDefault(tagrecipe => tagrecipe.TagId == TagId && tagrecipe.RecipeId == recipe.RecipeId);
    //   if (TagId != 0 && join == null)
    //   {
    //     _db.TagRecipe.Add(new TagRecipe() { TagId = TagId, RecipeId = recipe.RecipeId });
    //   }
    //   _db.SaveChanges();
    //   return RedirectToAction("Index");
    // }
    public ActionResult Delete(int id)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var thisRecipe = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == id);
      if (userId != thisRecipe.User.Id) 
      {
        return RedirectToAction("Details", new {id = id});
      }
      return View(thisRecipe);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisRecipe = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == id);
      _db.Recipes.Remove(thisRecipe);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult AddIngredient(int id)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var thisRecipe = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == id);
      if (userId != thisRecipe.User.Id) 
      {
        return RedirectToAction("Details", new {id = id});
      }
      ViewBag.IngredientId = new SelectList(_db.Ingredients, "IngredientId", "IngredientDescription");
      ViewBag.I = _db.RecipeIngredient.Include(recIng => recIng.Ingredient).Where(recIng => recIng.RecipeId != id);
      return View(thisRecipe);
    }

    [HttpPost]
    public ActionResult AddIngredient(Recipe recipe, int IngredientId)
    {
      RecipeIngredient join = _db.RecipeIngredient.FirstOrDefault(recipeIng => recipeIng.IngredientId == IngredientId && recipeIng.RecipeId == recipe.RecipeId);
      if (IngredientId != 0 && join == null)
      {
        _db.RecipeIngredient.Add(new RecipeIngredient() { IngredientId = IngredientId, RecipeId = recipe.RecipeId });
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult DeleteIngredient(int id)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var thisRecipe = _db.Recipes.Include(recipes => recipes.Ingredients).FirstOrDefault(recipe => recipe.RecipeId == id);
      if (userId != thisRecipe.User.Id) 
      {
        return RedirectToAction("Details", new {id = id});
      }
      ViewBag.IngredientId = new SelectList(_db.RecipeIngredient.Include(recipeIngredient => recipeIngredient.Ingredient).Where(recipeIngredient => recipeIngredient.RecipeId == id), "Ingredient.IngredientId", "Ingredient.IngredientDescription");
      return View(thisRecipe);
    }
    [HttpPost]
    public ActionResult DeleteIngredient(Recipe recipe, int IngredientId)
    {
      RecipeIngredient join = _db.RecipeIngredient.FirstOrDefault(recipeIngredient => recipeIngredient.IngredientId == IngredientId && recipeIngredient.RecipeId == recipe.RecipeId);
      _db.RecipeIngredient.Remove(join);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
    //  public ActionResult DeleteTag(int id)
    // {
    //   var thisRecipe = _db.Recipes.Include(recipes => recipes.Tags).FirstOrDefault(recipe => recipe.RecipeId == id);
    //   ViewBag.TagId = new SelectList(_db.TagRecipe.Include(tagrecipe => tagrecipe.Tag).Where(tagrecipe => tagrecipe.RecipeId == id), "Tag.TagId", "Tag.TagDescription");
    //   return View(thisRecipe);
    // }
    // [HttpPost]
    // public ActionResult DeleteTag(Recipe recipe, int TagId)
    // {
    //   TagRecipe join = _db.TagRecipe.FirstOrDefault(recipeTag => recipeTag.TagId == TagId && recipeTag.RecipeId == recipe.RecipeId);
    //   _db.TagRecipe.Remove(join);
    //   _db.SaveChanges();
    //   return RedirectToAction("Index");
    // }
  }
}