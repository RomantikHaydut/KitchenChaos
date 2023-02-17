using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter 
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // There is no kitchen object here.
            if (player.HasKitchenObject())
            {
                // Player is carrying something.
                player.GetKitchenObject().SetKithcenObjectParent(this);
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
            }
            else
            {
                // Player isnt carrying anything.
                GetKitchenObject().SetKithcenObjectParent(player);
            }
        }
    }

}
