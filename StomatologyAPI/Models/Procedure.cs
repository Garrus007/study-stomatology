﻿using StomatologyAPI.Models.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StomatologyAPI.Models
{
    /// <summary>
	/// Процедура содержиться в категории.
	/// Описывает информацию о процедуре:
	/// - Название
	/// - Описание
	/// - Текстовое описание
	/// </summary>
    public class Procedure:AbstractModel
    {
        /// <summary>
        /// Название процедуры
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Описание процедуры
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// Связь с подкатегорией
        /// </summary>
        [Required]
        public int SubcategoryId { get; set; }

        /// <summary>
		/// Стоимость процедуры
		/// </summary>
		[Required]
        public int Cost { get; set; }

        /////////////////////////////////// КОНСТРУКТОРЫ //////////////////////////////////////////
        public Procedure() { }
        public Procedure(string name, string description = null)
        {
            Name = name;
            Description = description;
        }
    }
}