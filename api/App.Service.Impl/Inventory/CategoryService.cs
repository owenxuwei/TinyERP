﻿using System.Collections.Generic;
using App.Service.Inventory;
using App.Common.Validation;
using App.Repository.Inventory;
using App.Common.DI;
using App.Common;
using App.Entity.Inventory;
using System;

namespace App.Service.Impl.Inventory
{
    public class CategoryService : ICategoryService
    {
        public void CreateIfNotExist(List<CreateCategoryRequest> createCategoriesRequest)
        {
            using (App.Common.Data.IUnitOfWork uow = new App.Common.Data.UnitOfWork(new App.Context.AppDbContext(IOMode.Write)))
            {
                ICategoryRepository categoryRepository = IoC.Container.Resolve<ICategoryRepository>(uow);
                foreach (CreateCategoryRequest createCategoryRequest in createCategoriesRequest)
                {
                    ValidateCreateCategoryRequest(createCategoryRequest);
                    if (categoryRepository.GetById(createCategoryRequest.Id.ToString()) != null)
                    {
                        continue;
                    }
                    Category category = new Category(createCategoryRequest.Name, createCategoryRequest.Description);
                    categoryRepository.Add(category);
                }
                uow.Commit();
            }
        }

        private void ValidateCreateCategoryRequest(CreateCategoryRequest createCategoryRequest)
        {
            ICategoryRepository categoryRepository = IoC.Container.Resolve<ICategoryRepository>();
            if (string.IsNullOrEmpty(createCategoryRequest.Name))
            {
                throw new ValidationException("inventory.addCategory.validation.nameRequire");
            }

            if (categoryRepository.GetByName(createCategoryRequest.Name) != null)
            {
                throw new ValidationException("inventory.addCategory.validation.nameAlreadyExist");
            }
        }

        public IList<CategoryListItem> GetCategories()
        {
            ICategoryRepository categoryRepository = IoC.Container.Resolve<ICategoryRepository>();
            return categoryRepository.GetItems<CategoryListItem>();
        }
    }
}