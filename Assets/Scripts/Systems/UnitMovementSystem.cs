using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct UnitMovementSystem : ISystem
{
    /// Cannot use <see cref="BurstCompileAttribute"/> with managed (class) data like <see cref="UnityEngine.MonoBehaviour"/>.
    /// This will give an error to Unity if both are used, but will not stop compilation.
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        /// Entity components are structs and so the original component will not be modified as it is not a reference but a value type.
        /// <see cref="RefRW{T}"/> gets a reference of the struct so that when we modify the 'local copy' we will actually be updating the original.
        /// The RW of <see cref="RefRW{T}"/> stands for Read and Write so we can both read the data and edit it.
        /// There is also <see cref="RefRO{T}"/> which stands for Reference Read Only. 
        /// <see cref="RefRO{T}"/> is preferred if you don't need to modify the data as it can be ran on multiple threads at the same time.
        foreach ((RefRW<LocalTransform> transform, RefRW <PhysicsVelocity> velocity, RefRO <UnitMovement> movement) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PhysicsVelocity>, RefRO<UnitMovement>>())
        {
            float3 moveDirection = movement.ValueRO.TargetPosition - transform.ValueRO.Position;
            moveDirection = math.normalize(moveDirection);

            quaternion targetRotation = quaternion.LookRotation(moveDirection, math.up());
            transform.ValueRW.Rotation = math.slerp(transform.ValueRO.Rotation, targetRotation, SystemAPI.Time.DeltaTime * movement.ValueRO.TurnSpeed);

            velocity.ValueRW.Linear = moveDirection * movement.ValueRO.MoveSpeed;
            velocity.ValueRW.Angular = float3.zero;
        }
    }
}
