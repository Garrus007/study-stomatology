﻿using StomatologyAPI.Models.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StomatologyAPI.Models
{
    /// <summary>
    /// Зубной техник
	/// Он выполняет наряд-заказы
    /// </summary>
    public class DentalTechnican:AbstractModel
    {
        /// <summary>
		/// Наряд-зкаказы
		/// </summary>
		public ICollection<Order> Orders { get; set; }

        /// <summary>
        /// Связь с пользователями
        /// </summary>
        [Required]
        public int ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        /// <summary>
        /// Информация о человеке
        /// </summary>
        public int PersonInfoId { get; set; }
        public virtual PersonInfo PersonInfo { get; set; }
    }
}