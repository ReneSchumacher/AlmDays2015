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
    /// Defines the context in which a custom background job can be run.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
    public sealed class JobContextAttribute : Attribute
    {
        readonly JobContext context;

        /// <summary>
        /// Creates a new instance of the JobContextAttribute.
        /// </summary>
        /// <param name="context">The context in which the attributed job can be run.</param>
        public JobContextAttribute(JobContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets the context in which the attributed job can be run.
        /// </summary>
        public JobContext Context
        {
            get { return context; }
        }
    }

    /// <summary>
    /// A context that a background job can be run in.
    /// </summary>
    public enum JobContext
    {
        /// <summary>
        /// The job can be run in the Team Foundation Server context and the Team Project Collection context.
        /// </summary>
        Any,
        /// <summary>
        /// The job can only be run in the Team Foundation Server context.
        /// </summary>
        Server,
        /// <summary>
        /// The job can only be run in the Team Project Collection context.
        /// </summary>
        Collection
    }
}
