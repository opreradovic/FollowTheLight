using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Object pool for the player ghosting effects, wasn't really necessary but I had preformance in mind when making it,
//didn't want to have a lot of instantiate/destroys for something purely visual/unecessary

public class AfterImagePool : MonoBehaviour
{
    [SerializeField]
    private GameObject afterImagePrefab = null;

    private Queue<GameObject> availableObjects = new Queue<GameObject>();

    public static AfterImagePool Instance { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        GrowPool();
    }

    private void GrowPool()
    {
        for (int i = 0; i < 10; i++)
        {
            var instanceToAdd = Instantiate(afterImagePrefab);
            instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);
        }
    }
    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        availableObjects.Enqueue(instance);
    }

    public GameObject GetFromPool()
    {
        if(availableObjects.Count == 0)
            GrowPool();

        var instance = availableObjects.Dequeue();
        instance.SetActive(true);
        return instance;
    }
}
