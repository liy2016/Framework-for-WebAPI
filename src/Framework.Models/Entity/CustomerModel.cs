//-----------------------------------------------------------------------
// <copyright file="CustomerModel.cs" company="Genesys Source">
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
using Genesys.Extensions;
using Genesys.Framework.Entity;
using System.Collections.Generic;

namespace Framework.Entity
{
    /// <summary>
    /// Customer screen model for binding and transport
    /// </summary>
    /// <remarks></remarks>
    [CLSCompliant(true)]
    public class CustomerModel : ModelEntity<CustomerModel>, ICustomer, IFormattable
    {
        /// <summary>
        /// FirstName of customers
        /// </summary>
        public string FirstName { get; set; } = TypeExtension.DefaultString;

        /// <summary>
        /// MiddleName of customer
        /// </summary>
        public string MiddleName { get; set; } = TypeExtension.DefaultString;

        /// <summary>
        /// LastName of customer
        /// </summary>
        public string LastName { get; set; } = TypeExtension.DefaultString;

        /// <summary>
        /// BirthDate of customer
        /// </summary>
        public DateTime BirthDate { get; set; } = TypeExtension.DefaultDate;

        /// <summary>
        /// BirthDate of customer
        /// </summary>
        public int GenderID { get; set; } = Genders.NotSet.Key;

        /// <summary>
        /// Type of customer
        /// </summary>
        public Guid CustomerTypeKey { get; set; } = CustomerTypeModel.Types.None;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks></remarks>
        public CustomerModel()
                : base()
        {
            this.CustomerTypeKey = CustomerTypeModel.Types.Standard;
        }

        /// <summary>
        /// Supports fml (First Middle Last), lfm (Last, First Middle)
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider = null)
        {
            if (formatProvider != null)
            {
                ICustomFormatter fmt = formatProvider.GetFormat(this.GetType()) as ICustomFormatter;
                if (fmt != null) { return fmt.Format(format, this, formatProvider); }
            }
            switch (format)
            {
                case "lfm": return String.Format("{0}, {1} {2}", this.LastName, this.FirstName, this.MiddleName);
                case "lfMI": return String.Format("{0}, {1} {2}.", this.LastName, this.FirstName, this.MiddleName.SubstringSafe(0, 1));
                case "fMIl": return String.Format("{0} {1}. {2}", this.FirstName, this.MiddleName.SubstringSafe(0, 1), this.LastName);
                case "fl": return String.Format("{0} {1}", this.FirstName, this.LastName);
                case "fml":
                case "G":
                default: return String.Format("{0} {1} {2}", this.FirstName, this.MiddleName, this.LastName);
            }
        }

        /// <summary>
        /// ISO 5218 Standard for Gender values
        /// </summary>
        public struct Genders
        {
            /// <summary>
            /// Default. Not set
            /// </summary>
            public static KeyValuePair<int, string> NotSet { get; } = new KeyValuePair<int, string>(-1, "Not Set");

            /// <summary>
            /// Unknown gender
            /// </summary>
            public static KeyValuePair<int, string> NotKnown { get; } = new KeyValuePair<int, string>(0, "Not Known");

            /// <summary>
            /// Male gender
            /// </summary>
            public static KeyValuePair<int, string> Male { get; } = new KeyValuePair<int, string>(1, "Male");

            /// <summary>
            /// Femal Gender
            /// </summary>
            public static KeyValuePair<int, string> Female { get; } = new KeyValuePair<int, string>(2, "Female");

            /// <summary>
            /// Not applicable or do not want to specify
            /// </summary>
            public static KeyValuePair<int, string> NotApplicable { get; } = new KeyValuePair<int, string>(9, "Not Applicable");
        }

        /// <summary>
        /// List of Genders, bindable to int ID and string Name
        /// </summary>
        public List<KeyValuePair<int, string>> GenderSelections()
        {
            return new List<KeyValuePair<int, string>>() { Genders.NotSet, Genders.Male, Genders.Female };
        }
    }
}
