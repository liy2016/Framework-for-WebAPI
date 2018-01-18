//-----------------------------------------------------------------------
// <copyright file="DatabaseReaderTests.cs" company="Genesys Source">
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
using Genesys.Framework.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Framework.Test
{
    [TestClass()]
    public class DatabaseReaderTests
    {
        /// <summary>
        /// Data_DatabaseReader_CountAny
        /// </summary>
        [TestMethod()]
        public void Data_DatabaseReader_CountAny()
        {
            var db = DatabaseReader<CustomerType>.Construct();

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
        /// Data_DatabaseReader_Select
        /// </summary>
        [TestMethod()]
        public void Data_DatabaseReader_GetAll()
        {
            var typeDB = DatabaseReader<CustomerType>.Construct();
            var typeResults = typeDB.GetAll().Take(1);
            Assert.IsTrue(typeResults.Count() > 0);
        }

        /// <summary>
        /// Data_DatabaseReader_GetByID
        /// </summary>
        [TestMethod()]
        public void Data_DatabaseReader_GetByID()
        {
            var custData = DatabaseReader<CustomerType>.Construct();
            var custEntity = new CustomerType();

            // ByID Should return 1 record
            var existingID = custData.GetAllExcludeDefault().FirstOrDefaultSafe().ID;
            var custWhereID = custData.GetAll().Where(x => x.ID == existingID);
            Assert.IsTrue(custWhereID.Count() > 0);
            Assert.IsTrue(custWhereID.Any());

            custEntity = custWhereID.FirstOrDefaultSafe();
            Assert.IsTrue(custEntity.ID == existingID);
            Assert.IsTrue(custEntity.IsNew == false);
        }

        /// <summary>
        /// Data_DatabaseReader_GetByKey
        /// </summary>
        [TestMethod()]
        public void Data_DatabaseReader_GetByKey()
        {
            var custData = DatabaseReader<CustomerType>.Construct();

            // ByKey Should return 1 record
            var existingKey = custData.GetAll().FirstOrDefaultSafe().Key;
            var custWhereKey = custData.GetAll().Where(x => x.Key == existingKey);
            Assert.IsTrue(custWhereKey.Count() > 0);
        }

        /// <summary>
        /// Data_DatabaseReader_Insert
        /// </summary>
        /// <remarks></remarks>
        [TestMethod()]
        public void Data_DatabaseReader_GetWhere()
        {
            // Plain EntityInfo object
            var typeData = DatabaseReader<CustomerType>.Construct();
            var testType = new CustomerType();
            var testId = typeData.GetAllExcludeDefault().FirstOrDefaultSafe().ID;
            testType = typeData.GetAll().Where(x => x.ID == testId).FirstOrDefaultSafe();
            Assert.IsTrue(testType.IsNew == false);
            Assert.IsTrue(testType.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(testType.Key != TypeExtension.DefaultGuid);
        }

        /// <summary>
        /// DatabaseReader context and connection
        /// </summary>
        [TestMethod()]
        public void Data_DatabaseReader_Lists()
        {
            var emptyGuid = TypeExtension.DefaultGuid;

            // List Type
            var typeDB = DatabaseReader<CustomerType>.Construct();
            var typeResults = typeDB.GetAllExcludeDefault();
            Assert.IsTrue(typeResults.Count() > 0);
            Assert.IsTrue(typeResults.Any(x => x.Key == emptyGuid) == false);
            Assert.IsTrue(typeResults.Any(x => x.ID == -1) == false);
        }

        /// <summary>
        /// DatabaseReader context and connection
        /// </summary>
        [TestMethod()]
        public void Data_DatabaseReader_Singles()
        {
            var typeDB = DatabaseReader<CustomerType>.Construct();
            var testItem = new CustomerType();
            var emptyGuid = TypeExtension.DefaultGuid;

            // By ID
            testItem = typeDB.GetByID(typeDB.GetAllExcludeDefault().FirstOrDefaultSafe().ID);
            Assert.IsTrue(testItem.IsNew == false);
            Assert.IsTrue(testItem.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(testItem.Key != TypeExtension.DefaultGuid);

            // By Key
            testItem = typeDB.GetByKey(typeDB.GetAllExcludeDefault().FirstOrDefaultSafe().Key);
            Assert.IsTrue(testItem.IsNew == false);
            Assert.IsTrue(testItem.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(testItem.Key != TypeExtension.DefaultGuid);
        }
    }    
}
