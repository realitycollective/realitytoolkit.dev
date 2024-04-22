using RealityCollective.ServiceFramework.Interfaces;
using System;

namespace RealityToolkit.Samples.SampleProject
{
    public interface ISampleProjectService : IService
    {
        event Action LocomotionRoomCompleted;

        void CompleteLocomotionRoom();
    }
}