﻿namespace ThinkElectric.Web.Controllers;

using Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using ViewModels.Company;

public class CompanyController : Controller
{
    private readonly IImageService _imageService;
    private readonly ICompanyService _companyService;
    private readonly IAddressService _addressService;

    public CompanyController(IImageService imageService, ICompanyService companyService, IAddressService addressService)
    {
        _imageService = imageService;
        _companyService = companyService;
        _addressService = addressService;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CompanyCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (model.FoundedDate == null)
        {
            ModelState.AddModelError("FoundedDate", "Please enter valid date in the specific format.");
            return View(model);
        }

        if (model.ImageFile == null || model.ImageFile.Length == 0)
        {
            ModelState.AddModelError("ImageFile", "Image is required.");
            return View(model);
        }

        string imageType = Path.GetExtension(model.ImageFile.FileName);

        if (imageType != ".jpg" && imageType != ".jpeg" && imageType != ".png")
        {
            ModelState.AddModelError("ImageFile", "Image must be a .jpg, .jpeg, or .png file.");
            return View(model);
        }

        try
        {
            var imageId = await _imageService.CreateAsync(model.ImageFile);

            var address = await _addressService.CreateAsync(model.Address);

            await _companyService.CreateAsync(model, imageId, address, User.GetId()!);
        }
        catch (Exception)
        {
            ModelState.AddModelError("", "An unexpected error occurred while proceeding you request.");
        }

        return RedirectToAction("Index", "Home");
    }
}
