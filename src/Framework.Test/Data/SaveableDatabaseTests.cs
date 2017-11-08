//-----------------------------------------------------------------------
// <copyright file="SaveableDatabaseTests.cs" company="Genesys Source">
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
using Genesys.Extensions;
using Genesys.Extras.Mathematics;
using Genesys.Framework.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Test
{
    [TestClass()]
    public class SaveableDatabaseTests
    {
        List<int> recycleBin = new List<int>();
        List<CustomerInfo> testEntities = new List<CustomerInfo>()
        {
            new CustomerInfo() {FirstName = "John", MiddleName = "Adam", LastName = "Doe", BirthDate = DateTime.Today.AddYears(Arithmetic.Random(2).Negate()) },
            new CustomerInfo() {FirstName = "Jane", MiddleName = "Michelle", LastName = "Smith", BirthDate = DateTime.Today.AddYears(Arithmetic.Random(2).Negate()) },
            new CustomerInfo() {FirstName = "Xi", MiddleName = "", LastName = "Ling", BirthDate = DateTime.Today.AddYears(Arithmetic.Random(2).Negate()) },
            new CustomerInfo() {FirstName = "Juan", MiddleName = "", LastName = "Gomez", BirthDate = DateTime.Today.AddYears(Arithmetic.Random(2).Negate()) },
            new CustomerInfo() {FirstName = "Maki", MiddleName = "", LastName = "Ishii", BirthDate = DateTime.Today.AddYears(Arithmetic.Random(2).Negate()) }
        };

        /// <summary>
        /// Data_ReadOnlyDatabase_CountAny
        /// </summary>
        [TestMethod()]
        public void Data_SaveableDatabase_CountAny()
        {
            var db = SaveableDatabase<CustomerType>.Construct();

            // GetAll() count and any
            var resultsAll = db.GetAll();
            Assert.IsTrue(resultsAll.Count() > 0);
            Assert.IsTrue(resultsAll.Any());

            // GetAll().Take(1) count and any
            var resultsTake = db.GetAll().Take(1);
            Assert.IsTrue(resultsTake.Count() == 1);
            Assert.IsTrue(resultsTake.Any());

            // Get an ID to test
            var id = db.GetAllExcludeDefault().FirstOrDefaultSafe().ID;
            Assert.IsTrue(id != TypeExtension.DefaultInteger);

            // GetAll().Where count and any
            var resultsWhere = db.GetAll().Where(x => x.ID == id);
            Assert.IsTrue(resultsWhere.Count() > 0);
            Assert.IsTrue(resultsWhere.Any());
        }

        /// <summary>s
        /// Data_SaveableDatabase_Select
        /// </summary>
        [TestMethod()]
        public void Data_SaveableDatabase_GetAll()
        {
            var typeDB = SaveableDatabase<CustomerType>.Construct();
            var typeResults = typeDB.GetAll().Take(1);
            Assert.IsTrue(typeResults.Count() > 0);

            this.Data_SaveableDatabase_Insert();
            var custDB = SaveableDatabase<CustomerInfo>.Construct();
            var custResults = custDB.GetAll().Take(1);
            Assert.IsTrue(custResults.Count() > 0);
        }

        /// <summary>
        /// Data_SaveableDatabase_GetByID
        /// </summary>
        [TestMethod()]
        public void Data_SaveableDatabase_GetByID()
        {            
            var custData = SaveableDatabase<CustomerInfo>.Construct();
            var custEntity = new CustomerInfo();
            var randomID = custData.GetAll().FirstOrDefaultSafe().ID;
            var randomID2 = custData.GetAll().OrderByDescending(x => x.ID).FirstOrDefaultSafe().ID;

            // GetByID
            var custGetByID = custData.GetByID(randomID);
            var custFirstName = custGetByID.FirstName;
            Assert.IsTrue(custGetByID.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(custGetByID.Key != TypeExtension.DefaultGuid);

            // By custom where
            var fname = custData.GetAll().Where(y => y.FirstName == custFirstName);
            Assert.IsTrue(fname.Any());
            var fnEntity = fname.FirstOrDefaultSafe();
            Assert.IsTrue(fnEntity.IsNew == false);
            Assert.IsTrue(fnEntity.FirstName != TypeExtension.DefaultString);

            // Where 1 record
            custEntity = custData.GetAll().Take(1).FirstOrDefaultSafe();
            Assert.IsTrue(custEntity.ID == randomID);
            Assert.IsTrue(custEntity.IsNew == false);
        }

        /// <summary>
        /// Data_SaveableDatabase_GetByKey
        /// </summary>
        [TestMethod()]
        public void Data_SaveableDatabase_GetByKey()
        {
            // Should create 1 record
            var custData = SaveableDatabase<CustomerInfo>.Construct();
            var custCount = custData.GetAll().Count();
            Assert.IsTrue(custCount > 0);
            // ByKey Should return 1 record            
            var existingKey = custData.GetAll().FirstOrDefaultSafe().Key;
            var custWhereKey = custData.GetByKey(existingKey);
            Assert.IsTrue(custWhereKey.Key == existingKey);
            Assert.IsTrue(custWhereKey.ID != TypeExtension.DefaultInteger);
        }

        /// <summary>
        /// Data_SaveableDatabase_Insert
        /// </summary>
        /// <remarks></remarks>
        [TestMethod()]
        public void Data_SaveableDatabase_GetWhere()
        {
            // Plain EntityInfo object
            var testData2 = SaveableDatabase<CustomerInfo>.Construct();
            var testEntity2 = new CustomerInfo();
            var testId2 = testData2.GetAllExcludeDefault().FirstOrDefaultSafe().ID;
            testEntity2 = testData2.GetAll().Where(x => x.ID == testId2).FirstOrDefaultSafe();
            Assert.IsTrue(testEntity2.IsNew == false);
            Assert.IsTrue(testEntity2.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(testEntity2.Key != TypeExtension.DefaultGuid);

            // CrudEntity object
            this.Data_SaveableDatabase_Insert();
            var testData = SaveableDatabase<CustomerInfo>.Construct();
            var testEntity = new CustomerInfo();
            var testId = testData.GetAllExcludeDefault().FirstOrDefaultSafe().ID;
            testEntity = testData.GetAll().Where(x => x.ID == testId).FirstOrDefaultSafe();
            Assert.IsTrue(testEntity.IsNew == false);
            Assert.IsTrue(testEntity.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(testEntity.Key != TypeExtension.DefaultGuid);
        }

        /// <summary>
        /// Data_SaveableDatabase_Insert
        /// </summary>
        /// <remarks></remarks>
        [TestMethod()]
        public void Data_SaveableDatabase_Insert()
        {
            var dataStore =  SaveableDatabase<CustomerInfo>.Construct();
            var testEntity = new CustomerInfo();
            var resultEntity = new CustomerInfo();
            var oldID = TypeExtension.DefaultInteger;
            var oldKey = TypeExtension.DefaultGuid;
            var newID = TypeExtension.DefaultInteger;
            var newKey = TypeExtension.DefaultGuid;

            // Create and insert record
            testEntity.Fill(testEntities[Arithmetic.Random(1, 5)]);
            oldID = testEntity.ID;
            oldKey = testEntity.Key;
            Assert.IsTrue(testEntity.IsNew);
            Assert.IsTrue(testEntity.IsNew);
            Assert.IsTrue(testEntity.Key == TypeExtension.DefaultGuid);

            // Do Insert and check passed entity and returned entity
            resultEntity = dataStore.Save(testEntity, true);
            Assert.IsTrue(testEntity.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(testEntity.Key != TypeExtension.DefaultGuid);
            Assert.IsTrue(resultEntity.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(resultEntity.Key != TypeExtension.DefaultGuid);
        
            // Pull from DB and retest
            testEntity = dataStore.GetByID(resultEntity.ID);
            Assert.IsTrue(testEntity.IsNew == false);
            Assert.IsTrue(testEntity.ID != oldID);
            Assert.IsTrue(testEntity.Key != oldKey);
            Assert.IsTrue(testEntity.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(testEntity.Key != TypeExtension.DefaultGuid);

            // Cleanup
            recycleBin.Add(testEntity.ID);
        }

        /// <summary>
        /// Data_SaveableDatabase_Update
        /// </summary>
        /// <remarks></remarks>
        [TestMethod()]
        public void Data_SaveableDatabase_Update()
        {
            var testEntity = new CustomerInfo();
            var saver = SaveableDatabase<CustomerInfo>.Construct();
            var oldFirstName = TypeExtension.DefaultString;
            var newFirstName = DateTime.UtcNow.Ticks.ToString();
            int entityID = TypeExtension.DefaultInteger;
            var entityKey = TypeExtension.DefaultGuid;

            // Create and capture original data
            this.Data_SaveableDatabase_Insert();
            testEntity = saver.GetAll().OrderByDescending(x => x.CreatedDate).FirstOrDefaultSafe();
            oldFirstName = testEntity.FirstName;
            entityID = testEntity.ID;
            entityKey = testEntity.Key;
            testEntity.FirstName = newFirstName;
            Assert.IsTrue(testEntity.IsNew == false);
            Assert.IsTrue(testEntity.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(testEntity.Key != TypeExtension.DefaultGuid);

            // Do Update
            saver.Save(testEntity);

            // Pull from DB and retest
            testEntity = saver.GetByID(entityID);
            Assert.IsTrue(testEntity.IsNew == false);
            Assert.IsTrue(testEntity.ID == entityID);
            Assert.IsTrue(testEntity.Key == entityKey);
            Assert.IsTrue(testEntity.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(testEntity.Key != TypeExtension.DefaultGuid);
        }

        /// <summary>
        /// Data_SaveableDatabase_Delete
        /// </summary>
        /// <remarks></remarks>
        [TestMethod()]
        public void Data_SaveableDatabase_Delete()
        {
            var saver = SaveableDatabase<CustomerInfo>.Construct();
            var testEntity = new CustomerInfo();
            var oldID = TypeExtension.DefaultInteger;
            var oldKey = TypeExtension.DefaultGuid;

            // Insert and baseline test
            this.Data_SaveableDatabase_Insert();
            testEntity = saver.GetAll().OrderByDescending(x => x.CreatedDate).FirstOrDefaultSafe();
            oldID = testEntity.ID;
            oldKey = testEntity.Key;
            Assert.IsTrue(testEntity.IsNew == false);
            Assert.IsTrue(testEntity.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(testEntity.Key != TypeExtension.DefaultGuid);

            // Do delete
            saver.Delete(testEntity);

            // Pull from DB and retest
            testEntity = saver.GetAll().Where(x => x.ID == oldID).FirstOrDefaultSafe();
            Assert.IsTrue(testEntity.IsNew);
            Assert.IsTrue(testEntity.ID != oldID);
            Assert.IsTrue(testEntity.Key != oldKey);
            Assert.IsTrue(testEntity.IsNew);
            Assert.IsTrue(testEntity.Key == TypeExtension.DefaultGuid);

            // Add to recycle bin for cleanup
            recycleBin.Add(testEntity.ID);
        }

        /// <summary>
        /// Data_SaveableDatabase_Insert
        /// </summary>
        /// <remarks></remarks>
        [TestMethod()]
        public void Data_SaveableDatabase_RepeatingQueries()
        {
            var dataStore = SaveableDatabase<CustomerInfo>.Construct();
            var customer = new CustomerInfo();
           
            // Multiple Gets
            var a = dataStore.GetAll().ToList();
            var aCount = a.Count;
            var b = dataStore.GetAll().ToList();
            var bCount = b.Count;
            // datastore.Save
            customer = dataStore.GetByID(a.FirstOrDefaultSafe().ID);
            customer.FirstName = DateTime.UtcNow.Ticks.ToString();
            dataStore.Save(customer);
            // Save check
            var c = dataStore.GetAll().ToList();
            var cCount = c.Count;
            Assert.IsTrue(aCount == bCount && bCount == cCount);
            // customer.save
            customer.Update();
            // Multiple Gets
            var x = dataStore.GetAll().ToList();
            var xCount = x.Count;
            var y = dataStore.GetAll().ToList();
            var yCount = y.Count;
            var z = dataStore.GetAll().ToList();
            var zCount = z.Count;
            Assert.IsTrue(xCount == yCount && yCount == zCount);
        }

        /// <summary>
        /// Cleanup all data
        /// </summary>
        [ClassCleanupAttribute()]
        private void Cleanup()
        {
            var saver = SaveableDatabase<CustomerInfo>.Construct();
            var toDelete = new CustomerInfo();

            foreach (int item in recycleBin)
            {
                toDelete = saver.GetAll().Where(x => x.ID == item).FirstOrDefaultSafe();
                saver.Delete(toDelete);
            }
        }
    }    
}
