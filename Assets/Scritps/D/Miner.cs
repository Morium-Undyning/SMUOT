using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Miner : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float stoppingDistance = 1.5f;

    [Header("Behavior")]
    [SerializeField] private float searchInterval = 1f;
    [SerializeField] private float restTime = 2f;

    [Header("References")]
    [SerializeField] private ResoursesUppers resourceCarrier;

    // Components
    private Animator animator;
    private NavMeshAgent agent;
    private GameObject towerBase;
    private Transform cachedTransform;

    // State
    private enum MinerState { Idle, MovingToResource, CarryingToBase, Resting }
    private MinerState currentState = MinerState.Idle;

    // Targets
    private GameObject targetResource;

    // Timers
    private float searchTimer;
    private float restTimer;

    // Resource reservation
    private static Dictionary<GameObject, GameObject> reservedResources = new Dictionary<GameObject, GameObject>();
    private GameObject[] allResources;

    void Awake()
    {
        cachedTransform = transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        resourceCarrier = GetComponent<ResoursesUppers>();
        towerBase = GameObject.FindGameObjectWithTag("Player");
    }

    void Start()
    {
        // Настройка NavMeshAgent
        agent.speed = moveSpeed;
        agent.stoppingDistance = stoppingDistance;
        agent.autoBraking = true;
        agent.autoRepath = true;

        currentState = MinerState.Idle;
        searchTimer = Random.Range(0f, searchInterval);
        restTimer = restTime;
    }

    void Update()
    {
        // Таймеры
        searchTimer -= Time.deltaTime;
        restTimer -= Time.deltaTime;

        // Поиск ресурсов
        if (searchTimer <= 0)
        {
            FindResources();
            searchTimer = searchInterval;
        }

        // Состояния
        switch (currentState)
        {
            case MinerState.Idle:
                UpdateIdle();
                break;
            case MinerState.MovingToResource:
                UpdateMovingToResource();
                break;
            case MinerState.CarryingToBase:
                UpdateCarryingToBase();
                break;
            case MinerState.Resting:
                UpdateResting();
                break;
        }

        // Анимация
        if (animator != null)
        {
            animator.SetBool("isRunning", agent.velocity.magnitude > 0.1f);
        }

        CleanupReservations();
    }

    void FindResources()
    {
        allResources = GameObject.FindGameObjectsWithTag("Resources");
    }

    void UpdateIdle()
    {
        // Если несет ресурс - несем на базу
        if (resourceCarrier.IsCarryingResource())
        {
            currentState = MinerState.CarryingToBase;
            return;
        }

        // Ищем свободный ресурс
        targetResource = GetAvailableResource();
        if (targetResource != null)
        {
            ReserveResource(targetResource);
            MoveTo(targetResource.transform.position);
            currentState = MinerState.MovingToResource;
        }
    }

    void UpdateMovingToResource()
    {
        // Проверка на наличие ресурса
        if (targetResource == null || !targetResource.activeInHierarchy)
        {
            ReleaseReservation();
            targetResource = null;
            currentState = MinerState.Idle;
            return;
        }

        // Проверка дистанции
        float distance = Vector3.Distance(cachedTransform.position, targetResource.transform.position);

        if (distance <= stoppingDistance + 0.5f)
        {
            // Собираем ресурс
            StopMoving();
            CollectResource();
        }
    }

    void CollectResource()
    {
        if (targetResource == null) return;

        Res resource = targetResource.GetComponent<Res>();
        if (resource != null)
        {
            // Забираем ресурс
            resourceCarrier.CollectResource(targetResource);
            ReleaseReservation();
            targetResource = null;
            currentState = MinerState.CarryingToBase;
        }
    }

    void UpdateCarryingToBase()
    {
        // Если нет ресурса - идем отдыхать
        if (!resourceCarrier.IsCarryingResource())
        {
            currentState = MinerState.Resting;
            restTimer = restTime;
            return;
        }

        // Идем к базе
        if (towerBase != null)
        {
            float distance = Vector3.Distance(cachedTransform.position, towerBase.transform.position);

            if (distance <= stoppingDistance + 1f)
            {
                // Сдаем ресурс
                StopMoving();
                resourceCarrier.DeliverResources();
                resourceCarrier.ResetResource();
                currentState = MinerState.Resting;
                restTimer = restTime;
            }
            else
            {
                MoveTo(towerBase.transform.position);
            }
        }
    }

    void UpdateResting()
    {
        StopMoving();

        if (restTimer <= 0)
        {
            ReleaseReservation();
            targetResource = null;
            currentState = MinerState.Idle;
        }
    }

    #region Movement
    void MoveTo(Vector3 position)
    {
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(position);
        }
    }

    void StopMoving()
    {
        if (agent.isOnNavMesh && agent.hasPath)
        {
            agent.ResetPath();
        }
    }
    #endregion

    #region Resource Reservation
    GameObject GetAvailableResource()
    {
        if (allResources == null || allResources.Length == 0) return null;

        foreach (GameObject resource in allResources)
        {
            if (resource != null && resource.activeInHierarchy && !IsResourceReserved(resource))
            {
                return resource;
            }
        }
        return null;
    }

    bool IsResourceReserved(GameObject resource)
    {
        if (reservedResources.ContainsKey(resource))
        {
            GameObject reservingMiner = reservedResources[resource];
            if (reservingMiner == null || !reservingMiner.activeInHierarchy)
            {
                reservedResources.Remove(resource);
                return false;
            }
            return reservingMiner != gameObject;
        }
        return false;
    }

    void ReserveResource(GameObject resource)
    {
        ReleaseReservation();
        reservedResources[resource] = gameObject;
    }

    void ReleaseReservation()
    {
        List<GameObject> toRemove = new List<GameObject>();
        foreach (var pair in reservedResources)
        {
            if (pair.Value == gameObject)
            {
                toRemove.Add(pair.Key);
            }
        }
        foreach (GameObject key in toRemove)
        {
            reservedResources.Remove(key);
        }
    }

    void CleanupReservations()
    {
        List<GameObject> toRemove = new List<GameObject>();
        foreach (var pair in reservedResources)
        {
            if (pair.Key == null || pair.Value == null || !pair.Value.activeInHierarchy)
            {
                toRemove.Add(pair.Key);
            }
        }
        foreach (GameObject key in toRemove)
        {
            reservedResources.Remove(key);
        }
    }
    #endregion

    void OnDestroy()
    {
        ReleaseReservation();
    }
}