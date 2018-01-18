//-----------------------------------------------------------------------
// <copyright file="CustomerType.cs" company="Genesys Source">
//      Copyright (c) 2017 Genesys Source. All rights reserved.
//      Licensed to the Apache Software Foundation (ASF) under one or more 
//      contributor license agreements.  See the NOTICE file distributed with 
//      this work for additional information regarding copyright ownership.
//      The ASF licenses this file to You under the Apache License, Version 2.0 
//      (the 'License'); you may not use this file except in compliance with 
//      the License.  You may obtain a copy of the License at 
//       
//        http://www.apache.org/licenses/LICENSE-2.0 
//       
//       Unless required by applicable law or agreed to in writing, software  
//       distributed under the License is distributed on an 'AS IS' BASIS, 
//       WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  
//       See the License for the specific language governing permissions and  
//       limitations under the License. 
// </copyright>
//-----------------------------------------------------------------------
using Framework.Entity;
using Genesys.Extensions;
using Genesys.Framework.Data;
using Genesys.Framework.Entity;
using System;
using System.Linq;

namespace Framework.DataAccess
{
    /// <summary>
    /// EntityCustomer
    /// </summary>
    [CLSCompliant(true), ConnectionStringName("DefaultConnection"), DatabaseSchemaName("EntityCode")]
    public partial class CustomerType : EntityInfo<CustomerType>, ICustomerType
    {
        /// <summary>
        /// CustomerTypeID enumeration for static values
        /// </summary>
        public struct Types
        {
            public static Guid None { get; set; } = TypeExtension.DefaultGuid;
            public static Guid Standard { get; set; } = new Guid("BF3797EE-06A5-47F2-9016-369BEB21D944");
            public static Guid Premium { get; set; } = new Guid("36B08B23-0C1D-4488-B557-69665FD666E1");
            public static Guid Lifetime { get; set; } = new Guid("51A84CE1-4846-4A71-971A-CB610EEB4848");
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CustomerType()
            : base()
        {
        }

        /// <summary>
        /// Gets all records that exactly equal passed name
        /// </summary>
        /// <param name="name">Value to search CustomerTypeName field </param>
        /// <returns>All records matching the passed name</returns>
        public static IQueryable<CustomerType> GetByName(string name)
        {
            var reader = DatabaseReader<CustomerType>.Construct();
            return reader.GetAll().Where(x => x.Name == name);
        }
    }
}
