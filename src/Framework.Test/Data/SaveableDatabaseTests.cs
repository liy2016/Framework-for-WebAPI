//-----------------------------------------------------------------------
// <copyright file="DatabaseWriterTests.cs" company="Genesys Source">
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
using Genesys.Framework.Operation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Test
{
    [TestClass()]
    public class DatabaseWriterTests
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
        List<CustomerInfo> testEntities = new List<CustomerInfo>()
        {
            new CustomerInfo() {FirstName = "John", MiddleName = "Adam", LastName = "Doe", BirthDate = DateTime.Today.AddYears(Arithmetic.Random(2).Negate()) },
            new CustomerInfo() {FirstName = "Jane", MiddleName = "Michelle", LastName = "Smith", BirthDate = DateTime.Today.AddYears(Arithmetic.Random(2).Negate()) },
            new CustomerInfo() {FirstName = "Xi", MiddleName = "", LastName = "Ling", BirthDate = DateTime.Today.AddYears(Arithmetic.Random(2).Negate()) },
            new CustomerInfo() {FirstName = "Juan", MiddleName = "", LastName = "Gomez", BirthDate = DateTime.Today.AddYears(Arithmetic.Random(2).Negate()) },
            new CustomerInfo() {FirstName = "Maki", MiddleName = "", LastName = "Ishii", BirthDate = DateTime.Today.AddYears(Arithmetic.Random(2).Negate()) }
        };

        /// <summary>
        /// Data_DatabaseReader_CountAny
        /// </summary>
        [TestMethod()]
        public void Data_DatabaseWriter_CountAny()
        {
            var db = DatabaseWriter<CustomerType>.Construct();

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
        /// Data_DatabaseWriter_Select
        /// </summary>
        [TestMethod()]
        public void Data_DatabaseWriter_GetAll()
        {
            var typeDB = DatabaseWriter<CustomerType>.Construct();
            var typeResults = typeDB.GetAll().Take(1);
            Assert.IsTrue(typeResults.Count() > 0);

            this.Data_DatabaseWriter_Insert();
            var custDB = DatabaseWriter<CustomerInfo>.Construct();
            var custResults = custDB.GetAll().Take(1);
            Assert.IsTrue(custResults.Count() > 0);
        }

        /// <summary>
        /// Data_DatabaseWriter_GetByID
        /// </summary>
        [TestMethod()]
        public void Data_DatabaseWriter_GetByID()
        {            
            var custData = DatabaseWriter<CustomerInfo>.Construct();
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
        /// Data_DatabaseWriter_GetByKey
        /// </summary>
        [TestMethod()]
        public void Data_DatabaseWriter_GetByKey()
        {
            // Should create 1 record
            var custData = DatabaseWriter<CustomerInfo>.Construct();
            var custCount = custData.GetAll().Count();
            Assert.IsTrue(custCount > 0);
            // ByKey Should return 1 record            
            var existingKey = custData.GetAll().FirstOrDefaultSafe().Key;
            var custWhereKey = custData.GetByKey(existingKey);
            Assert.IsTrue(custWhereKey.Key == existingKey);
            Assert.IsTrue(custWhereKey.ID != TypeExtension.DefaultInteger);
        }

        /// <summary>
        /// Data_DatabaseWriter_Insert
        /// </summary>
        /// <remarks></remarks>
        [TestMethod()]
        public void Data_DatabaseWriter_GetWhere()
        {
            // Plain EntityInfo object
            var testData2 = DatabaseWriter<CustomerInfo>.Construct();
            var testEntity2 = new CustomerInfo();
            var testId2 = testData2.GetAllExcludeDefault().FirstOrDefaultSafe().ID;
            testEntity2 = testData2.GetAll().Where(x => x.ID == testId2).FirstOrDefaultSafe();
            Assert.IsTrue(testEntity2.IsNew == false);
            Assert.IsTrue(testEntity2.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(testEntity2.Key != TypeExtension.DefaultGuid);

            // CrudEntity object
            this.Data_DatabaseWriter_Insert();
            var testData = DatabaseWriter<CustomerInfo>.Construct();
            var testEntity = new CustomerInfo();
            var testId = testData.GetAllExcludeDefault().FirstOrDefaultSafe().ID;
            testEntity = testData.GetAll().Where(x => x.ID == testId).FirstOrDefaultSafe();
            Assert.IsTrue(testEntity.IsNew == false);
            Assert.IsTrue(testEntity.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(testEntity.Key != TypeExtension.DefaultGuid);
        }

        /// <summary>
        /// Data_DatabaseWriter_Insert
        /// </summary>
        /// <remarks></remarks>
        [TestMethod()]
        public void Data_DatabaseWriter_Insert()
        {
            var dataStore =  DatabaseWriter<CustomerInfo>.Construct();
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
            dataStore = DatabaseWriter<CustomerInfo>.Construct(testEntity);
            resultEntity = dataStore.Save(SaveBehaviors.InsertOnly);
            Assert.IsTrue(testEntity.ID != TypeExtension.DefaultInteger);
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
            DatabaseWriterTests.RecycleBin.Add(testEntity.ID);
        }

        /// <summary>
        /// Data_DatabaseWriter_Update
        /// </summary>
        /// <remarks></remarks>
        [TestMethod()]
        public void Data_DatabaseWriter_Update()
        {
            var testEntity = new CustomerInfo();
            var saver = DatabaseWriter<CustomerInfo>.Construct();
            var oldFirstName = TypeExtension.DefaultString;
            var newFirstName = DateTime.UtcNow.Ticks.ToString();
            int entityID = TypeExtension.DefaultInteger;
            var entityKey = TypeExtension.DefaultGuid;

            // Create and capture original data
            this.Data_DatabaseWriter_Insert();
            testEntity = saver.GetAll().OrderByDescending(x => x.CreatedDate).FirstOrDefaultSafe();
            oldFirstName = testEntity.FirstName;
            entityID = testEntity.ID;
            entityKey = testEntity.Key;
            testEntity.FirstName = newFirstName;
            Assert.IsTrue(testEntity.IsNew == false);
            Assert.IsTrue(testEntity.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(testEntity.Key != TypeExtension.DefaultGuid);

            // Do Update
            saver = DatabaseWriter<CustomerInfo>.Construct(testEntity);
            saver.Save();

            // Pull from DB and retest
            testEntity = saver.GetByID(entityID);
            Assert.IsTrue(testEntity.IsNew == false);
            Assert.IsTrue(testEntity.ID == entityID);
            Assert.IsTrue(testEntity.Key == entityKey);
            Assert.IsTrue(testEntity.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(testEntity.Key != TypeExtension.DefaultGuid);
        }

        /// <summary>
        /// Data_DatabaseWriter_Delete
        /// </summary>
        /// <remarks></remarks>
        [TestMethod()]
        public void Data_DatabaseWriter_Delete()
        {
            var dbWriter = DatabaseWriter<CustomerInfo>.Construct();
            var testEntity = new CustomerInfo();
            var oldID = TypeExtension.DefaultInteger;
            var oldKey = TypeExtension.DefaultGuid;

            // Insert and baseline test
            this.Data_DatabaseWriter_Insert();
            testEntity = dbWriter.GetAll().OrderByDescending(x => x.CreatedDate).FirstOrDefaultSafe();
            oldID = testEntity.ID;
            oldKey = testEntity.Key;
            Assert.IsTrue(testEntity.IsNew == false);
            Assert.IsTrue(testEntity.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(testEntity.Key != TypeExtension.DefaultGuid);

            // Do delete
            dbWriter = DatabaseWriter<CustomerInfo>.Construct(testEntity);
            dbWriter.Delete();

            // Pull from DB and retest
            testEntity = dbWriter.GetAll().Where(x => x.ID == oldID).FirstOrDefaultSafe();
            Assert.IsTrue(testEntity.IsNew);
            Assert.IsTrue(testEntity.ID != oldID);
            Assert.IsTrue(testEntity.Key != oldKey);
            Assert.IsTrue(testEntity.IsNew);
            Assert.IsTrue(testEntity.Key == TypeExtension.DefaultGuid);

            // Add to recycle bin for cleanup
            DatabaseWriterTests.RecycleBin.Add(testEntity.ID);
        }

        /// <summary>
        /// Data_DatabaseWriter_Insert
        /// </summary>
        /// <remarks></remarks>
        [TestMethod()]
        public void Data_DatabaseWriter_RepeatingQueries()
        {
            var dbWriter = DatabaseWriter<CustomerInfo>.Construct();
            var customer = new CustomerInfo();
           
            // Multiple Gets
            var a = dbWriter.GetAll().ToList();
            var aCount = a.Count;
            var b = dbWriter.GetAll().ToList();
            var bCount = b.Count;
            // datastore.Save
            customer = dbWriter.GetByID(a.FirstOrDefaultSafe().ID);
            customer.FirstName = DateTime.UtcNow.Ticks.ToString();
            dbWriter = DatabaseWriter<CustomerInfo>.Construct(customer);
            dbWriter.Save();
            // Save check
            var c = dbWriter.GetAll().ToList();
            var cCount = c.Count;
            Assert.IsTrue(aCount == bCount && bCount == cCount);
            // customer.save
            customer.Update();
            // Multiple Gets
            var x = dbWriter.GetAll().ToList();
            var xCount = x.Count;
            var y = dbWriter.GetAll().ToList();
            var yCount = y.Count;
            var z = dbWriter.GetAll().ToList();
            var zCount = z.Count;
            Assert.IsTrue(xCount == yCount && yCount == zCount);
        }

        /// <summary>
        /// Cleanup all data
        /// </summary>
        [ClassCleanupAttribute()]
        public static void Cleanup()
        {
            var dbWriter = DatabaseWriter<CustomerInfo>.Construct();
            var toDelete = new CustomerInfo();

            foreach (int item in DatabaseWriterTests.RecycleBin)
            {
                toDelete = dbWriter.GetAll().Where(x => x.ID == item).FirstOrDefaultSafe();
                dbWriter = DatabaseWriter<CustomerInfo>.Construct(toDelete);
                dbWriter.Delete();
            }
        }
    }    
}
