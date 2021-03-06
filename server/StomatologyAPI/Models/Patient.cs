﻿using StomatologyAPI.Models.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StomatologyAPI.Models
{
    /// <summary>
    /// Пациент
    /// </summary>
    public class Patient:AbstractModel
    {
        /// <summary>
		/// Номер полиса
		/// </summary>
        public int? MedicalCardNumber { get; set; }

        /// <summary>
		/// Возраст
		/// </summary>
        [Required]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Пол. Я до ужаса нетолерантный.
        /// </summary>
        [Required]
        public bool IsMen { get; set; }

        /// <summary>
        /// Связь с пользователями
        /// </summary>
        public int? ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        /// <summary>
        /// Информация о человеке
        /// </summary>
        public int PersonInfoId { get; set; }
        public virtual PersonInfo PersonInfo { get; set; }
        

        /// <summary>
        /// Посещения
        /// </summary>
        public ICollection<PatientVisit> Visits
        {
            get { return this.visits ?? (this.visits = new List<PatientVisit>()); }
            set { visits = value; }
        }
        private ICollection<PatientVisit> visits;

        /// <summary>
        /// Наряд-заказы
        /// </summary>
        public ICollection<Order> Orders
        {
            get { return this.orders ?? (this.orders = new List<Order>()); }
            set { orders = value; }
        }
        private ICollection<Order> orders;

    }

   
}