﻿using Beef.Mapper.Converters;
using System;

namespace Beef.Mapper
{
    /// <summary>
    /// Represents an attribute for defining property characteristics for auto-mapping.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MapperPropertyAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the property name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicates whether the property forms part of the unique (primary) key. 
        /// </summary>
        public bool IsUniqueKey { get; set; }

        /// <summary>
        /// Indicates whether the property value is auto-generated on create (defaults to <c>true</c>); used where <see cref="IsUniqueKey"/> is <c>true</c>. 
        /// </summary>
        public bool IsUniqueKeyAutoGeneratedOnCreate { get; protected set; } = true;

        /// <summary>
        /// Gets or set the <see cref="IPropertyMapperConverter"/> <see cref="Type"/>.
        /// </summary>
        public Type ConverterType { get; set; }

        /// <summary>
        /// Gets or set the <see cref="IEntityMapperBase"/> <see cref="Type"/> for the complex property type.
        /// </summary>
        public Type MapperType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Mapper.OperationTypes"/> selection to enable inclusion or exclusion of property (default to <see cref="OperationTypes.Any"/>).
        /// </summary>
        public OperationTypes OperationTypes { get; set; } = OperationTypes.Any;
    }
}
