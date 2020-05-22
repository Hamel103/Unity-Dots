using Unity.Entities;
using UnityEngine;

using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Mesh unitMesh;
    [SerializeField] private Material unitMaterial;
    [SerializeField] private GameObject gameObjectPrefab;

    [SerializeField] private int xSize = 10;
    [SerializeField] private int ySize = 10;
    [Range(0.1f,2f)]
    [SerializeField] private float spacing = 1f;

    private Entity entityPrefab;
    private World defaultWorld;
    private EntityManager entityManager;

    // Start is called before the first frame update
    void Start()
    {
        //MakeEntity();

        defaultWorld = World.DefaultGameObjectInjectionWorld;
        entityManager = defaultWorld.EntityManager;

        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(defaultWorld, null);
        entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(gameObjectPrefab, settings);

        InstantiateEntityGrid(xSize, ySize, spacing);
    }

    private void MakeEntity()
    {
        //Pulls the EntityManager in the world
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        //Determines the type of data that this element will store
        EntityArchetype archetype = entityManager.CreateArchetype(
            typeof(Translation),    //Allows to move and rotate in world space
            typeof(Rotation),       //  ^^
            typeof(RenderMesh),     //Used to render the entity on screen
            typeof(RenderBounds),   //  ^^
            typeof(LocalToWorld)    //  ^^
            );
        
        //Creates the entity
        Entity myEntity = entityManager.CreateEntity(archetype);

        //Places the entity at this position
        entityManager.AddComponentData(myEntity, new Translation { Value = new float3(2f, 0f, 4f) });

        //Applies the model and material to the entity
        entityManager.AddSharedComponentData(myEntity, new RenderMesh 
            {
                mesh = unitMesh,
                material = unitMaterial
            });
    }

    private void InstantiateEntity(float3 position)
    {
        //Creates the entity
        Entity myEntity = entityManager.Instantiate(entityPrefab);

        //Places the entity at this position
        entityManager.SetComponentData(myEntity, new Translation
            {
                Value = position
            });
    }

    private void InstantiateEntityGrid(int dimX, int dimY, float space = 1f)
    {
        for (int i = 0; i < dimX; i++)
        {
            for (int j = 0; j < dimY; j++)
            {
                InstantiateEntity(new float3(i * space, j * space, 0f));
            }
        }
    }
}
