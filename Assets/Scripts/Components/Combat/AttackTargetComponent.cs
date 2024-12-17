using Unity.Entities;
using UnityEngine;

/// <summary>
/// The DOTs component that will be added to entities.
/// DOTs components shouldn't have any functions ideally.
/// </summary>
public struct AttackTarget : IComponentData
{
    public Entity ActiveTarget;
}

/// <summary>
/// The <see cref="MonoBehaviour"/> that can be added to a <see cref="GameObject"/>.
/// This <see cref="MonoBehaviour"/> will be baked into a <see cref="AttackTarget"/> DOTs component through the internal <see cref="Baker"/> class.
/// </summary>
public class AttackTargetComponent : MonoBehaviour
{
    /// <summary>
    /// A class to bake a <see cref="AttackTargetComponent"/> into a <see cref="AttackTarget"/> DOTs component.
    /// </summary>
    public class Baker : Baker<AttackTargetComponent>
    {
        public override void Bake(AttackTargetComponent authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new AttackTarget()
            {

            });
        }
    }
}