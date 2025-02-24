﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SunEngine.Core.Cache.CacheModels;
using SunEngine.Core.DataBase;
using SunEngine.Core.Models;

namespace SunEngine.Core.Cache.Services
{
    /// <summary>
    /// Store categories in cache to fast access for singleton service
    /// </summary>
    public interface ICategoriesCache : ISunMemoryCache
    {
        IReadOnlyDictionary<string, SectionTypeCached> AllSectionTypes { get; }
        CategoryCached GetCategory(int id);
        CategoryCached GetCategory(string name);
        CategoryCached RootCategory { get; }
        IDictionary<string, CategoryCached> GetAllCategoriesIncludeSub(string categoriesList);
    }
    
    /// <summary>
    /// Store categories in cache to fast access for singleton service
    /// </summary>
    public class CategoriesCache : ICategoriesCache
    {
        protected readonly object lockObject = new object();
        protected readonly IDataBaseFactory dataBaseFactory;

        protected IReadOnlyDictionary<string, SectionTypeCached> _allSectionTypes;
        protected IReadOnlyDictionary<string, CategoryCached> _allCategoriesByName;
        protected IReadOnlyDictionary<int, CategoryCached> _allCategoriesById;
        protected CategoryCached _rootCategory;

        #region Getters

        public IReadOnlyDictionary<string, SectionTypeCached> AllSectionTypes
        {
            get
            {
                lock (lockObject)
                    if (_allSectionTypes == null)
                        Initialize();

                return _allSectionTypes;
            }
        }

        protected IReadOnlyDictionary<string, CategoryCached> AllCategoriesByNameByName
        {
            get
            {
                lock (lockObject)
                    if (_allCategoriesByName == null)
                        Initialize();

                return _allCategoriesByName;
            }
        }

        protected IReadOnlyDictionary<int, CategoryCached> AllCategoriesById
        {
            get
            {
                lock (lockObject)
                    if (_allCategoriesById == null)
                        Initialize();

                return _allCategoriesById;
            }
        }

        public CategoryCached RootCategory
        {
            get
            {
                lock (lockObject)
                    if (_rootCategory == null)
                        Initialize();

                return _rootCategory;
            }
        }

        #endregion

        public CategoriesCache(IDataBaseFactory dataBaseFactory)
        {
            this.dataBaseFactory = dataBaseFactory;
        }

        public CategoryCached GetCategory(int id)
        {
            return AllCategoriesById[id];
        }

        public CategoryCached GetCategory(string name)
        {
            return AllCategoriesByNameByName[name];
        }

        public IDictionary<string, CategoryCached> GetAllCategoriesIncludeSub(string categoriesList)
        {
            Dictionary<string, CategoryCached> materialsCategoriesDic = new Dictionary<string, CategoryCached>();

            if (categoriesList == null) return materialsCategoriesDic;

            var categoriesNames = categoriesList.Split(',').Select(x => x.Trim());
            foreach (var name in categoriesNames)
            {
                CategoryCached category = GetCategory(name);
                var allSub = category.AllSubCategories.ToDictionary(x => x.Name, x => x);
                allSub.Add(category.Name, category);

                foreach (var (key, value) in allSub)
                {
                    if (!materialsCategoriesDic.ContainsKey(key))
                        materialsCategoriesDic.Add(key, value);
                }
            }

            return materialsCategoriesDic;
        }
        
        public void Initialize()
        {
            using (var db = dataBaseFactory.CreateDb())
            {
                _allSectionTypes = db.SectionTypes
                    .ToImmutableDictionary(x => x.Name, x => new SectionTypeCached(x));

                var categories = db.Categories.Where(x => !x.IsDeleted).Select(x => new CategoryCached(x))
                    .ToDictionary(x => x.Id);

                PrepareCategories(categories);
            }
        }

        protected void PrepareCategories(Dictionary<int, CategoryCached> categories)
        {
            foreach (var category in categories.Values)
            {
                category.Init1ParentAndSub(categories);
            }

            _rootCategory = categories.Values.FirstOrDefault(x => x.Name == Category.RootName);
            if (_rootCategory == null)
                throw new Exception($"Can not find category '{Category.RootName}' in data base.");


            var categoriesList = _rootCategory.Init2AllSub();
            categoriesList.Insert(0, _rootCategory);

            foreach (var category in categoriesList)
            {
                category.Init3ISectionType(_allSectionTypes);
            }

            _rootCategory.Init4InitSectionsRoots();

            foreach (var category in categoriesList)
            {
                category.Init5SetListsAndBlockEditable();
            }

            _allCategoriesByName =
                categoriesList.ToImmutableDictionary(x => x.NameNormalized, StringComparer.OrdinalIgnoreCase);

            _allCategoriesById = _allCategoriesByName.ToImmutableDictionary(x => x.Value.Id, x => x.Value);
        }

        public void Reset()
        {
            _allCategoriesByName = null;
            _allCategoriesById = null;
            _rootCategory = null;
        }
    }
}
