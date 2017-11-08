//-----------------------------------------------------------------------
// <copyright file="CustomerViewModelTests.cs" company="Genesys Source">
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
using Genesys.Extras.Configuration;
using Genesys.Extras.Mathematics;
using Genesys.Extras.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Framework.Test
{
    /// <summary>
    /// Test Genesys Framework for Web API endpoints
    /// </summary>
    /// <remarks></remarks>
    [TestClass()]
    public class CustomerViewModelTests
    {
        private List<int> recycleBin = new List<int>();
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
        public async Task ViewModel_CRUD_Read()
        {
            var customer = new CustomerModel();
            var viewModel = new TestViewModel<CustomerModel>("Customer");

            // Create test record
            await this.ViewModel_CRUD_Create();
            var idToTest = recycleBin.Count() > 0 ? recycleBin[0] : TypeExtension.DefaultInteger;
            
            // Verify update success
            customer = await viewModel.ReadAsync(idToTest);
            Assert.IsTrue(customer.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(customer.Key != TypeExtension.DefaultGuid);
            Assert.IsTrue(!customer.IsNew);
        }

        /// <summary>
        /// Create a new customer, via HttpPut to Framework.WebServices endpoint
        /// </summary>
        /// <remarks></remarks>
        [TestMethod()]
        public async Task ViewModel_CRUD_Create()
        {
            var customer = new CustomerModel();
            var url = new Uri(new ConfigurationManagerFull().AppSettingValue("MyWebService").AddLast("/Customer"));

            customer.Fill(customerTestData[Arithmetic.Random(1, customerTestData.Count)]);
            var request = new HttpRequestPut<CustomerModel>(url, customer);
            customer = await request.SendAsync();
            Assert.IsTrue(customer.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(customer.Key != TypeExtension.DefaultGuid);

            recycleBin.Add(customer.ID);
        }

        /// <summary>
        /// Update a customer, via HttpPost to Framework.WebServices endpoint
        /// </summary>
        /// <remarks></remarks>
        [TestMethod()]
        public async Task ViewModel_CRUD_Update()
        {
            var customer = new CustomerModel();
            var viewModel = new TestViewModel<CustomerModel>("Customer");

            // Create test record
            await this.ViewModel_CRUD_Create();
            var idToTest = recycleBin.Count() > 0 ? recycleBin[0] : TypeExtension.DefaultInteger;            
            // Read test record
            customer = await viewModel.ReadAsync(idToTest);            
            // Update test record
            var testKey = Guid.NewGuid().ToString();
            customer.FirstName = customer.FirstName.AddLast(testKey);
            customer = await viewModel.UpdateAsync();
            Assert.IsTrue(customer.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(customer.Key != TypeExtension.DefaultGuid);
            // Verify update success
            customer = await viewModel.ReadAsync(idToTest);
            Assert.IsTrue(customer.FirstName.Contains(testKey));
            Assert.IsTrue(!viewModel.MyModel.IsNew);
            Assert.IsTrue(!customer.IsNew);
        }

        /// <summary>
        /// Delete a customer, via HttpDelete to Framework.WebServices endpoint
        /// </summary>
        /// <remarks></remarks>
        [TestMethod()]
        public async Task ViewModel_CRUD_Delete()
        {
            var customer = new CustomerModel();
            var viewModel = new TestViewModel<CustomerModel>("Customer");
            var success = TypeExtension.DefaultBoolean;

            // Create test record
            await this.ViewModel_CRUD_Create();
            var idToTest = recycleBin.Count() > 0 ? recycleBin[0] : TypeExtension.DefaultInteger;

            // Test
            customer = await viewModel.ReadAsync(idToTest);
            Assert.IsTrue(!customer.IsNew);
            Assert.IsTrue(customer.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(customer.Key != TypeExtension.DefaultGuid);
            success = await viewModel.DeleteAsync();
            Assert.IsTrue(success);
            Assert.IsTrue(viewModel.MyModel.IsNew);
            Assert.IsTrue(viewModel.MyModel.ID == TypeExtension.DefaultInteger);
            Assert.IsTrue(viewModel.MyModel.Key == TypeExtension.DefaultGuid);
            // Verify update success
            customer = await viewModel.ReadAsync(idToTest);
            Assert.IsTrue(viewModel.MyModel.IsNew);
            Assert.IsTrue(customer.IsNew);
            Assert.IsTrue(customer.ID == TypeExtension.DefaultInteger);
            Assert.IsTrue(customer.Key == TypeExtension.DefaultGuid);
        }

        /// <summary>
        /// Cleanup all data
        /// </summary>
        [ClassCleanupAttribute()]
        private void Cleanup()
        {
            foreach (int item in recycleBin)
            {
                CustomerInfo.GetByID(item).Delete();
            }
        }
    }
}
