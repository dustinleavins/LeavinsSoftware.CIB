// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;

namespace LeavinsSoftware.Collection.Persistence
{
    /// <summary>
    /// Interface for <see cref="Model"/> persistence.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IPersistence<TModel> where TModel : Model
    {
        /// <summary>
        /// Creates a persisted record of a model instance.
        /// </summary>
        /// <remarks>
        /// Implementors need to validate the item before persisting it.
        /// </remarks>
        /// <param name="item"></param>
        /// <returns></returns>
        TModel Create(TModel item);

        /// <summary>
        /// Retrieves a persisted record of a model by its id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TModel Retrieve(long id);

        /// <summary>
        /// Updates the record of an already persisted model.
        /// </summary>
        /// <remarks>
        /// Implementors need to validate the item before persisting it.
        /// </remarks>
        /// <param name="item"></param>
        /// <returns></returns>
        TModel Update(TModel item);

        /// <summary>
        /// Deletes the record associated with the model instance.
        /// </summary>
        /// <param name="item"></param>
        void Delete(TModel item);
    }
}
