using System;
using System.Collections;
using System.Collections.Generic;
using QFSW.QC;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ekkam
{
    public class CheckpointManager : MonoBehaviour
    {
        public static CheckpointManager Instance;
        public CheckpointData currentCheckpointData;
        public Transform savedItemsParent;

        private ObjectiveManager objectiveManager;
        private Inventory inventory;

        private void Awake()
        {
            AssignReferences();
            
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                objectiveManager.InitializeFromCurrentIndex();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void AssignReferences()
        {
            objectiveManager = FindObjectOfType<ObjectiveManager>();
            inventory = FindObjectOfType<Inventory>();
        }

        [Command("save-checkpoint")]
        public void SaveTempCheckpoint()
        {
            SaveCheckpointData(Player.Instance.transform.position, Player.Instance.transform.eulerAngles);
        }
        
        public void SaveCheckpointData(Vector3 position, Vector3 rotation)
        {
            currentCheckpointData = new CheckpointData
            {
                position = position,
                rotation = rotation,
                health = Player.Instance.health,
                items = new List<GameObject>(),
                coins = Player.Instance.coins,
                tokens = Player.Instance.tokens,
                objectiveIndex = objectiveManager.currentObjectiveIndex
            };
            var savedItems = savedItemsParent.GetComponentsInChildren<Item>();
            foreach (Item item in savedItems)
            {
                Destroy(item.gameObject);
            }
            foreach (Item item in inventory.items)
            {
                var newItemGO = Instantiate(item.gameObject, savedItemsParent.transform);
                currentCheckpointData.items.Add(newItemGO);
            }
        }
        
        public void LoadCheckpointData()
        {
            StartCoroutine(LoadCheckpoint(currentCheckpointData));
        }
        
        IEnumerator LoadCheckpoint(CheckpointData checkpointData)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            print("Scene reload complete");
            AssignReferences();
            
            Player.Instance.transform.position = checkpointData.position;
            Player.Instance.transform.eulerAngles = checkpointData.rotation;
            Player.Instance.health = checkpointData.health;
            Player.Instance.coins = checkpointData.coins;
            Player.Instance.tokens = checkpointData.tokens;

            yield return new WaitForSeconds(0.5f);
            print("Restoring " + currentCheckpointData.items.Count + " items to inventory...");
            foreach (GameObject go in currentCheckpointData.items)
            {
                print("Restoring " + go.gameObject.name + " to inventory...");
                if (go.GetComponent<Interactable>() != null)
                {
                    go.GetComponent<Interactable>().Interact();
                    yield return new WaitForSeconds(0.5f);
                }
            }
            currentCheckpointData.items.Clear();
            
            yield return new WaitForSeconds(0.5f);
            foreach (var item in inventory.items)
            {
                var newItemGO = Instantiate(item.gameObject, savedItemsParent.transform);
                newItemGO.SetActive(false);
                currentCheckpointData.items.Add(newItemGO);
            }
            
            objectiveManager.currentObjectiveIndex = checkpointData.objectiveIndex;
            objectiveManager.InitializeFromCurrentIndex();
        }
    }
    
    [System.Serializable]
    public class CheckpointData
    {
        public Vector3 position;
        public Vector3 rotation;
        public int health;
        public List<GameObject> items;
        public int coins;
        public int tokens;
        public int objectiveIndex;
    }
}