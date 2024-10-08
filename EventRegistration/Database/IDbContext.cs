﻿namespace EventRegistration.Database
{
    /// <summary> Интерфейс контекста БД </summary>
    public interface IDbContext
    {
        /// <summary> Получение  всех сущностей из одной таблицы </summary>
        /// <typeparam name="T"> Тип сущностей </typeparam>
        /// <returns> IQueryable, через который можно уточнить запрос </returns>
        IQueryable<T> GetEntities<T>() where T : class;

        /// <summary> Получение одной сущности по id </summary>
        /// <param name="id"> Идентификатор</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <typeparam name="T"> Тип сущности</typeparam>
        /// <returns> Сущность, либо null, если такого идентификатора нет в базе </returns>
        Task<T?> GetByIdAsync<T>(Guid id, CancellationToken cancellationToken) where T : class;

        /// <summary> Обновление сущности </summary>
        /// <param name="entity"> Сущность </param>
        void Update<T>(T entity) where T : class;

        /// <summary> Удаление сущности </summary>
        /// <param name="entity"> Сущность </param>
        void Remove<T>(T entity) where T : class;

        /// <summary> Добавление новой сущности в контекст </summary>
        /// <typeparam name="T"> Тип сущности </typeparam>
        /// <param name="entity"> Сущность </param>
        void AddEntity<T>(T entity) where T : class;

        /// <summary> Сохранить изменения </summary>
        Task SaveAsync(CancellationToken cancellationToken);
    }
}
