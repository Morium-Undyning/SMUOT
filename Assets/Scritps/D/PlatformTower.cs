using UnityEngine;
using UnityEngine.EventSystems;

public class PlatformTower : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject tower;
    [SerializeField] private GameObject panelBuild;

    [SerializeField] private bool ispanelActive = false;
    [SerializeField] private bool isbuild = false;
    [SerializeField] private MeshRenderer mr;

    [SerializeField] private Material act;
    [SerializeField] private Material NoAct;

    [SerializeField] private ResourcesSystem rs;
    
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        rs = FindFirstObjectByType<ResourcesSystem>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameObject.CompareTag("Platform"))
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                ActiveDesckTowerBuild();
            }
        }
        
    }

    public void ActiveTower()
    {
        panelBuild.SetActive(true);
        ispanelActive = true;
        mr.material = act;
    }

    public void NoActiveTower()
    {
        panelBuild.SetActive(false);
        ispanelActive = false;
        mr.material = NoAct;
    }

    public void ActiveDesckTowerBuild()
    {
        //Debug.Log("p");
        if(!isbuild)
        {
            if (ispanelActive&&panelBuild.activeSelf==true)
            {
                NoActiveTower();
            }
            else if (!ispanelActive&&panelBuild.activeSelf==false) 
            {
                ActiveTower();
            }
        }
        
    }

    public void BuildTower()
    {
        if (rs.Stone >= 20 && rs.Wood >= 20 && rs.OilCristall >=10)
        {
            if (!isbuild && ispanelActive)
            {
                Instantiate(tower, this.transform.position, this.transform.rotation);
                panelBuild.SetActive(false);
                ispanelActive = false;
                isbuild = true;
                rs.SubStone(20);
                rs.SubCristall(10);
                rs.SubWood(20);
                NoActiveTower();
            }
        }
        
        
    }
    void Update()
    {
        
    }
}
