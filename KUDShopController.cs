using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MoralisUnity;
using MoralisUnity.Platform.Queries;
using UnityEngine;

public class KUDShopController
{
    public static async UniTask<Dictionary<KUDShop, PlantObject>> GetStocks(string _userAddress)
    {
        MoralisQuery<KUDShop> farmer = await Moralis.Query<KUDShop>();

        farmer = farmer.WhereEqualTo("kudAddress", _userAddress);
        IEnumerable<KUDShop> result = await farmer.FindAsync();
        var list = new Dictionary<KUDShop, PlantObject>();

        foreach (KUDShop stock in result)
        {
            var plant = ScriptableUtils.GetPlant(stock.plantName) ;
            list.Add(stock, plant);
        }

        return list;
    }

    public static async UniTask<Dictionary<KUDShop, PlantObject>> GetMyStocks()
    {
        UserModel user = await UserController.GetMyUser();
        return await GetStocks(user.ethAddress);
    }

    public static async UniTask<Dictionary<KUDShop, PlantObject>> GetShopItems(bool isKUD)
    {
        var moralisUser = await Moralis.GetUserAsync();
        MoralisQuery<KUDShop> query = await Moralis.Query<KUDShop>();

        if(isKUD)
            query = query.WhereEqualTo("kudAddress", moralisUser.ethAddress);

        var result = await query.FindAsync();
        var list = new Dictionary<KUDShop, PlantObject>();

        foreach (KUDShop item in result)
        {
            var plant = ScriptableUtils.GetPlant(item.plantName);
            list.Add(item, plant);
        }

        return list;
    }

    public static async void CustomerBuy(KUDShop _shopItem, int _amount)
    {
        var customerNewStock = Moralis.Client.Create<CustomerStock>();
        var user = await Moralis.GetUserAsync();

        MoralisQuery<KUDShop> query = await Moralis.Query<KUDShop>();
        query = query.WhereEqualTo("stockId", _shopItem.stockId);
        KUDShop itemKUD = await query.FirstOrDefaultAsync();

        itemKUD.quantity               -= _amount;
        customerNewStock.stockId        = _shopItem.stockId;
        customerNewStock.boughtTime     = DateTime.Now.ToUniversalTime();
        customerNewStock.condition      = _shopItem.condition;
        customerNewStock.expirationDate = _shopItem.expirationDate;
        customerNewStock.farmerAddress  = _shopItem.farmerAddress;
        customerNewStock.customerAddress= _shopItem.customerAddress;
        customerNewStock.kudAddress     = _shopItem.kudAddress;
        customerNewStock.harvestedDate  = _shopItem.harvestedDate;
        customerNewStock.quantity       = _amount;
        customerNewStock.plantPrice     = _shopItem.plantPrice;
        customerNewStock.plantName      = _shopItem.plantName;
        customerNewStock.status         = 0;


        await customerNewStock.SaveAsync();

        if (itemKUD.quantity > 0)
        {
            var success = await itemKUD.SaveAsync();
            if (success)
            {
                Debug.Log("FarmerShop has been decreased, updated to DB");
            }
            else
            {
                Debug.Log("Failed to update to FarmerShop DB");
            }
        }
        else
        {
            await itemKUD.DeleteAsync();
            Debug.Log("Shop item reached 0, deleted from Database");
        }
    }
}