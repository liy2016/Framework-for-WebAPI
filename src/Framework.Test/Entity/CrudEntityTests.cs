//-----------------------------------------------------------------------
// <copyright file="CrudEntityTests.cs" company="Genesys Source">
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
    public class CrudEntityTests
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
        /// Entity_CrudEntity
        /// </summary>
        [TestMethod()]
        public void Entity_CrudEntity_Create()
        {
            var newCustomer = new CustomerInfo();
            var resultCustomer = new CustomerInfo();
            var dbCustomer = new CustomerInfo();
            
            // Create should update original object, and pass back a fresh-from-db object
            newCustomer.Fill(testEntities[Arithmetic.Random(1, 5)]);
            resultCustomer = newCustomer.Create();
            Assert.IsTrue(newCustomer.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(newCustomer.Key != TypeExtension.DefaultGuid);
            Assert.IsTrue(resultCustomer.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(resultCustomer.Key != TypeExtension.DefaultGuid);

            // Object in db should match in-memory objects
            dbCustomer = dbCustomer.Read(x => x.ID == resultCustomer.ID).FirstOrDefaultSafe();
            Assert.IsTrue(!dbCustomer.IsNew);
            Assert.IsTrue(dbCustomer.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(dbCustomer.Key != TypeExtension.DefaultGuid);
            Assert.IsTrue(dbCustomer.ID == resultCustomer.ID && resultCustomer.ID == newCustomer.ID);
            Assert.IsTrue(dbCustomer.Key == resultCustomer.Key && resultCustomer.Key == newCustomer.Key);

            CrudEntityTests.RecycleBin.Add(newCustomer.ID);
        }

        /// <summary>
        /// Entity_CrudEntity
        /// </summary>
        [TestMethod()]
        public void Entity_CrudEntity_Read()
        {            
            var dbCustomer = new CustomerInfo();
            var lastID = TypeExtension.DefaultInteger;

            Entity_CrudEntity_Create();
            lastID = CrudEntityTests.RecycleBin.Last();

            dbCustomer = dbCustomer.Read(x => x.ID == lastID).FirstOrDefaultSafe();
            Assert.IsTrue(!dbCustomer.IsNew);
            Assert.IsTrue(dbCustomer.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(dbCustomer.Key != TypeExtension.DefaultGuid);
            Assert.IsTrue(dbCustomer.CreatedDate.Date == DateTime.UtcNow.Date);
        }

        /// <summary>
        /// Entity_CrudEntity
        /// </summary>
        [TestMethod()]
        public void Entity_CrudEntity_Update()
        {
            var resultCustomer = new CustomerInfo();
            var dbCustomer = new CustomerInfo();
            var uniqueValue = Guid.NewGuid().ToString().Replace("-", "");
            var lastID = TypeExtension.DefaultInteger;
            var originalID = TypeExtension.DefaultInteger;
            var originalKey = TypeExtension.DefaultGuid;

            Entity_CrudEntity_Create();
            lastID = CrudEntityTests.RecycleBin.Last();

            dbCustomer = dbCustomer.Read(x => x.ID == lastID).FirstOrDefaultSafe();
            originalID = dbCustomer.ID;
            originalKey = dbCustomer.Key;
            Assert.IsTrue(!dbCustomer.IsNew);
            Assert.IsTrue(dbCustomer.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(dbCustomer.Key != TypeExtension.DefaultGuid);

            dbCustomer.FirstName = uniqueValue;
            resultCustomer = dbCustomer.Update();
            Assert.IsTrue(!resultCustomer.IsNew);
            Assert.IsTrue(resultCustomer.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(resultCustomer.Key != TypeExtension.DefaultGuid);
            Assert.IsTrue(dbCustomer.ID == resultCustomer.ID && resultCustomer.ID == originalID);
            Assert.IsTrue(dbCustomer.Key == resultCustomer.Key && resultCustomer.Key == originalKey);

            dbCustomer = dbCustomer.Read(x => x.ID == originalID).FirstOrDefaultSafe();
            Assert.IsTrue(!dbCustomer.IsNew);
            Assert.IsTrue(dbCustomer.ID == resultCustomer.ID && resultCustomer.ID == originalID);
            Assert.IsTrue(dbCustomer.Key == resultCustomer.Key && resultCustomer.Key == originalKey);
            Assert.IsTrue(dbCustomer.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(dbCustomer.Key != TypeExtension.DefaultGuid);
        }

        /// <summary>
        /// Entity_CrudEntity
        /// </summary>
        [TestMethod()]
        public void Entity_CrudEntity_Delete()
        {            
            var dbCustomer = new CustomerInfo();
            var result = TypeExtension.DefaultBoolean;
            var lastID = TypeExtension.DefaultInteger;
            var originalID = TypeExtension.DefaultInteger;
            var originalKey = TypeExtension.DefaultGuid;

            Entity_CrudEntity_Create();
            lastID = CrudEntityTests.RecycleBin.Last();

            dbCustomer = dbCustomer.Read(x => x.ID == lastID).FirstOrDefaultSafe();
            originalID = dbCustomer.ID;
            originalKey = dbCustomer.Key;
            Assert.IsTrue(!dbCustomer.IsNew);
            Assert.IsTrue(dbCustomer.ID != TypeExtension.DefaultInteger);
            Assert.IsTrue(dbCustomer.Key != TypeExtension.DefaultGuid);
            Assert.IsTrue(dbCustomer.CreatedDate.Date == DateTime.UtcNow.Date);

            result = dbCustomer.Delete();
            Assert.IsTrue(result);

            dbCustomer = dbCustomer.Read(x => x.ID == originalID).FirstOrDefaultSafe();
            Assert.IsTrue(dbCustomer.IsNew);
            Assert.IsTrue(dbCustomer.ID == TypeExtension.DefaultInteger);
            Assert.IsTrue(dbCustomer.Key == TypeExtension.DefaultGuid);
        }

        /// <summary>
        /// Cleanup all data
        /// </summary>
        [ClassCleanupAttribute()]
        public static void Cleanup()
        {
            var db = DatabaseWriter<CustomerInfo>.Construct();
            foreach (int item in CrudEntityTests.RecycleBin)
            {
                db.GetByID(item).Delete();
            }
        }
    }
}
