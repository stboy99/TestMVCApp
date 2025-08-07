// Controllers/PersonController.cs
using Microsoft.AspNetCore.Mvc;
using TestMVCApp.Models;
using TestMVCApp.Models.Global;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

//[ApiController] force json responses and disable views rendering
[Authorize]
[Route("Person")]
public class PersonController : Controller
{
    private readonly PersonRepository _repository;
    private readonly ILogger<PersonController> _logger;

    public PersonController(PersonRepository repository, ILogger<PersonController> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    [HttpGet("Index")]
    public async Task<IActionResult> Index()
    {
        var people = await _repository.GetAllAsync();
        return View("Index", people);
    }
    [HttpGet("Add")]
    public IActionResult Add()
    {
        var model = new PersonFormViewModel
        {
            FormAction = "/Person/Add",
            ButtonText = "Add",
            Person = new Person(), // empty
            Fields = new List<FormField>
        {
            new() { Name = "Name", Label = "Full Name", Type = "text", IsRequired = true },
            new() { Name = "Email", Label = "Email Address", Type = "email", IsRequired = true },
            new() { Name = "Age", Label = "Age", Type = "number", IsRequired = true },
            new()
            {
                Name = "Gender",
                Label = "Gender",
                Type = "select",
                IsRequired = false,
                Options = new List<SelectOption>
                {
                    new() { Value = "M", Text = "Male" },
                    new() { Value = "F", Text = "Female" },
                    new() { Value = "O", Text = "Other" }
                }
            }
        }
        };

        return View("Add", model);
    }

    [HttpPost("Add")]
    public async Task<IActionResult> Add(Person person)
    {
        _logger.LogInformation("Adding a new person: {Name}, Age: {Age}, Email: {Email}", person.Name, person.Age, person.Email);
        try
        {
            await _repository.AddAsync(person);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding a new person.");
            ModelState.AddModelError("", "An error occurred while adding the person. Please try again.");
            return View(person);
        }
    }
    [HttpGet("Edit/{Id}")]
    public async Task<IActionResult> Edit(int Id)
    {
        var person = await _repository.GetByIdAsync(Id);
        if (person == null)
            return NotFound();

        var model = new PersonFormViewModel
        {
            FormAction = "/Person/Edit", // will generate /Person/Edit
            ButtonText = "Update",
            Person = person,
            Fields = new List<FormField>
        {
            new() { Name = "Id", Type = "hidden", Value = person.Id.ToString() },
            new() { Name = "Name", Label = "Full Name", Type = "text", IsRequired = true, Value = person.Name },
            new() { Name = "Email", Label = "Email Address", Type = "email", IsRequired = true, Value = person.Email },
            new() { Name = "Age", Label = "Age", Type = "number", IsRequired = true, Value = person.Age.ToString() },
            new()
            {
                Name = "Gender",
                Label = "Gender",
                Type = "select",
                IsRequired = false,
                Options = new List<SelectOption>
                {
                    new() { Value = "M", Text = "Male" },
                    new() { Value = "F", Text = "Female" },
                    new() { Value = "O", Text = "Other" }
                }
            }
        }
        };

        return View("Edit", model);
    }


    [HttpPost("Edit")]
    public async Task<IActionResult> Edit(Person person)
    {
        if (!ModelState.IsValid)
            return View(person);

        _logger.LogInformation("Editing person: {Name}", person.Name);
        await _repository.EditAsync(person);
        return RedirectToAction("Index");
    }

    [HttpPost("Delete/{Id}")]
    public async Task<IActionResult> Delete(int Id)
    {
        try
        {
            _logger.LogInformation("Deleting person with Id: {Id}", Id);
            await _repository.DeleteAsync(Id);
            return RedirectToAction("Index"); // back to the view
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Delete failed");
            return StatusCode(500, "An error occurred while deleting the person.");
        }
    }
}
