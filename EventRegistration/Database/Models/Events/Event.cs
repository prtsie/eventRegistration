﻿using System.ComponentModel.DataAnnotations;
using EventRegistration.Database.Models.Users;

namespace EventRegistration.Database.Models.Events
{
    /// <summary> Модель мероприятия </summary>
    public class Event
    {
        /// <summary> Уникальный идентификатор события </summary>
        [Required]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary> Название мероприятия </summary>
        [Required]
        [MaxLength(Constraints.MaxEventNameLength)]
        public required string Name { get; set; }

        /// <summary> Дата и время проведения </summary>
        [Required]
        public required DateTime Date { get; set; }

        /// <summary> Организатор </summary>
        /// <remarks> Навигационное свойство </remarks>
        [Required]
        public required User Host { get; set; }

        /// <summary> Имя организатора </summary>
        [Required]
        [MaxLength(Constraints.MaxEventHostNameLength)]
        public required string HostName { get; set; }

        /// <summary> Описание </summary>
        [Required]
        [MaxLength(Constraints.MaxEventDescriptionLength)]
        public required string Description { get; set; }

        /// <summary> Отменено ли мероприятие организатором </summary>
        public bool IsCanceled { get; set; }
    }
}
