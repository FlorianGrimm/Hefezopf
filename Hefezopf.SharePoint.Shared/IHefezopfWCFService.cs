// Hefezopf
// MIT License
// Copyright (c) 2016 Florian Grimm

namespace Hefezopf.Service
{
    using System;
    using System.ServiceModel;

    /// <summary>
    /// The Service Contract.
    /// </summary>
    [ServiceContract(Namespace = Hefezopf.Contracts.ContractConsts.Namespace)]
    [System.Runtime.InteropServices.Guid("79f521b0-e048-479e-8c54-7a93b636e6a4")]
    internal interface IHefezopfWCFService
        : Hefezopf.Contracts.Communication.IHZTransportContract
        , Contracts.Communication.IHZServiceContract
    {
    }
}
