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
        UnitMovementJob unitMovementJob = new UnitMovementJob()
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        };

        unitMovementJob.ScheduleParallel();

        /// Entity components are structs and so the original component will not be modified as it is not a reference but a value type.
        /// <see cref="RefRW{T}"/> gets a reference of the struct so that when we modify the 'local copy' we will actually be updating the original.
        /// The RW of <see cref="RefRW{T}"/> stands for Read and Write so we can both read the data and edit it.
        /// There is also <see cref="RefRO{T}"/> which stands for Reference Read Only. 
        /// <see cref="RefRO{T}"/> is preferred if you don't need to modify the data as it can be ran on multiple threads at the same time.
        //foreach ((RefRW<LocalTransform> transform, RefRW <PhysicsVelocity> velocity, RefRO <UnitMovement> movement) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PhysicsVelocity>, RefRO<UnitMovement>>())
        //{
        //    if (math.distance(movement.ValueRO.TargetPosition, transform.ValueRO.Position) > movement.ValueRO.TargetDistance)
        //    {
        //        float3 moveDirection = movement.ValueRO.TargetPosition - transform.ValueRO.Position;
        //        moveDirection = math.normalize(moveDirection);
        //        quaternion targetRotation = quaternion.LookRotation(moveDirection, math.up());
        //        transform.ValueRW.Rotation = math.slerp(transform.ValueRO.Rotation, targetRotation, SystemAPI.Time.DeltaTime * movement.ValueRO.TurnSpeed);

        //        velocity.ValueRW.Linear = moveDirection * movement.ValueRO.MoveSpeed;
        //        velocity.ValueRW.Angular = float3.zero;
        //    }
        //    else
        //    {
        //        velocity.ValueRW.Linear = float3.zero;
        //    }
        //}
    }
}

/// Jobs run on multiple threads in parallel.
/// If there are not that many entities found by the query defined by the arguments given to the execute function) then the job may only run on one thread.
[BurstCompile]
public partial struct UnitMovementJob : IJobEntity
{
    public float DeltaTime;

    /// Use <see cref="ref"/> for arguments that need to be both readable and writeable.
    /// Use <see cref="in"/> for arguments that only need to be readable.
    public void Execute(ref LocalTransform transform, ref PhysicsVelocity velocity, in UnitMovement movement)
    {
        float3 moveDirection = movement.TargetPosition - transform.Position;
        if (math.lengthsq(moveDirection) > movement.TargetDistance)
        {
            moveDirection = math.normalize(moveDirection);
            quaternion targetRotation = quaternion.LookRotation(moveDirection, math.up());
            transform.Rotation = math.slerp(transform.Rotation, targetRotation, DeltaTime * movement.TurnSpeed);

            velocity.Linear = moveDirection * movement.MoveSpeed;
            velocity.Angular = float3.zero;
        }
        else
        {
            velocity.Linear = float3.zero;
        }
    }
}