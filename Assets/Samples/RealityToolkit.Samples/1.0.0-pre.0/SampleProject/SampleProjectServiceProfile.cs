// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Definitions;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.Samples.SampleProject
{
    /// <summary>
    /// Configuration profile for the <see cref="SampleProjectService"/>.
    /// </summary>
    public class SampleProjectServiceProfile : BaseServiceProfile<ISampleProjectServiceModule>
    {
        /// <summary>
        /// Scenes to load upon application launch.
        /// </summary>
        [field: SerializeField]
        public List<string> ScenesToLoad { get; private set; } = null;
    }
}