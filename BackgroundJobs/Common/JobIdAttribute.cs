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
    /// Defines a unique ID (GUID) for a custom background job.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class JobIdAttribute : Attribute
    {
        readonly string jobId;

        /// <summary>
        /// Creates a new instance of the JobIdAttribute.
        /// </summary>
        /// <param name="jobId">The unique ID for the job.</param>
        public JobIdAttribute(string jobId)
        {
            this.jobId = jobId;
        }
        /// <summary>
        /// Gets the unique ID for the job.
        /// </summary>
        public string JobId
        {
            get { return jobId; }
        }
    }
}
