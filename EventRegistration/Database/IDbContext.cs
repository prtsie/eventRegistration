namespace EventRegistration.Database
{
    /// <summary> Интерфейс контекста БД </summary>
    public interface IDbContext
    {
        /// <summary> Получение  всех сущностей из одной таблицы </summary>
        /// <typeparam name="T"> Тип сущностей </typeparam>
        /// <returns> IQueryable, через который можно уточнить запрос </returns>
        IQueryable<T> GetEntities<T>();

        /// <summary> Получение одной сущности по id </summary>
        /// <param name="id"> Идентификатор</param>
        /// <typeparam name="T"> Тип сущности</typeparam>
        /// <returns> Сущность, либо null, если такого идентификатора нет в базе </returns>
        T? GetById<T>(Guid id);

        /// <summary> Обновление сущности </summary>
        /// <param name="entity"> Сущность </param>
        void Update(object entity);

        /// <summary> Удаление сущности </summary>
        /// <param name="entity"> Сущность </param>
        void Remove(object entity);

        /// <summary> Сохранить изменения </summary>
        void Save();
    }
}
