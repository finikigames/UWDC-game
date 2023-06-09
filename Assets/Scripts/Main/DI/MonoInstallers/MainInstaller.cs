﻿using Main.Services.Startup;
using Main.States;
using Main.UI.Data;
using Zenject;

namespace Main.DI.MonoInstallers {
    public class MainInstaller : MonoInstaller {
        public override void InstallBindings() {
            Container
                .BindInterfacesAndSelfTo<MainSceneService>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<MainRootState>()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<CheckersInitialize>()
                .AsSingle()
                .NonLazy();

            Container
                .DeclareSignal<ToCheckersMetaSignal>();
        }
    }
}