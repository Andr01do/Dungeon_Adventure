using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine.AI;

public class LevelGenerator : MonoBehaviour
{
    [Header("Configuration")]
    public DungeonRoomFactory factory;
    public int maxRooms = 20;
    public LayerMask roomLayer;

    [Header("Navigation")]
    public NavMeshSurface navMeshSurface;

    [Header("Statistics")]
    public int totalEnemies = 0;

    private List<Room> spawnedPieces = new List<Room>();
    private int roomCount = 0;
    private Queue<Transform> pendingConnectors = new Queue<Transform>();
    private Room startRoomInstance;

    private IEnumerator Start()
    {
        int attempt = 1;

        while (true)
        {
            if (GenerateDungeon())
            {
                yield return new WaitForFixedUpdate();
                Physics.SyncTransforms();
                BuildNavMesh();
                yield return null; 

                foreach (var room in spawnedPieces)
                {
                    SpawnEnemies(room);
                }
                SpawnPlayer();

                Debug.Log($"<color=green>Рівень згенеровано! Спроба №{attempt}. Ворогів: {totalEnemies}</color>");

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.SetTotalEnemies(totalEnemies);
                }

                yield break; 
            }

            ResetLevel();
            attempt++;
            yield return null;
        }
    }

    void BuildNavMesh()
    {
        if (navMeshSurface != null)
        {
            navMeshSurface.RemoveData();
            navMeshSurface.BuildNavMesh();
        }
        else
        {
            Debug.LogError("LevelGenerator: Не призначено NavMeshSurface!");
        }
    }

    void SpawnPlayer()
    {
        if (GameManager.Instance == null) return;

        if (startRoomInstance != null)
        {
            if (startRoomInstance.playerSpawnPoint != null)
            {
                GameManager.Instance.SpawnPlayerAt(startRoomInstance.playerSpawnPoint);
            }
            else
            {
                GameObject tempSpawn = new GameObject("TempSpawnPoint");
                tempSpawn.transform.position = startRoomInstance.transform.position + Vector3.up * 1f;
                GameManager.Instance.SpawnPlayerAt(tempSpawn.transform);
                Destroy(tempSpawn, 1f);
            }
        }
    }

    void SpawnEnemies(Room room)
    {
        if (room.type == RoomType.Start || room.type == RoomType.Corridor) return;
        if (room.type == RoomType.Boss)
        {
            GameObject bossPrefab = factory.GetBoss();
            Transform spawnPoint = room.bossSpawnPoint;

            if (spawnPoint == null && room.enemySpawnPoints.Count > 0)
                spawnPoint = room.enemySpawnPoints[0];

            if (bossPrefab != null && spawnPoint != null)
            {
                GameObject spawnedBoss = Instantiate(bossPrefab, spawnPoint.position, spawnPoint.rotation, room.transform);
                totalEnemies++;
                SafeWarpToNavMesh(spawnedBoss, spawnPoint.position);

                if (room.bossTrigger != null) room.bossTrigger.Setup(spawnedBoss);
            }
            return;
        }

        if (room.type == RoomType.Normal && room.enemySpawnPoints != null)
        {
            foreach (Transform spawnPoint in room.enemySpawnPoints)
            {

                if (Random.value > 0.4f)
                {
                    GameObject enemyPrefab = factory.GetRandomEnemy();
                    if (enemyPrefab != null)
                    {
                        GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation, room.transform);
                        totalEnemies++;
                        SafeWarpToNavMesh(spawnedEnemy, spawnPoint.position);
                    }
                }
            }
        }
    }

    void SafeWarpToNavMesh(GameObject enemy, Vector3 position)
    {
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        if (agent != null)
        {

            if (NavMesh.SamplePosition(position, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }
    }


    bool GenerateDungeon()
    {
        totalEnemies = 0;
        startRoomInstance = Instantiate(factory.GetStartRoom(), Vector3.zero, Quaternion.identity);
        Room startRoom = startRoomInstance;

        RegisterPiece(startRoom, null);

        bool bossSpawned = false;
        int iterations = 0;

        while (roomCount < maxRooms && pendingConnectors.Count > 0 && iterations < 5000)
        {
            iterations++;
            Transform sourceConnector = pendingConnectors.Dequeue();
            if (sourceConnector == null) continue;

            Room sourceRoomScript = sourceConnector.GetComponentInParent<Room>();
            if (sourceRoomScript == null || !sourceRoomScript.connectors.Contains(sourceConnector)) continue;

            bool wantCorridor = (sourceRoomScript.type != RoomType.Corridor);
            bool piecePlaced = false;
            bool readyForBoss = roomCount >= maxRooms - 2;

            if (!wantCorridor && readyForBoss && !bossSpawned)
            {
                if (TryPlaceRoom(factory.GetBossRoom(), sourceConnector, sourceRoomScript))
                {
                    piecePlaced = true;
                    bossSpawned = true;
                }
            }

            if (!piecePlaced && !bossSpawned)
            {
                foreach (var option in factory.GetRandomizedOptions(wantCorridor))
                {
                    if (TryPlaceRoom(option, sourceConnector, sourceRoomScript))
                    {
                        piecePlaced = true;
                        break;
                    }
                }
            }
        }

        if (bossSpawned)
        {
            PruneDeadEnds();
            SealDungeon();
            return true;
        }

        return false;
    }

    bool TryPlaceRoom(Room prefab, Transform sourceConnector, Room sourceRoom)
    {
        Room newPiece = Instantiate(prefab);
        foreach (var newConnector in newPiece.connectors)
        {
            newPiece.transform.rotation = Quaternion.identity;
            Quaternion targetRotation = Quaternion.LookRotation(-sourceConnector.forward, Vector3.up);
            newPiece.transform.rotation = targetRotation * Quaternion.Inverse(newConnector.localRotation);

            Vector3 offset = newConnector.position - newPiece.transform.position;
            newPiece.transform.position = sourceConnector.position - offset;

            if (!CheckOverlap(newPiece))
            {
                RegisterPiece(newPiece, sourceRoom);
                newPiece.DisableConnector(newConnector);
                sourceRoom.DisableConnector(sourceConnector);
                return true;
            }
        }
        DestroyImmediate(newPiece.gameObject);
        return false;
    }

    void RegisterPiece(Room piece, Room parent)
    {
        spawnedPieces.Add(piece);
        piece.transform.SetParent(this.transform);

        if (parent != null) parent.childRooms.Add(piece);

        if (piece.type == RoomType.Normal || piece.type == RoomType.Boss) roomCount++;

        foreach (var c in piece.connectors) pendingConnectors.Enqueue(c);
    }

    void PruneDeadEnds()
    {
        bool deadEndFound = true;
        while (deadEndFound)
        {
            deadEndFound = false;
            List<Room> roomsToRemove = new List<Room>();

            foreach (var room in spawnedPieces)
            {
                if (room.type == RoomType.Corridor && room.childRooms.Count == 0)
                    roomsToRemove.Add(room);
            }

            if (roomsToRemove.Count > 0)
            {
                deadEndFound = true;
                foreach (var deadEnd in roomsToRemove)
                {
                    Room parent = spawnedPieces.FirstOrDefault(p => p.childRooms.Contains(deadEnd));
                    if (parent != null)
                    {
                        parent.childRooms.Remove(deadEnd);
                        ReopenParentConnector(parent, deadEnd);
                    }
                    spawnedPieces.Remove(deadEnd);
                    DestroyImmediate(deadEnd.gameObject);
                }
            }
        }
    }

    void ReopenParentConnector(Room parent, Room deadEnd)
    {
        foreach (Transform parentChildTransform in parent.transform)
        {
            if (parent.connectors.Contains(parentChildTransform)) continue;
            foreach (Transform deadEndChild in deadEnd.transform)
            {
                if (Vector3.Distance(parentChildTransform.position, deadEndChild.position) < 0.1f)
                {
                    parent.connectors.Add(parentChildTransform);
                    return;
                }
            }
        }
    }

    void SealDungeon()
    {
        GameObject wallPrefab = factory.GetWall();
        if (wallPrefab == null) return;

        foreach (Room room in spawnedPieces)
        {
            List<Transform> openConnectors = new List<Transform>(room.connectors);
            foreach (Transform connector in openConnectors)
            {
                if (connector == null) continue;
                Instantiate(wallPrefab, connector.position, connector.rotation, room.transform);
                room.DisableConnector(connector);
            }
        }
    }

    bool CheckOverlap(Room room)
    {
        Physics.SyncTransforms();
        foreach (BoxCollider box in room.roomBounds)
        {
            if (box == null) continue;
            Transform t = box.transform;
            Vector3 halfExtents = Vector3.Scale(box.size, t.lossyScale) * 0.48f;
            Vector3 center = t.TransformPoint(box.center);
            Collider[] hits = Physics.OverlapBox(center, halfExtents, t.rotation, roomLayer);
            foreach (var hit in hits)
            {
                if (hit.transform.root != room.transform.root) return true;
            }
        }
        return false;
    }

    void ResetLevel()
    {
        foreach (var p in spawnedPieces) if (p != null) DestroyImmediate(p.gameObject);
        spawnedPieces.Clear();
        pendingConnectors.Clear();
        roomCount = 0;
        totalEnemies = 0;
    }
}