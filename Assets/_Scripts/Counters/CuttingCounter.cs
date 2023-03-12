using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter , IHasProgress
{
    public static event EventHandler OnAnyCut;

    new public static void ResetStaticData()
    {
        OnAnyCut = null;
    }

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public event EventHandler OnCut;

    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    private int cuttingProgress = 0;

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // There is no kitchen object here.
            if (player.HasKitchenObject())
            {
                // Player is carrying something.
                if (HasRecipeForInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    // Player carries something can cut.
                    player.GetKitchenObject().SetKithcenObjectParent(this);
                    cuttingProgress = 0;

                    CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { 
                        progressNormalized = (float)cuttingProgress/ cuttingRecipeSO.cuttingProgressMax,
                    });


                }
            }
            else
            {
                // Player has not carrying anything.

            }
        }
        else
        {
            // There is a kitchen object.
            if (player.HasKitchenObject())
            {
                // Player is carrying something.
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    // Player is holding a plate.
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                    }

                }
            }
            else
            {
                // Player isnt carrying anything.
                GetKitchenObject().SetKithcenObjectParent(player);
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasRecipeForInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            // There is a kitchen object cut it.
            cuttingProgress++;

            OnCut?.Invoke(this, EventArgs.Empty);   
            OnAnyCut?.Invoke(this, EventArgs.Empty);

            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax,
            });

            if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
            {
                KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
                GetKitchenObject().DestroySelf();

                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
            }

        }
    }

    private bool HasRecipeForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        return cuttingRecipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);

        if (cuttingRecipeSO == null)
        {
            return null;
        }
        else
        {
            return cuttingRecipeSO.output;
        }

    }

    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.input == inputKitchenObjectSO)
            {
                return cuttingRecipeSO;
            }
        }

        return null;

    }
}
