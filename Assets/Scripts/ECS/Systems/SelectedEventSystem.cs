using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

partial struct SelectedEventSystem : ISystem
{
    //public void OnUpdate(ref SystemState state)
    //{
    //    foreach((RefRW<SelectedEventComponent> selectedEventComponent, Entity entity) in SystemAPI.Query<RefRW<SelectedEventComponent>>().WithEntityAccess())
    //    {
    //        if(selectedEventComponent.ValueRO.onSelected)
    //        {
    //            Debug.Log("onSelected");
    //            selectedEventComponent.ValueRW.onSelected = false;
    //        }
    //        else if(selectedEventComponent.ValueRO.onDeselected)
    //        {
    //            Debug.Log("onDeselected");
    //            selectedEventComponent.ValueRW.onDeselected = false;
    //        }

    //        if(selectedEventComponent.ValueRO.onInteract)
    //        {
    //            Debug.Log("onInteract");



    //            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    //            // Get the ECB system and create a command buffer
    //            var ecbSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<BeginSimulationEntityCommandBufferSystem>();
    //            EntityCommandBuffer ecb = ecbSystem.CreateCommandBuffer();

    //            // Create a queue to hold all descendants (BFS traversal)
    //            using NativeList<Entity> entitiesToProcess = new NativeList<Entity>(Allocator.Temp);

    //            // Initialize the queue with the immediate children of the root entity
    //            if(entityManager.HasComponent<Child>(entity))
    //            {
    //                DynamicBuffer<Child> rootChildren = entityManager.GetBuffer<Child>(entity);
    //                foreach(Child child in rootChildren)
    //                {
    //                    entitiesToProcess.Add(child.Value);
    //                }
    //            }

    //            // Process all entities in the queue
    //            while(entitiesToProcess.Length > 0)
    //            {
    //                // Dequeue the first entity
    //                Entity currentEntity = entitiesToProcess[0];
    //                entitiesToProcess.RemoveAt(0);

    //                // Process the current entity
    //                if(entityManager.HasComponent<RenderMeshArray>(currentEntity))
    //                {
    //                    RenderMeshArray renderMeshArray = entityManager.GetSharedComponentManaged<RenderMeshArray>(currentEntity);

    //                    // Modify materials
    //                    List<UnityObjectRef<Material>> mats = new List<UnityObjectRef<Material>>(renderMeshArray.MaterialReferences);
    //                    foreach(Material material in GameManager.Instance.outlineMaterials)
    //                    {
    //                        mats.Add(material);
    //                    }

    //                    renderMeshArray.MaterialReferences = mats.ToArray();
    //                    ecb.SetSharedComponentManaged(currentEntity, renderMeshArray);
    //                }

    //                // Enqueue children of the current entity
    //                if(entityManager.HasComponent<Child>(currentEntity))
    //                {
    //                    DynamicBuffer<Child> currentChildren = entityManager.GetBuffer<Child>(currentEntity);
    //                    foreach(Child child in currentChildren)
    //                    {
    //                        entitiesToProcess.Add(child.Value);
    //                    }
    //                }
    //            }



    //            selectedEventComponent.ValueRW.onInteract = false;
    //        }
    //    }
    //}
}
