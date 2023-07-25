﻿namespace ThinkElectric.Web.Controllers;

using Data.Models.Enums.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using ViewModels.Scooter;
using static Common.ErrorMessages;
using static Common.NotificationsMessagesConstants;
using static Common.GeneralMessages;

[Authorize]
public class ScooterController : Controller
{
    private readonly IScooterService _scooterService;
    private readonly IProductService _productService;
    private readonly IImageService _imageService;
    private readonly IReviewService _reviewService;

    public ScooterController(
        IScooterService scooterService,
        IProductService productService,
        IImageService imageService,
        IReviewService reviewService)
    {
        _scooterService = scooterService;
        _productService = productService;
        _imageService = imageService;
        _reviewService = reviewService;
    }

    [HttpGet]
    [Authorize(Policy = "CompanyOnly")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Policy = "CompanyOnly")]
    public async Task<IActionResult> Create(ScooterCreateViewModel scooterModel)
    {
        if (!ModelState.IsValid)
        {
            return View(scooterModel);
        }

        if (scooterModel.Product.ImageFile == null || scooterModel.Product.ImageFile.Length == 0)
        {
            ModelState.AddModelError(nameof(scooterModel.Product.ImageFile), ImageRequiredErrorMessage);
            return View(scooterModel);
        }

        var imageType = scooterModel.Product.ImageFile.ContentType;

        if (imageType != "image/jpg" && imageType != "image/jpeg" && imageType != "image/png")
        {
            ModelState.AddModelError(nameof(scooterModel.Product.ImageFile), ImageFormatErrorMessage);
            return View(scooterModel);
        }

        try
        {
            var companyId = User.FindFirst("companyId")!.Value;

            var imageId = await _imageService.CreateAsync(scooterModel.Product.ImageFile);

            var productId = await _productService.CreateAsync(scooterModel.Product, companyId, imageId, ProductType.Scooter);

            var scooterId = await _scooterService.CreateAsync(scooterModel, productId);

            TempData[SuccessMessage] = ScooterCreateSuccessMessage;

            return RedirectToAction("Index", "Home");

            //return RedirectToAction("Details", "Scooter", new { id = scooterId });
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Details(string id)
    {
        var scooterModel = await _scooterService.GetScooterDetailsByIdAsync(id);

        if (scooterModel == null)
        {
            TempData[ErrorMessage] = ScooterNotFoundErrorMessage;
            return RedirectToAction("Index", "Home");
        }

        try
        {
            scooterModel.Product = await _productService.GetProductDetailsByIdAsync(scooterModel.ProductId);

            scooterModel.Product.Image = await _imageService.GetImageByIdAsync(scooterModel.Product.ImageId);

            scooterModel.Product.Reviews = await _reviewService.GetReviewsByProductIdAsync(scooterModel.ProductId);

            return View(scooterModel);
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    [HttpGet]
    [Authorize(Policy = "CompanyOnly")]
    public async Task<IActionResult> Edit(string id)
    {
        bool isScooterExisting = await _scooterService.IsScooterExistingAsync(id);

        if (!isScooterExisting)
        {
            TempData[ErrorMessage] = ScooterNotFoundErrorMessage;
            return RedirectToAction("Index", "Home");
        }

        bool isUserAuthorized = await _scooterService.IsUserAuthorizedToEditAsync(id, User.FindFirst("companyId")!.Value);

        if (!isUserAuthorized)
        {
            TempData[ErrorMessage] = UnauthorizedErrorMessage;
            return RedirectToAction("Index", "Home");
        }

        try
        {
            var scooterModel = await _scooterService.GetScooterEditViewModelByIdAsync(id);

            scooterModel.Product = await _productService.GetProductEditViewModelByIdAsync(scooterModel.ProductId);

            scooterModel.Product.CurrentImage = await _imageService.GetImageByIdAsync(scooterModel.Product.ImageId);

            return View(scooterModel);
        }
        catch (Exception)
        {
            return GeneralError();
        }
    }

    private IActionResult GeneralError()
    {
        this.TempData[ErrorMessage] = UnexpectedErrorMessage;

        return RedirectToAction("Index", "Home");
    }
}
