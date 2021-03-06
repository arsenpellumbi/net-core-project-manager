﻿namespace ProjectManager.Core.SeedWork.Interfaces
{
    /// <summary>
    /// Base contract for map aggregate to dto.
    /// <remarks>
    /// This is a  contract for work with "auto" mappers ( auto-mapper,emit-mapper,value-injector...)
    /// or ad-hoc mappers
    /// </remarks>
    /// </summary>
    public interface ITypeAdapter
    {
        /// <summary>
        /// Adapt a source object to an instance of type <typeparamref name="TTarget"/>
        /// </summary>
        /// <typeparam name="TSource">Type of source item</typeparam>
        /// <typeparam name="TTarget">Type of target item</typeparam>
        /// <param name="source">Instance to adapt</param>
        /// <returns><paramref name="source"/> mapped to <typeparamref name="TTarget"/></returns>
        TTarget Adapt<TSource, TTarget>(TSource source)
            where TTarget : class
            where TSource : class;

        /// <summary>
        /// Adapt a source object to an instance of type <typeparamref name="TTarget"/>
        /// </summary>
        /// <typeparam name="TTarget">Type of target item</typeparam>
        /// <param name="source">Instance to adapt</param>
        /// <returns><paramref name="source"/> mapped to <typeparamref name="TTarget"/></returns>
        TTarget Adapt<TTarget>(object source)
            where TTarget : class;
    }
}