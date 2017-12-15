//-----------------------------------------------------------------------
// <copyright file="CustomerCloudTests.cs" company="Genesys Source">
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Genesys.Extensions;
using Framework.Entity;
using Genesys.Extras.Mathematics;
using Genesys.Extras.Net;
using Genesys.Extras.Configuration;
using System.Threading.Tasks;
using Framework.DataAccess;

namespace Framework.Test
{
    /// <summary>
    /// Test Genesys Framework for Web API endpoints
    /// </summary>
    /// <remarks></remarks>
    [TestClass()]
    public class Endpoints_Framework_for_WebApi
    {
        private static readonly object LockObject = new object();
        private static volatile List<int> _recycleBin = null;
        /// <summary>
        /// Singleton for recycle bin
        /// </summary>
        private static List<int> RecycleBin
        {
            get
            {
                if (_recycleBin != null) return _recycleBin;
                lock (LockObject)
                {
                    if (_recycleBin == null)
                    {
                        _recycleBin = new List<int>();
                    }
                }
                return _recycleBin;
            }
        }

        private List<CustomerModel> customerTestData = new List<CustomerModel>()
        {
            new CustomerModel() {FirstName = "John", MiddleName = "Adam", LastName = "Doe", BirthDate = DateTime.Today.AddYears(Arithmetic.Random(2).Negate()) },
            new CustomerModel() {FirstName = "Jane", MiddleName = "Michelle", LastName = "Smith", BirthDate = DateTime.Today.AddYears(Arithmetic.Random(2).Negate()) },
            new CustomerModel() {FirstName = "Xi", MiddleName = "", LastName = "Ling", BirthDate = DateTime.Today.AddYears(Arithmetic.Random(2).Negate()) },
            new CustomerModel() {FirstName = "Juan", MiddleName = "", LastName = "Gomez", BirthDate = DateTime.Today.AddYears(Arithmetic.Random(2).Negate()) },
            new CustomerModel() {FirstName = "Maki", MiddleName = "", LastName = "Ishii", BirthDate = DateTime.Today.AddYears(Arithmetic.Random(2).Negate()) }
        };

        /// <summary>
        /// Get a customer, via HttpGet from Framework.WebServices endpoint
        /// </summary>
        /// <remarks></remarks>
        [TestMethod()]
        public async Task Endpoints_Framework_WebAPI_CustomerGet()
        {
            var urlCustomer = new ConfigurationManagerFull().AppSettingValue("MyWebService").AddLast("/Customer");
            await this.Endpoints_Framework_WebAPI_CustomerPut();
            var idToGet = (Endpoints_Framework_for_WebApi.RecycleBin.Count() > 0 ? Endpoints_Framework_for_WebApi.RecycleBin[0] : TypeExtension.DefaultInteger).ToString();
            var request = new HttpRequestGet<CustomerModel>(urlCustomer.AddLast("/") + idToGet.ToString());

            var responseData = await request.SendAsync();
            Assert.IsTrue(!responseData.IsNew);
        }

        /// <summary>
        /// Create a new customer, via HttpPut to Framework.WebServices endpoint
        /// </summary>
        /// <remarks></remarks>
        [TestMethod()]
        public async Task Endpoints_Framework_WebAPI_CustomerPut()
        {
            var customerToCreate = new CustomerModel();
            var url = new Uri(new ConfigurationManagerFull().AppSettingValue("MyWebService").AddLast("/Customer"));

            customerToCreate.Fill(customerTestData[Arithmetic.Random(1, customerTestData.Count)]);
            var request = new HttpRequestPut<CustomerModel>(url, customerToCreate);
            customerToCreate = await request.SendAsync();
            Assert.IsTrue(!customerToCreate.IsNew);
            Endpoints_Framework_for_WebApi.RecycleBin.Add(customerToCreate.ID);
        }

        /// <summary>
        /// Update a customer, via HttpPost to Framework.WebServices endpoint
        /// </summary>
        /// <remarks></remarks>
        [TestMethod()]
        public async Task Endpoints_Framework_WebAPI_CustomerPost()
        {
            var responseData = new CustomerModel();
            var urlCustomer = new ConfigurationManagerFull().AppSettingValue("MyWebService").AddLast("/Customer");

            await this.Endpoints_Framework_WebAPI_CustomerPut();
            var idToGet = Endpoints_Framework_for_WebApi.RecycleBin.Count() > 0 ? Endpoints_Framework_for_WebApi.RecycleBin[0] : TypeExtension.DefaultInteger;

            var url = new Uri(urlCustomer.AddLast("/") + idToGet.ToStringSafe());
            var requestGet = new HttpRequestGet<CustomerModel>(url);
            responseData = await requestGet.SendAsync();
            Assert.IsTrue(!responseData.IsNew);

            var testKey = Guid.NewGuid().ToString();
            responseData.FirstName = responseData.FirstName.AddLast(testKey);
            var request = new HttpRequestPost<CustomerModel>(urlCustomer.TryParseUri(), responseData);
            responseData = await request.SendAsync();
            Assert.IsTrue(!responseData.IsNew);
            Assert.IsTrue(responseData.FirstName.Contains(testKey));
        }

        /// <summary>
        /// Delete a customer, via HttpDelete to Framework.WebServices endpoint
        /// </summary>
        /// <remarks></remarks>
        [TestMethod()]
        public async Task Endpoints_Framework_WebAPI_CustomerDelete()
        {
            var responseData = new CustomerModel();
            var urlCustomer = new ConfigurationManagerFull().AppSettingValue("MyWebService").AddLast("/Customer");

            await this.Endpoints_Framework_WebAPI_CustomerPut();
            var idToDelete = Endpoints_Framework_for_WebApi.RecycleBin.Count() > 0 ? Endpoints_Framework_for_WebApi.RecycleBin[0] : TypeExtension.DefaultInteger;

            var requestDelete = new HttpRequestDelete(urlCustomer.AddLast("/") + idToDelete.ToString());
            await requestDelete.SendAsync();
            Assert.IsTrue(requestDelete.Response.IsSuccessStatusCode);

            var requestGet = new HttpRequestGet<CustomerModel>(urlCustomer);
            responseData = await requestGet.SendAsync();
            Assert.IsTrue(responseData.IsNew);            
        }

        /// <summary>
        /// Get a customer from the cloud
        /// </summary>
        /// <remarks></remarks>
        [TestMethod()]
        public async Task Endpoints_Framework_WebAPI_CustomerSearchGet()
        {
            var url = new ConfigurationManagerFull().AppSettingValue("MyWebService");
            var request = new HttpRequestGet<CustomerSearchModel>(url + "/CustomerSearch/-1/i/x/");
            var returnValue = await request.SendAsync();
            Assert.IsTrue(request.Response.IsSuccessStatusCode, request.Response.ReasonPhrase);
            Assert.IsTrue(returnValue.Results.Count > 0);
        }

        /// <summary>
        /// Cleanup all data
        /// </summary>
        [ClassCleanupAttribute()]
        public static void Cleanup()
        {
            foreach (int item in Endpoints_Framework_for_WebApi.RecycleBin)
            {
                CustomerInfo.GetByID(item).Delete();
            }
        }
    }
}
