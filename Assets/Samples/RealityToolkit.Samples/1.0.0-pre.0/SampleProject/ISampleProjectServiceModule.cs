// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Interfaces;

namespace RealityToolkit.Samples.SampleProject
{
    /// <summary>
    /// This interface is used to filter for <see cref="IServiceModule"/>s that can be registered with
    /// the <see cref="SampleProjectServiceProfile"/>.
    /// </summary>
    public interface ISampleProjectServiceModule : IServiceModule { }
}