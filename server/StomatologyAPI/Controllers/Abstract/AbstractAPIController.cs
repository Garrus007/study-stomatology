﻿using Newtonsoft.Json.Linq;
using StomatologyAPI.Abstract;
using StomatologyAPI.Infrastructure;
using StomatologyAPI.Models.Abstract;
using StomatologyAPI.Models.BindingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace StomatologyAPI.Controllers.Abstract
{
    /// <summary>
	/// Базовый абстрактный контроллер, которой реализует все базовые REST операции
	/// </summary>
	/// <typeparam name="M">Тип модели</typeparam>
	public abstract class AbstractAPIController<M> : ApiController where M : class, IModel
    {
        protected IRepository<M> m_repository;

        public AbstractAPIController(IUnitOfWork uof)
        {
            m_repository = uof.GetRepository<M>();
        }

        //Вернуть все
        public virtual IEnumerable<M> Get() =>  m_repository.Entities.ToList();


        //Вернуть один
        virtual public M Get(int id)
        {
            try
            {
                return m_repository.GetById(id);
            }
            catch (EntityNotFoundException)
            {
                return null;
            }
            catch(Exception exp)
            {
                return null;
            }

        }


        //Изменить
        virtual public HttpResponseMessage Post([FromBody]M value)
        {
            try
            {
                m_repository.Update(value);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (EntityNotFoundException exp)
            {
                return ResponseCreator.GenerateResponse(HttpStatusCode.NotFound, exp); //new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            catch(Exception exp)
            {
                return ResponseCreator.GenerateResponse(HttpStatusCode.InternalServerError, exp); //new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        //Создать
        virtual public HttpResponseMessage Put([FromBody]M value)
        {
            try
            {
                int id = m_repository.Create(value);
				
				//JObject obj = new JObject();
				//obj["id"] = id;
                return Request.CreateResponse<PutResponse>(HttpStatusCode.OK, new PutResponse(id));
				
            }
            catch (EntityAlreadyExistsException exp)
            {
                return ResponseCreator.GenerateResponse(HttpStatusCode.BadRequest, exp);// new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            catch (Exception exp)
            {
                return ResponseCreator.GenerateResponse(HttpStatusCode.InternalServerError, exp); //new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }


        //Удалить
        virtual public HttpResponseMessage Delete(int id)
        {
            try
            {
                m_repository.Delete(id);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (EntityNotFoundException exp)
            {
                return ResponseCreator.GenerateResponse(HttpStatusCode.NotFound, exp.Message);//new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            catch (Exception exp)
            {
                return ResponseCreator.GenerateResponse(HttpStatusCode.InternalServerError, exp.Message); //new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}