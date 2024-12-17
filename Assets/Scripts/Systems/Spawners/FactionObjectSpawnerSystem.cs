using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct FactionObjectSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((RefRO<LocalTransform> transform, RefRW<FactionObjectSpawner> spawner) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<FactionObjectSpawner>>())
        {
            spawner.ValueRW.SpawnTimer += SystemAPI.Time.DeltaTime;

            if (spawner.ValueRO.SpawnTimer >= spawner.ValueRO.SpawnDelay)
            {
                Entity objectEntity = state.EntityManager.Instantiate(spawner.ValueRO.ObjectEntity);
                SystemAPI.SetComponent(objectEntity, LocalTransform.FromPosition(transform.ValueRO.Position));

                spawner.ValueRW.SpawnTimer = 0f;
            }
        }
    }
}
