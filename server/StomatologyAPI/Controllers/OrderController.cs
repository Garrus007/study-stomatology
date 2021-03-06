﻿using StomatologyAPI.Controllers.Abstract;
using StomatologyAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using StomatologyAPI.Abstract;
using Microsoft.AspNet.Identity;
using StomatologyAPI.Infrastructure;
using System.Data.Entity;
using StomatologyAPI.Models.BindingModels;

namespace StomatologyAPI.Controllers
{
    /// <summary>
    /// Контроллер заказов для 
    /// </summary>
    [RoutePrefix("api/Order")]
    public class OrderController : AbstractAPIController<Order>
    {
        IRepository<Doctor> doctor_repository;
        IRepository<DentalTechnican> technican_repository;
        IRepository<ClinicInfo> clinic_repository;
		private IRepository<ToothWork> tooth_repository;

		public OrderController(IUnitOfWork uof) : base(uof)
        {
            doctor_repository = uof.GetRepository<Doctor>();
            clinic_repository = uof.GetRepository<ClinicInfo>();
            technican_repository = uof.GetRepository<DentalTechnican>();
			tooth_repository = uof.GetRepository<ToothWork>();
        }


		//Возвращает доктора текущего пользователя
		Doctor get_current_doctor()
		{
			var user_id = System.Web.HttpContext.Current.User.Identity.GetUserId<int>();
			return doctor_repository.Entities.FirstOrDefault(x => x.ApplicationUserId == user_id);
		}

		//Возвращает все невыполненные заказы
		[Authorize(Roles ="admin,dental_technican")]
        public override IEnumerable<Order> Get()
        {
            var orders = m_repository.Entities.Where(x => !x.IsFinished).ToList();
            return orders;
        }




		[Authorize(Roles = "admin,doctor,dental_technican")]
        public override Order Get(int id)
        {
			
            var order =  m_repository.Entities.Include("Teeth.Procedure").Include(x=>x.ClinicInfo).FirstOrDefault(x => x.Id == id);
			if (order == null)
				return null;

			order.Cost =  order.Teeth.Select(x => x.Procedure).Aggregate((decimal)0, (a, x) => a + x.Cost);
			return order;
		}

        [Authorize(Roles ="doctor")]
        public override HttpResponseMessage Put([FromBody] Order value)
        {
            value.IsFinished = false;
            var user_id = System.Web.HttpContext.Current.User.Identity.GetUserId<int>();
			value.Doctor = get_current_doctor();// doctor_repository.Entities.FirstOrDefault(x=>x.ApplicationUserId== user_id);
            value.Date = DateTime.Now;
            value.ClinicInfo = clinic_repository.Entities.First();
            return base.Put(value);
        }

        [Authorize(Roles ="doctor")]
        public override HttpResponseMessage Post([FromBody] Order value)
        {
			try
			{
				if (value.IsClosed) throw new EntityIsClosedException();
				//value.IsFinished = false;

				var doctor = get_current_doctor();
				if (doctor == null) throw new EntityNotFoundException();
				value.Doctor = doctor;


				return base.Post(value);
			}
			catch (EntityIsClosedException exp)
			{
				return ResponseCreator.GenerateResponse(HttpStatusCode.Forbidden, exp);
			}
		}


        [Authorize(Roles = "admin")]
        public override HttpResponseMessage Delete(int id)
        {
            return base.Delete(id);
        }

        [Authorize(Roles = "doctor")]
        //Добавляет процедуру в посещение
        [Route("AddTooth")]
		[HttpPost]
        public HttpResponseMessage AddTooth(int orderId, int toothNo, int procedureId)
        {
            try
            {
                var order = m_repository.GetById(orderId);
                if (order == null) throw new EntityNotFoundException();
				if (order.IsClosed) throw new EntityIsClosedException();

                order.Teeth.Add(new ToothWork() { ToothNo = toothNo, ProcedureId = procedureId });
                m_repository.Update(order);

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (EntityNotFoundException exp)
            {
                return ResponseCreator.GenerateResponse(HttpStatusCode.NotFound, exp);
            }
			catch (EntityIsClosedException exp)
			{
				return ResponseCreator.GenerateResponse(HttpStatusCode.Forbidden, exp);
			}
		}

        [Authorize(Roles = "doctor")]
        [Route("RemoveTooth/{id}")]
        [HttpDelete]
        //Удаляет процедуру из посещения
        public HttpResponseMessage RemoveTooth(int id)
        {
            try
            {
                /*var order = m_repository.Entities.Include("Teeth.Procedure").Include(x=>x.ClinicInfo).FirstOrDefault(x => x.Id == orderId);
                if (order == null) throw new EntityNotFoundException();
				if (order.IsClosed)throw new EntityIsClosedException();

                var tooth = order.Teeth.FirstOrDefault(x => x.ToothNo == toothNo);
				//order.Teeth.Remove(tooth);
				//m_repository.Update(order);*/
				tooth_repository.Delete(id);

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (EntityNotFoundException exp)
            {
                return ResponseCreator.GenerateResponse(HttpStatusCode.NotFound, exp);
            }
			catch (EntityIsClosedException exp)
			{
				return ResponseCreator.GenerateResponse(HttpStatusCode.Forbidden, exp);
			}
        }

		[Authorize(Roles="doctor")]
		[HttpGet]
		[Route("Close/{orderId}")]

		public HttpResponseMessage Close(int orderId)
		{
			try
			{
				var order = m_repository.GetById(orderId);
				if (order == null)
					throw new EntityNotFoundException();

				order.IsClosed = true;
				m_repository.Update(order);

				return new HttpResponseMessage(HttpStatusCode.OK);
			}
			catch (EntityNotFoundException exp)
			{
				return ResponseCreator.GenerateResponse(HttpStatusCode.NotFound, exp);
			}
		}

        /// <summary>
        /// Завершает заказ
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [Authorize(Roles ="dental_technican")]
        [Route("Finish/{orderId}")]
        [HttpGet]
        public HttpResponseMessage Finish(int orderId)
        {
            try
            {
                var tech_id = System.Web.HttpContext.Current.User.Identity.GetUserId<int>();
                var tech = technican_repository.Entities.First(x => x.ApplicationUserId == tech_id);

                var order = m_repository.GetById(orderId);
                if (order == null) throw new EntityNotFoundException();

                order.IsFinished = true;
                order.DentalTechnican = tech;
				order.FinishDate = DateTime.Now;

                m_repository.Update(order);

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (EntityNotFoundException exp)
            {
                return ResponseCreator.GenerateResponse(HttpStatusCode.NotFound, exp);
            }
            catch(Exception exp)
            {
                return ResponseCreator.GenerateResponse(HttpStatusCode.InternalServerError, exp);
            }
        }
    }
}
