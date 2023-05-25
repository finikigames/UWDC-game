using UnityEngine;
using Zenject;

namespace Core.ZenjectContextCallbacks {
    public class InstallerAggregator : MonoBehaviour,
                                       IInstallerPostResolve,
                                       IInstallerPreResolve,
                                       IInstallerPreInstall,
                                       IInstallerPostInstall {
        private void OnEnable() {
            var context = GetComponent<Context>();
            
            if (context is SceneContext sceneContext) {
                Subscribe(sceneContext);
            }

            if (context is GameObjectContext gameObjectContext) {
                Subscribe(gameObjectContext);
            }
        }

        private void Subscribe(SceneContext context) {
            context.OnPreInstall.AddListener(OnPreInstall);
            context.OnPostInstall.AddListener(OnPostInstall);
            context.OnPreResolve.AddListener(OnPreResolve);
            context.OnPostResolve.AddListener(OnPostResolve);
        }
        
        private void Subscribe(GameObjectContext context) {
            context.PreInstall += OnPreInstall;
            context.PostInstall += OnPostInstall;
            context.PreResolve += OnPreResolve;
            context.PostResolve += OnPostResolve;
        }

        public void OnPreInstall() { 
            var context = GetComponent<Context>();
            var installers = context.Installers;
            foreach (var installer in installers) {
                if (installer is IInstallerPreInstall installerCallbacks) {
                    installerCallbacks.OnPreInstall();
                }
            }
        }

        public void OnPostInstall() {
            var context = GetComponent<Context>();
            var installers = context.Installers;
            foreach (var installer in installers) {
                if (installer is IInstallerPostInstall installerCallbacks) {
                    installerCallbacks.OnPostInstall();
                }
            }
        }

        public void OnPreResolve() {
            var context = GetComponent<Context>();
            var installers = context.Installers;
            foreach (var installer in installers) {
                if (installer is IInstallerPreResolve installerCallbacks) {
                    installerCallbacks.OnPreResolve();
                }
            }
        }

        public void OnPostResolve() {
            var context = GetComponent<Context>();
            var installers = context.Installers;
            foreach (var installer in installers) {
                if (installer is IInstallerPostResolve installerCallbacks) {
                    installerCallbacks.OnPostResolve();
                }
            }
        }
    }
}