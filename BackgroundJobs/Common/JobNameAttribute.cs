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
    /// Defines a user friendly name for a custom background job.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class JobNameAttribute : Attribute
    {
        readonly string jobName;

        /// <summary>
        /// Creates a new instance of the JobNameAttribute.
        /// </summary>
        /// <param name="jobName">The user friendly name for the job.</param>
        public JobNameAttribute(string jobName)
        {
            this.jobName = jobName;
        }

        /// <summary>
        /// Gets the user friendly name for the job.
        /// </summary>
        public string JobName
        {
            get { return jobName; }
        }
    }
}
