using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateBefore(typeof(SelectedResetEventsSystem))]
partial struct SelectedVisualsSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRO<Selected> selected in SystemAPI.Query<RefRO<Selected>>().WithPresent<Selected>())
        {
            if (selected.ValueRO.OnSelected)
            {
                RefRW<LocalTransform> visualTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.SelectedVisual);
                visualTransform.ValueRW.Scale = selected.ValueRO.SelectedShowScale;
            }
            else if (selected.ValueRO.OnDeselected)
            {
                RefRW<LocalTransform> visualTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.SelectedVisual);
                visualTransform.ValueRW.Scale = 0;
            }
        }
    }
}
