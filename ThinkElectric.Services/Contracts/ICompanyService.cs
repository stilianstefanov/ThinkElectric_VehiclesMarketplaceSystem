﻿namespace ThinkElectric.Services.Contracts;

using Data.Models;
using Web.ViewModels.Company;

public interface ICompanyService
{
    Task<string> CreateAsync(CompanyCreateViewModel model, string imageId, string addressId, string userId);

    Task<CompanyCreateViewModel> GetCompanyCreateViewModelByUserIdAsync(string id);

    Task<CompanyDetailsViewModel?> GetCompanyDetailsByIdAsync(string id);

}
