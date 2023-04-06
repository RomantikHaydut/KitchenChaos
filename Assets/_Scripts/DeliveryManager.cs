using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DeliveryManager : NetworkBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeFailed;
    public event EventHandler<RecipeSO> OnRecipeSuccess;


    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeSOListSO recipeListSO;

    [SerializeField] private List<RecipeSO> waitingRecipeSOList = new List<RecipeSO>();

    private float spawnRecipeTimer = 4f;

    private float spawnRecipeTimerMax = 4f;

    private int waitingRecipeMax = 4;

    private int successfullDeliverAmount;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        spawnRecipeTimer = spawnRecipeTimerMax;
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        spawnRecipeTimer -= Time.deltaTime;

        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (GameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count <= waitingRecipeMax)
            {
                int recipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.RecipeSOList.Count);

                SpawnNewWaitingRecipeClientRpc(recipeSOIndex);

            }
        }
    }

    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int recipeSOIndex)
    {
        RecipeSO recipeSO = recipeListSO.RecipeSOList[recipeSOIndex];

        waitingRecipeSOList.Add(recipeSO);

        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }


    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenobjectSoList().Count)
            {
                // Has same number of ingredients.
                bool plateContentsMatchesRecipe = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    // Cycling throgh all ingredients in the recipe
                    bool ingredientFound = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenobjectSoList())
                    {
                        // Cycling throgh all ingredients in the Plate
                        if (plateKitchenObjectSO == recipeKitchenObjectSO)
                        {
                            ingredientFound = true;
                            break;
                        }
                    }
                    //
                    if (!ingredientFound)
                    {
                        // This recipe ingredient was not found on the plate
                        plateContentsMatchesRecipe = false;
                    }
                }

                if (plateContentsMatchesRecipe)
                {
                    // Player delivered Correct recipe.
                    //waitingRecipeSOList[i].scoreFactor
                    DeliverCorrectRecipeServerRpc(i);
                    return;
                }
            }
        }

        // No Matches found.
        // Player did not correct delivery.
        DeliverIncorrectRecipeServerRpc();


    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverIncorrectRecipeServerRpc()
    {
        DeliverIncorrectRecipeClientRpc();
    }

    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc()
    {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int waitingRecipeSOListIndex)
    {
        DeliverCorrectRecipeClientRpc(waitingRecipeSOListIndex);
    }

    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int waitingRecipeSOListIndex)
    {
        OnRecipeSuccess?.Invoke(this, waitingRecipeSOList[waitingRecipeSOListIndex]);
        waitingRecipeSOList.RemoveAt(waitingRecipeSOListIndex);
        successfullDeliverAmount++;
        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
    }



    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    public int SuccessfullDeliverAmount()
    {
        return successfullDeliverAmount;
    }

}
