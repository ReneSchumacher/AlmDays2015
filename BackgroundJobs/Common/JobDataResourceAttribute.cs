//--------------------------------------------------------------------------------
// This file is part of a Microsoft sample.
//
// (c) 2013 Microsoft Corporation. All rights reserved. 
// 
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//--------------------------------------------------------------------------------

using System;

namespace Microsoft.PSfD.TeamFoundation.BackgroundJobs.Common
{
    /// <summary>
    /// Defines an optional assembly manifest resource that contains a sample job data
    /// XML element.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
    public sealed class JobDataResourceAttribute : Attribute
    {
        readonly string jobDataResource;

        /// <summary>
        /// Creates a new instance of the JobDataResourceAttribute.
        /// </summary>
        /// <param name="jobDataResource">
        /// The fully qualified name of the assembly manifest resource that contains a sample job data
        /// XML element.
        /// </param>
        public JobDataResourceAttribute(string jobDataResource)
        {
            this.jobDataResource = jobDataResource;
        }

        /// <summary>
        /// Gets the fully qualified name of the assembly manifest resource that contains a sample job data
        /// XML element.
        /// </summary>
        public string JobDataResource
        {
            get { return jobDataResource; }
        }
    }
}
