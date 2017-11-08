//-----------------------------------------------------------------------
// <copyright file="CustomerController.cs" company="Genesys Source">
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

using Framework.DataAccess;
using Framework.Entity;
using Genesys.Extensions;
using Genesys.Extras.Web.Http;
using Genesys.Framework.Data;
using System.Web.Http;

namespace Framework.WebServices
{
    /// <summary>
    /// Accepts HttpGet, HttpPut, HttpPost and HttpDelete operations on a customer
    /// </summary>
    public class CustomerController : WebApiController
    {
        public const string ControllerName = "Customer";
        public const string GetActionText = "Get Customer";
        public const string GetAction = "Get";
        public const string PutAction = "Put";
        public const string PostAction = "Post";
        public const string DeleteAction = "Delete";

        /// <summary>
        /// Retrieves customer by ID
        /// </summary>
        /// <returns>Customer that matches the ID, or initialized CustomerModel for not found condition</returns>
        [HttpGet()]
        public CustomerModel Get(string id)
        {
            var reader = ReadOnlyDatabase<CustomerInfo>.Construct();
            var customer = new CustomerInfo();

            if (id.IsInteger())
                customer = reader.GetByID(id.TryParseInt32());
            else
                customer = reader.GetByKey(id.TryParseGuid());

            return customer.CastOrFill<CustomerModel>();
        }

        /// <summary>
        /// Creates a new customer
        /// </summary>
        /// <returns></returns>
        [HttpPut()]
        public CustomerModel Put(CustomerModel model)
        {
            var customer = new CustomerInfo();

            customer.Fill(model);
            customer.Save();

            return customer.CastOrFill<CustomerModel>();
        }

        /// <summary>
        /// Saves changes to a Customer
        /// </summary>
        /// <param name="model">Full customer model worth of data with user changes</param>
        /// <returns>CustomerModel containing customer data</returns>
        [HttpPost()]
        public CustomerModel Post(CustomerModel model)
        {
            var customer = new CustomerInfo();

            customer.Fill(model);
            customer.Save();

            return customer.CastOrFill<CustomerModel>();
        }

        /// <summary>
        /// Saves changes to a Customer
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete()]
        public CustomerModel Delete(string id)
        {
            var reader = ReadOnlyDatabase<CustomerInfo>.Construct();
            var customer = new CustomerInfo();
            CustomerModel model = new CustomerModel();
            
            if(id.IsInteger())
                customer = reader.GetByID(id.TryParseInt32());
            else
                customer = reader.GetByKey(id.TryParseGuid());
            customer.Delete();
            customer = reader.GetByID(id.TryParseInt32());

            return customer.CastOrFill<CustomerModel>();
        }
    }
}