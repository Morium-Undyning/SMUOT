using UnityEngine;

public class ResoursesUppers : MonoBehaviour, IRes
{
    public enum Resourses { Wood, Stone, OilCristall, None }
    private ResourcesSystem _resourcesSystem;
    public Resourses thisRes;

    [SerializeField] private int resourceAmount = 10;
    [SerializeField] public GameObject carriedResource;

    void Start()
    {
        _resourcesSystem = FindFirstObjectByType<ResourcesSystem>();
    }

    public void CollectResource(GameObject resourceObject)
    {
        Res resComponent = resourceObject.GetComponent<Res>();
        if (resComponent != null)
        {
            thisRes = (Resourses)resComponent.ress;

            Destroy(resourceObject);

            if (carriedResource != null)
            {
                carriedResource.SetActive(true);
                
            }
        }
    }

    public void DeliverResources()
    {
        if (_resourcesSystem == null) return;

        switch (thisRes)
        {
            case Resourses.Wood:
                _resourcesSystem.AddWood(resourceAmount);
                break;
            case Resourses.Stone:
                _resourcesSystem.AddStone(resourceAmount);
                break;
            case Resourses.OilCristall:
                _resourcesSystem.AddCristall(resourceAmount);
                break;
        }
    }

    public void ResetResource()
    {
        thisRes = Resourses.None;
        if (carriedResource != null)
        {
            carriedResource.SetActive(false);
        }
    }

    public bool IsCarryingResource()
    {
        return thisRes != Resourses.None;
    }

    public Resourses GetCarriedResourceType()
    {
        return thisRes;
    }
}