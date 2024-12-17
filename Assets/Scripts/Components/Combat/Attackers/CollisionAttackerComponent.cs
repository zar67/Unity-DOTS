using Unity.Entities;
using UnityEngine;

/// <summary>
/// The DOTs component that will be added to entities.
/// DOTs components shouldn't have any functions ideally.
/// </summary>
public struct CollisionAttacker : IComponentData
{
    public float AttackDelay;
    public float AttackTimer;
    public int Damage;
    public float CollisionDistance;
    public bool DestroyOnNoTarget;
    public bool DestroyOnHit;
}

/// <summary>
/// The <see cref="MonoBehaviour"/> that can be added to a <see cref="GameObject"/>.
/// This <see cref="MonoBehaviour"/> will be baked into a <see cref="CollisionAttacker"/> DOTs component through the internal <see cref="Baker"/> class.
/// </summary>
public class CollisionAttackerComponent : MonoBehaviour
{
    public float AttackDelay = 0.5f;
    public int Damage = 10;
    public float CollisionDistance = 1f;
    public bool DestroyOnNoTarget = false;
    public bool DestroyOnHit = false;

    /// <summary>
    /// A class to bake a <see cref="CollisionAttackerComponent"/> into a <see cref="CollisionAttacker"/> DOTs component.
    /// </summary>
    public class Baker : Baker<CollisionAttackerComponent>
    {
        public override void Bake(CollisionAttackerComponent authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new CollisionAttacker()
            {
                AttackDelay = authoring.AttackDelay,
                Damage = authoring.Damage,
                CollisionDistance = authoring.CollisionDistance,
                DestroyOnNoTarget= authoring.DestroyOnNoTarget,
                DestroyOnHit = authoring.DestroyOnHit,
            });
        }
    }
}
