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
  public class IngredientsController : Controller
  {
    private readonly RecipeBoxContext _db;
    private readonly UserManager<ApplicationUser> _useManager;
    public IngredientsController(UserManager<ApplicationUser> userManager, RecipeBoxContext db)
    {
      _useManager = userManager;
      _db = db;
    }

    public ActionResult Index()
    {
      return View(_db.Ingredients.RecipeBox());
    }

    public ActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public ActionResult Create(Ingredient ingredient)
    {
      if (_db.Ingredients.FirstOrDefault(ing => ing.IngredientDescription == ingredient.IngredientDescription) == null)
      {
        _db.Ingredients.Add(ingredient);
        _db.SaveChanges();
      }
      
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
      var thisIngredient = _db.Ingredients
          .Include(ingredient => ingredient.Recipes)
          .ThenInclude(join => join.Recipe)
          .FirstOrDefault(ingredient => ingredient.IngredientId == id);
      return View(thisIngredient);
    }
    public ActionResult Edit(int id)
    {
      var thisIngredient = _db.Ingredients.FirstOrDefault(ingredients => ingredients.IngredientId == id);
      return View(thisIngredient);
    }

    [HttpPost]
    public ActionResult Edit(Ingredient ingredient)
    {
      if (_db.Ingredients.FirstOrDefault(ing => ing.IngredientDescription == ingredient.IngredientDescription) ==null)
      {
        _db.Entry(ingredient).State = EntityState.Modified;
        _db.SaveChanges();
      }
      
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
      var thisIngredient = _db.Ingredients.FirstOrDefault(ingredients => ingredients.IngredientId == id);
      return View(thisIngredient);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisIngredient = _db.Ingredients.FirstOrDefault(ingredients => ingredients.IngredientId == id);
      _db.Ingredients.Remove(thisIngredient);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}