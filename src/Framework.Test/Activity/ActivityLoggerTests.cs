//-----------------------------------------------------------------------
// <copyright file="ActivityLoggerTests.cs" company="Genesys Source">
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Genesys.Framework.Activity;
using System.Linq;
using Genesys.Extensions;
using Framework.Test.Data;

namespace Framework.Test
{
    /// <summary>
    /// Tests code first ActivityLogger object saving activity to the database 
    /// </summary>
    [TestClass()]
    public partial class ActivityLoggerTests
    {
        /// <summary>
        /// Tests code first ActivityLogger object saving activity to the database
        /// </summary>
        [TestMethod()]
        public void Activity_ActivityLogger()
        {
            var preSaveCount = TypeExtension.DefaultInteger;
            var postSaveCount = TypeExtension.DefaultInteger;

            Tables.DropMigrationHistory();
            preSaveCount = ActivityLogger.GetAll("DefaultConnection", "Activity").Count();
            ActivityLogger log1 = new ActivityLogger("DefaultConnection", "Activity");
            log1.Save();
            postSaveCount = ActivityLogger.GetAll("DefaultConnection", "Activity").Count();
            Assert.IsTrue(log1.ActivityContextID != TypeExtension.DefaultInteger);
            Assert.IsTrue(postSaveCount == preSaveCount + 1);

            // Your custom schema
            ActivityLogger log2 = new ActivityLogger("DefaultConnection", "MySchema");
            preSaveCount = ActivityLogger.GetAll("DefaultConnection", "Activity").Count();
            log2.Save();
            postSaveCount = ActivityLogger.GetAll("DefaultConnection", "Activity").Count();
            Assert.IsTrue(log2.ActivityContextID != TypeExtension.DefaultInteger);
            Assert.IsTrue(postSaveCount == preSaveCount + 1);
        }
    }
}