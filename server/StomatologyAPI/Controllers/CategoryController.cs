﻿using StomatologyAPI.Controllers.Abstract;
using StomatologyAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using StomatologyAPI.Abstract;
using System.Data.Entity;

namespace StomatologyAPI.Controllers
{
    /// <summary>
    /// Контроллер категории
    /// - просмотр всех
	/// - просмотр одной(+ подкатегории + процедуры)
	/// - Редактирование А
	/// - УДаление А
    /// </summary>
    public class CategoryController : AbstractAPIController<Category>
    {
        public CategoryController(IUnitOfWork uof) : base(uof)
        {
        }

        public override Category Get(int id)
        {
            return m_repository.Entities.Include("Subcategories.Procedures").FirstOrDefault(x => x.Id == id);
        }

        [Authorize(Roles = "admin")]
        public override HttpResponseMessage Put([FromBody] Category value)
        {
            return base.Put(value);
        }

        [Authorize(Roles = "admin")]
        public override HttpResponseMessage Post([FromBody] Category value)
        {
            return base.Post(value);
        }
        
        [Authorize(Roles = "admin")]
        public override HttpResponseMessage Delete(int id)
        {
            return base.Delete(id);
        }
    }
}
