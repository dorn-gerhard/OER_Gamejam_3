using UnityEngine;
using Zenject;

namespace Code
{
    public class DependencyInstaller : MonoInstaller
    {
        [SerializeField] private ScreenRoot screenRoot;
        
        public override void InstallBindings()
        {
            Container.BindInstance(screenRoot).AsSingle();
        }
    }
}