//-----------------------------------------------------------------------
// <copyright file="CustomerInfo.cs" company="Genesys Source">
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
using Genesys.Extras.Text.Cleansing;
using Genesys.Framework.Activity;
using Genesys.Framework.Data;
using Genesys.Framework.Entity;
using System;
using System.Linq;

namespace Framework.DataAccess
{
    /// <summary>
    /// Customer DAO Entity
    /// </summary>
    [CLSCompliant(true), ConnectionStringName("DefaultConnection")]
    public partial class CustomerInfo : CrudEntity<CustomerInfo>, ICustomer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CustomerInfo()
            : base()
        {
            CustomerTypeKey = CustomerType.Types.Standard;
        }

        /// <summary>
        /// Gets all records that equal first + last + birth date
        /// Does == style search
        /// </summary>
        /// <param name="firstName">First name of customer</param>
        /// <param name="lastName">Last Name of customer</param>
        /// <param name="birthDate">Birth Date of customer</param>
        /// <returns></returns>
        public static IQueryable<CustomerInfo> GetByNameBirthdayKey(string firstName, string lastName, DateTime birthDate)
        {
            var reader = ReadOnlyDatabase<CustomerInfo>.Construct();
            IQueryable<CustomerInfo> returnValue = reader.GetAll()
                .Where(x => (firstName != TypeExtension.DefaultString && x.FirstName == firstName)
                && (lastName != TypeExtension.DefaultString && x.LastName == lastName)
                && (birthDate != TypeExtension.DefaultDate && x.BirthDate == birthDate));
            return returnValue;
        }

        /// <summary>
        /// Gets all records that contain any of the passed fields.
        /// Does contains/like style search
        /// </summary>
        /// <param name="searchFields">ICustomer with data to search</param>
        /// <returns>All records matching the passed ICustomer</returns>
        public static IQueryable<CustomerInfo> GetByAny(ICustomer searchFields)
        {
            var reader = ReadOnlyDatabase<CustomerInfo>.Construct();
            IQueryable<CustomerInfo> returnValue = reader.GetAll()
                .Where(x => (searchFields.FirstName != TypeExtension.DefaultString && x.FirstName.Contains(searchFields.FirstName))
                || (searchFields.LastName != TypeExtension.DefaultString && x.LastName.Contains(searchFields.LastName))
                || (searchFields.BirthDate != TypeExtension.DefaultDate && x.BirthDate == searchFields.BirthDate)
                || (x.ID == searchFields.ID));
            return returnValue;
        }

        /// <summary>
        /// Save the entity to the database. This method will auto-generate activity tracking.
        /// </summary>
        public CustomerInfo Save()
        {
            // Ensure data does not contain cross site scripting injection HTML/Js/SQL
            FirstName = new HtmlUnsafeCleanser(FirstName).Cleanse();
            MiddleName = new HtmlUnsafeCleanser(MiddleName).Cleanse();
            LastName = new HtmlUnsafeCleanser(LastName).Cleanse();
            return base.Update();
        }

        /// <summary>
        /// Save the entity to the database.
        /// This method requires a valid Activity to track this database commit
        /// </summary>
        /// <param name="activity">Activity tracking this record</param>
        public CustomerInfo Save(IActivityContext activity)
        {
            base.ActivityContextID = activity.ActivityContextID;
            // Ensure data does not contain cross site scripting injection HTML/Js/SQL
            FirstName = new HtmlUnsafeCleanser(FirstName).Cleanse();
            MiddleName = new HtmlUnsafeCleanser(MiddleName).Cleanse();
            LastName = new HtmlUnsafeCleanser(LastName).Cleanse();
            return base.Update();
        }

        /// <summary>
        /// ISO 5218 Standard for Gender values
        /// </summary>
        public enum GenderIDs
        {
            NotSet = -1,
            NotKnown = 0,
            Male = 1,
            Female = 2,
            NotApplicable = 9
        }
    }
}
