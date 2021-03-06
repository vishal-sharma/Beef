﻿// Copyright (c) Avanade. Licensed under the MIT License. See https://github.com/Avanade/Beef

using Beef.RefData;
using System;

namespace Beef.Mapper.Converters
{
    /// <summary>
    /// Represents a <see cref="ReferenceDataBase"/> mapper property value converter that enables <see cref="Guid"/>-based <see cref="ReferenceDataBase.Id"/> mapping.
    /// </summary>
    /// <typeparam name="TSrceProperty">The source property <see cref="Type"/>.</typeparam>
    public sealed class ReferenceDataGuidIdConverter<TSrceProperty> : Singleton<ReferenceDataGuidIdConverter<TSrceProperty>>, IPropertyMapperConverter<TSrceProperty, Guid> where TSrceProperty : ReferenceDataBaseGuid
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ReferenceDataGuidIdConverter{TSrceProperty}"/> class.
        /// </summary>
        public ReferenceDataGuidIdConverter()
        {
            var tc = ReferenceDataBase.GetIdTypeCode(typeof(TSrceProperty));
            if (tc != ReferenceDataIdTypeCode.Guid)
                throw new InvalidOperationException($"ReferenceData '{GetType().Name}.Id' has Type of '{tc.ToString()}'; must be Type 'Guid' to use this Converter.");
        }

        /// <summary>
        /// Gets the source value <see cref="Type"/>.
        /// </summary>
        Type IPropertyMapperConverter.SrceType { get; } = typeof(TSrceProperty);

        /// <summary>
        /// Gets the destination value <see cref="Type"/>.
        /// </summary>
        Type IPropertyMapperConverter.DestType { get; } = typeof(Guid);

        /// <summary>
        /// Gets the underlying source <see cref="Type"/> allowing for nullables.
        /// </summary>
        Type IPropertyMapperConverter.SrceUnderlyingType { get; } = Nullable.GetUnderlyingType(typeof(TSrceProperty)) ?? typeof(TSrceProperty);

        /// <summary>
        /// Gets the underlying destination <see cref="Type"/> allowing for nullables.
        /// </summary>
        Type IPropertyMapperConverter.DestUnderlyingType { get; } = Nullable.GetUnderlyingType(typeof(Guid)) ?? typeof(Guid);

        /// <summary>
        /// Converts the source <paramref name="value"/> to the destination equivalent.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>The destination value.</returns>
        public Guid ConvertToDest(TSrceProperty value)
        {
            return (value == null) ? Guid.Empty : value.Id;
        }

        /// <summary>
        /// Converts the destination <paramref name="value"/> to the source equivalent.
        /// </summary>
        /// <param name="value">The destination value.</param>
        /// <returns>The source value.</returns>
        public TSrceProperty ConvertToSrce(Guid value)
        {
            return (TSrceProperty)ReferenceDataManager.Current[typeof(TSrceProperty)].GetById(value);
        }

        /// <summary>
        /// Converts the source <paramref name="value"/> to the destination equivalent.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>The destination value.</returns>
        object IPropertyMapperConverter.ConvertToDest(object value)
        {
            return ConvertToDest((TSrceProperty)value);
        }

        /// <summary>
        /// Converts the destination <paramref name="value"/> to the source equivalent.
        /// </summary>
        /// <param name="value">The destination value.</param>
        /// <returns>The source value.</returns>
        object IPropertyMapperConverter.ConvertToSrce(object value)
        {
            return ConvertToSrce((Guid)value);
        }
    }
}
