﻿namespace ThinkElectric.Services
{
    using Contracts;
    using Data;
    using Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Web.ViewModels.Company;

    public class CompanyService : ICompanyService
    {
        private readonly ThinkElectricDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public CompanyService(ThinkElectricDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<string> CreateAsync(CompanyCreateViewModel model, string imageId, string addressId, string userId)
        {
            Company company = new Company()
            {
                Name = model.Name,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Website = model.Website,
                Description = model.Description,
                FoundedDate = model.FoundedDate!.Value,
                ImageId = imageId,
                AddressId = Guid.Parse(addressId),
                UserId = Guid.Parse(userId)
            };

            await _dbContext.Companies.AddAsync(company);
            await _dbContext.SaveChangesAsync();

            return company.Id.ToString();
        }

        public async Task<CompanyCreateViewModel> GetCompanyCreateViewModelByUserIdAsync(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);

            CompanyCreateViewModel model = new CompanyCreateViewModel()
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };

            return model;
        }

        public async Task<CompanyDetailsViewModel?> GetCompanyDetailsByIdAsync(string id)
        {
            CompanyDetailsViewModel? model = await _dbContext
                .Companies
                .Where(c => c.Id.ToString() == id)
                .Select(c => new CompanyDetailsViewModel()
                {
                    Id = c.Id.ToString(),
                    Name = c.Name,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    Website = c.Website,
                    Description = c.Description,
                    FoundedDate = c.FoundedDate,
                    ImageId = c.ImageId,
                    AddressId = c.AddressId.ToString(),

                })
                .FirstOrDefaultAsync();
                
                
            return model;
        }

        public async Task<bool> HasAlreadyCreatedCompanyAsync(string id)
        {
            bool hasCompany = await _dbContext.
                Companies.
                AnyAsync(c => c.UserId.ToString() == id);

            return hasCompany;
        }
    }
}
