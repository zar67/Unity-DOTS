using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct UnitMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        /// Entity components are structs and so the original component will not be modified as it is not a reference but a value type.
        /// <see cref="RefRW{T}"/> gets a reference of the struct so that when we modify the 'local copy' we will actually be updating the original.
        /// The RW of <see cref="RefRW{T}"/> stands for Read and Write so we can both read the data and edit it.
        /// There is also <see cref="RefRO{T}"/> which stands for Reference Read Only. 
        /// <see cref="RefRO{T}"/> is preferred if you don't need to modify the data as it can be ran on multiple threads at the same time.
        foreach ((RefRW<LocalTransform> transform, RefRO<MoveSpeed> moveSpeed) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveSpeed>>())
        {
            transform.ValueRW.Position += new float3(moveSpeed.ValueRO.Value, 0, 0) * SystemAPI.Time.DeltaTime;
        }
    }
}
