using Unity.Entities;
using UnityEngine;

/// <summary>
/// The DOTs component that will be added to entities.
/// DOTs components shouldn't have any functions ideally.
/// </summary>
public struct FactionObjectSpawner : IComponentData
{
    public float SpawnDelay;
    public float SpawnTimer;
    public Entity ObjectEntity;
}

/// <summary>
/// The <see cref="MonoBehaviour"/> that can be added to a <see cref="GameObject"/>.
/// This <see cref="MonoBehaviour"/> will be baked into a <see cref="FactionObjectSpawner"/> DOTs component through the internal <see cref="Baker"/> class.
/// </summary>
public class FactionObjectSpawnerComponent : MonoBehaviour
{
    public float SpawnDelay = 1f;
    public GameObject ObjectPrefab;

    /// <summary>
    /// A class to bake a <see cref="FactionObjectSpawnerComponent"/> into a <see cref="FactionObjectSpawner"/> DOTs component.
    /// </summary>
    public class Baker : Baker<FactionObjectSpawnerComponent>
    {
        public override void Bake(FactionObjectSpawnerComponent authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new FactionObjectSpawner()
            {
                SpawnDelay = authoring.SpawnDelay,
                ObjectEntity = GetEntity(authoring.ObjectPrefab, TransformUsageFlags.Dynamic),
            });
        }
    }
}
