using UnityEngine;

public class Res : MonoBehaviour, IRes
{
    public enum Resourses { Wood, Stone, OilCristall, None }
    public Resourses ress;

    [SerializeField] private int resourceValue = 10;
    [SerializeField] private GameObject woodModel;
    [SerializeField] private GameObject stoneModel;
    [SerializeField] private GameObject oilModel;

    void Start()
    {
        GenerateRandomResource();
    }

    private void GenerateRandomResource()
    {
        woodModel.SetActive(false);
        stoneModel.SetActive(false);
        if (oilModel != null) oilModel.SetActive(false);

        int i = Random.Range(0, 3);

        switch (i)
        {
            case 0:
                ress = Resourses.Wood;
                woodModel.SetActive(true);
                break;
            case 1:
                ress = Resourses.Stone;
                stoneModel.SetActive(true);
                break;
        }
    }

    public Resourses GetResourceType()
    {
        return ress;
    }

    public int GetResourceValue()
    {
        return resourceValue;
    }

    public void SetupResource(Resourses type, int value = 10)
    {
        ress = type;
        resourceValue = value;

        woodModel.SetActive(false);
        stoneModel.SetActive(false);
        if (oilModel != null) oilModel.SetActive(false);

        switch (type)
        {
            case Resourses.Wood:
                woodModel.SetActive(true);
                break;
            case Resourses.Stone:
                stoneModel.SetActive(true);
                break;
            case Resourses.OilCristall:
                if (oilModel != null) oilModel.SetActive(true);
                break;
        }
    }
}