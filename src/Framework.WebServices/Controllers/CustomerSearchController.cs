//-----------------------------------------------------------------------
// <copyright file="CustomerSearchController.cs" company="Genesys Source">
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
using System;
using System.Web.Http;
using Genesys.Extensions;
using Genesys.Extras.Web.Http;
using Framework.Entity;
using System.Linq;
using Framework.DataAccess;

namespace Framework.WebServices
{
    /// <summary>
    /// Searches for customer records    
    /// </summary>
    public class CustomerSearchController : WebApiController
    {
        public const string ControllerName = "CustomerSearch";
        public const string GetActionText = "Search";
        public const string GetAction = "Get";
        public const string PostAction = "Post";        
        public const string SearchRoute = "v1/" + CustomerSearchController.ControllerName + "/{id}/{firstName}/{lastName}";

        /// <summary>
        /// Parameterized HttpGet search, refreshing only the results region
        /// Path: /v1/CustomerSearch/{id}/{firstName}/{lastName}/
        ///  Parameters are strings in order to validate, log and handle incorrect values
        /// </summary>
        /// <param name="id">Int32 - ID to include in search results</param>
        /// <param name="firstName">String - Text to search in first name</param>
        /// <param name="lastName">String - Text to search in the last name field</param>
        /// <returns>Partial view of only the search results region</returns>
        [HttpGet(), Route(CustomerSearchController.SearchRoute)]
        public CustomerSearchModel Get(string id = "-1", string firstName = "", string lastName = "")
        {            
            CustomerSearchModel model = new CustomerSearchModel() { ID = id.TryParseInt32(), FirstName = firstName, LastName = lastName };
            IQueryable<CustomerInfo> searchResults;

            searchResults = CustomerInfo.GetByAny(model).Take(25);
            if (searchResults.Any())
            {
                model.Results.FillRange(searchResults);
            }

            return model;
        }

        /// <summary>
        /// Performs a full HttpPost search, accepting search parameters and returning search parameters and results
        /// </summary>
        /// <param name="model">Model of type ICustomer with results list</param>
        /// <returns>JSON of search parameters and any found results</returns>
        [HttpPost()]
        public CustomerSearchModel Post(CustomerSearchModel model)
        {
            IQueryable<CustomerInfo> searchResults;

            searchResults = CustomerInfo.GetByAny(model).Take(25);
            if (searchResults.Any())
            {
                model.Results.FillRange(searchResults.ToList());
            }

            return model;
        }     
    }
}