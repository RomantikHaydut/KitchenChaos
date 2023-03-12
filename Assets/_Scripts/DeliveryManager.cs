using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeFailed;
    public event EventHandler<RecipeSO> OnRecipeSuccess;


    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeSOListSO recipeListSO;

    [SerializeField] private List<RecipeSO> waitingRecipeSOList = new List<RecipeSO>();

    private float spawnRecipeTimer = 0f;

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
        spawnRecipeTimer -= Time.deltaTime;

        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (GameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count <= waitingRecipeMax)
            {
                RecipeSO waitinRecipeSO = recipeListSO.RecipeSOList[UnityEngine.Random.Range(0, recipeListSO.RecipeSOList.Count)];
                waitingRecipeSOList.Add(waitinRecipeSO);

                OnRecipeSpawned?.Invoke(this,EventArgs.Empty);
            }
        }
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
                foreach (KitchenObjectSO  recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
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
                    OnRecipeSuccess?.Invoke(this, waitingRecipeSOList[i]);
                    waitingRecipeSOList.RemoveAt(i);
                    successfullDeliverAmount++;
                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
        }

        // No Matches found.
        // Player did not correct delivery.
        Debug.Log("Player did not correct delivery");
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
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
