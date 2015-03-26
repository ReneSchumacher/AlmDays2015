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
    /// Defines an optional assembly manifest resource that contains registry information
    /// for a custom job's Team Foundation registry settings.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class RegistryInfoResourceAttribute : Attribute
    {
        readonly string registryInfoResource;

        /// <summary>
        /// Creates a new instance of the RegistryInfoResourceAttribute.
        /// </summary>
        /// <param name="registryInfoResource">
        /// The fully qualified assembly manifest resource that contains registry information
        /// for a custom job's Team Foundation registry settings.
        /// </param>
        public RegistryInfoResourceAttribute(string registryInfoResource)
        {
            this.registryInfoResource = registryInfoResource;
        }

        /// <summary>
        /// Gets the fully qualified assembly manifest resource that contains registry information
        /// for a custom job's Team Foundation registry settings.
        /// </summary>
        public string RegistryInfoResource
        {
            get { return registryInfoResource; }
        }
    }
}
