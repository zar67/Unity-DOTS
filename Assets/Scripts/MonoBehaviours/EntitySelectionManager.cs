using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class EntitySelectionManager : MonoBehaviourSingleton<EntitySelectionManager>
{
    public event EventHandler<Vector2> OnSelectionAreaStart;
    public event EventHandler<Vector2> OnSelectionAreaEnd;

    [SerializeField] private float m_minimumSelectionAreaSize = 40f;
    [SerializeField] private float m_idealDistanceBetweenEntities = 2f;

    private Vector2 m_selectionStartMousePosition;

    public Rect GetSelectionArea()
    {
        Vector2 selectionEndMousePosition = Input.mousePosition;

        var lowerLeftCorner = new Vector2(
            Mathf.Min(m_selectionStartMousePosition.x, selectionEndMousePosition.x),
            Mathf.Min(m_selectionStartMousePosition.y, selectionEndMousePosition.y));

        var upperRightCorner = new Vector2(
            Mathf.Max(m_selectionStartMousePosition.x, selectionEndMousePosition.x),
            Mathf.Max(m_selectionStartMousePosition.y, selectionEndMousePosition.y));

        return new Rect(
            lowerLeftCorner.x,
            lowerLeftCorner.y,
            upperRightCorner.x - lowerLeftCorner.x,
            upperRightCorner.y - lowerLeftCorner.y);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_selectionStartMousePosition = Input.mousePosition;

            OnSelectionAreaStart?.Invoke(this, m_selectionStartMousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            SelectEntities();

            OnSelectionAreaEnd?.Invoke(this, Input.mousePosition);
        }

        if (Input.GetMouseButtonDown(1))
        {
            SetTargetPositionsOfSelectedEntities();
        }
    }

    private void SelectEntities()
    {
        DeselectAllEntities();

        Rect selectionArea = GetSelectionArea();
        float areaSize = selectionArea.width + selectionArea.height;

        if (areaSize > m_minimumSelectionAreaSize)
        {
            SelectAllEntitiesInSelectionArea(selectionArea);
        }
        else
        {
            SelectSingleEntity();
        }
    }

    private void SelectAllEntitiesInSelectionArea(Rect selectionArea)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform>().WithPresent<Selected>().Build(entityManager);
        NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
        NativeArray<Selected> selectedArray = entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);
        NativeArray<LocalTransform> transformArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
        for (int i = 0; i < transformArray.Length; i++)
        {
            LocalTransform transform = transformArray[i];
            Vector2 entityScreenPosition = Camera.main.WorldToScreenPoint(transform.Position);
            if (selectionArea.Contains(entityScreenPosition))
            {
                entityManager.SetComponentEnabled<Selected>(entityArray[i], true);
                Selected selected = selectedArray[i];
                selected.OnSelected = true;

                /// Cannot use <see cref="EntityQuery.CopyFromComponentDataArray"/> after this for loop as when we disable the component the <see cref="EntityQuery"/> will update.
                /// Meaning the length of data in the <see cref="EntityQuery"/> will be 0, but will have more than 0 items in the selectedArray.
                entityManager.SetComponentData(entityArray[i], selected);
            }
        }
    }

    private void SelectSingleEntity()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        /// Alternative way of writing an <see cref="EntityQuery"/> instead of using <see cref="EntityQueryBuilder"/>.
        EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));

        PhysicsWorldSingleton physics = entityQuery.GetSingleton<PhysicsWorldSingleton>();
        UnityEngine.Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        var raycastInput = new RaycastInput()
        {
            Start = cameraRay.GetPoint(0f),
            End = cameraRay.GetPoint(999999f),
            Filter = new CollisionFilter()
            {
                BelongsTo = ~0u, // Any layer
                CollidesWith = ~0u, // Any layer
                GroupIndex = 0
            }
        };

        if (physics.CollisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit hit) && 
            entityManager.HasComponent<Selected>(hit.Entity))
        {
            entityManager.SetComponentEnabled<Selected>(hit.Entity, true);
            Selected selected = entityManager.GetComponentData<Selected>(hit.Entity);
            selected.OnSelected = true;
            entityManager.SetComponentData(hit.Entity, selected);
        }
    }

    private void DeselectAllEntities()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        /// <see cref="Allocator"/> determines the lifetime of the memory.
        /// <see cref="Allocator.Temp"/> means the memory will be cleaned up at the end of the frame.
        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, Selected>().Build(entityManager);
        NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
        NativeArray<Selected> selectedArray = entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);
        for (int i = 0; i < entityArray.Length; i++)
        {
            entityManager.SetComponentEnabled<Selected>(entityArray[i], false);
            Selected selected = selectedArray[i];
            selected.OnDeselected = true;

            /// Cannot use <see cref="EntityQuery.CopyFromComponentDataArray"/> after this for loop as when we disable the component the <see cref="EntityQuery"/> will update.
            /// Meaning the length of data in the <see cref="EntityQuery"/> will be 0, but will have more than 0 items in the selectedArray.
            entityManager.SetComponentData(entityArray[i], selected);
        }
    }

    private void SetTargetPositionsOfSelectedEntities()
    {
        Vector3 mouseWorldPosition = MousePositionManager.Instance.GetWorldPosition();

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        /// <see cref="Allocator"/> determines the lifetime of the memory.
        /// <see cref="Allocator.Temp"/> means the memory will be cleaned up at the end of the frame.
        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<TargetPositionMovement, Selected>().Build(entityManager);

        NativeArray<TargetPositionMovement> movementArray = entityQuery.ToComponentDataArray<TargetPositionMovement>(Allocator.Temp);

        NativeArray<float3> targetPositions = GetFormationTargetPositions(mouseWorldPosition, movementArray.Length);

        for (int i = 0; i < movementArray.Length; i++)
        {
            TargetPositionMovement movement = movementArray[i];
            movement.Target = targetPositions[i];
            movement.IsMovingToTarget = true;
            movementArray[i] = movement;
        }

        // Update the data on all the components.
        // This is needed because the components are structs (a type value) not a reference value so any changes will apply to the copy of the data not the original data.
        entityQuery.CopyFromComponentDataArray(movementArray);
    }

    /// <summary>
    /// Gets a formation of positions around a central point in rings out from the origin.
    /// </summary>
    /// <param name="origin">The central point for the formation.</param>
    /// <param name="positionCount">The number of positions to generate.</param>
    /// <returns>An array of positions with the length of <paramref name="positionCount"/></returns>
    private NativeArray<float3> GetFormationTargetPositions(float3 origin, int positionCount)
    {
        var positionArray = new NativeArray<float3>(positionCount, Allocator.Temp);

        if (positionCount > 0)
        {
            positionArray[0] = origin;
        }
        
        if (positionCount > 1)
        {
            int ringCount = 1;
            int positionIndex = 1;
            while (positionIndex < positionCount)
            {
                float ringRadius = ringCount * m_idealDistanceBetweenEntities;
                float ringCircumfrence = math.PI * ringRadius * 2;
                int positionsInRing = (int)math.floor(ringCircumfrence / m_idealDistanceBetweenEntities);
                for (int i = 0; i < positionsInRing; i++)
                {
                    float angle = i * (math.PI2 / positionsInRing);
                    float3 positionVector = math.rotate(quaternion.RotateY(angle), new float3(0, 0, ringRadius));

                    positionArray[positionIndex] = origin + positionVector;
                    positionIndex++;

                    if (positionIndex >= positionCount)
                    {
                        break;
                    }
                }

                ringCount++;
            }
        }

        return positionArray;
    }
}
