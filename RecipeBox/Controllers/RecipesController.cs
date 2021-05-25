using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RecipeBox.Models;
using System.Collections.Generic;
using System.Linq;
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

    public RecipesController(UserManager<ApplicationUser> userManager, RecipesContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    public async Task<ActionResult> Index()
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var userRecipes = _db.Recipes.OrderByDecending(Recipe => recipe.RecipeRating).ToList();
      return View(userRecipes);
    }
    
    public async Task<ActionResult> Create()
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      ViewBag.IngredientId = new MultiSelectList(_db.Ingredients, "IngredientId", "IngredientDescription");
      ViewBag.currentUser = userId;
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(Recipe recipe, List<int> IngredientId)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      recipe.User = currentUser;
      _db.Recipes.Add(recipe);
      if (IngredientId != 0)
      {
        foreach (int Ingredient in IngredientId)
        {
          _db.RecipeIngredient.Add(new RecipeIngredient() { IngredientId = IngredientId, RecipeId = recipe.RecipeId });
        }
      }
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
          .FirstOrDefault(recipe => recipe.RecipeId == id);
      ViewBag.currentUser = userId;
      return View(thisRecipe);
    }

    public async<ActionResult> Edit(int id)
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
      var thisRecipe = _db.Recipes.FirstOrDefault(Recipe => recipe.RecipeId == id);
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
  }
}