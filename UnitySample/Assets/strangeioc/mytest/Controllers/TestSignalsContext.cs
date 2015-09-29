using System;
using UnityEngine;
using strange.extensions.context.impl;
using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace StrangeTest
{

    public class TestSignalsContext : MVCSContext
    {

        public TestSignalsContext(MonoBehaviour contextView)
            : base(contextView)
        {
        }

        protected override void addCoreComponents()
        {
            base.addCoreComponents();

            // bind signal command binder      
            injectionBinder.Unbind<ICommandBinder>();
            injectionBinder.Bind<ICommandBinder>().To<SignalCommandBinder>().ToSingleton();
        } 

        protected override void mapBindings()
        {
            base.mapBindings();
       
            commandBinder.Bind<StartSignal>().To<StartCommand>().Once();
            commandBinder.Bind<DoManagementSignal>().To<DoManagementCommand>().Pooled();

            mediationBinder.Bind<TestSignalsView>().To<TestSignalsMediator>();

            injectionBinder.Bind<ISomeManager>().To<ManagerAsNormalClass>().ToSingleton();
        }

        public override void Launch()
        {
            base.Launch();

            Signal startSignal = injectionBinder.GetInstance<StartSignal>();
            startSignal.Dispatch();
        } 
    }
}