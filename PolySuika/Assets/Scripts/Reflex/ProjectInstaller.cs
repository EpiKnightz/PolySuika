using UnityEngine;
using Reflex.Core;

public class ProjectInstaller : MonoBehaviour, IInstaller
{
    public void InstallBindings(ContainerBuilder builder)
    {
        //builder.AddSingleton(this, typeof(IInstaller));
    }
}
