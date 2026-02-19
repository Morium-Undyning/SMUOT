using UnityEngine;

public class ResourcesSystem : MonoBehaviour
{
    public static ResourcesSystem Instance { get; private set; }
    
    [SerializeField] private int wood;
    [SerializeField] private int stone;
    [SerializeField] private int oilCristall;
    
    public int Wood => wood;
    public int Stone => stone;
    public int OilCristall => oilCristall;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    public void AddWood(int w) => wood += w;
    public void AddStone(int s) => stone += s;
    public void AddCristall(int oc) => oilCristall += oc;
    
    public bool SubWood(int w)
    {
        if (wood >= w)
        {
            wood -= w;
            return true;
        }
        return false;
    }
    
    public bool SubStone(int s)
    {
        if (stone >= s)
        {
            stone -= s;
            return true;
        }
        return false;
    }
    
    public bool SubCristall(int oc)
    {
        if (oilCristall >= oc)
        {
            oilCristall -= oc;
            return true;
        }
        return false;
    }
}