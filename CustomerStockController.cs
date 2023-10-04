using Cysharp.Threading.Tasks;
using MoralisUnity;
using MoralisUnity.Platform.Queries;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerStockController
{
    public static async UniTask<Dictionary<CustomerStock, PlantObject>> GetStocks(string _userAddress)
    {
        MoralisQuery<CustomerStock> query = await Moralis.Query<CustomerStock>();

        query = query.WhereEqualTo("customerAddress", _userAddress);
        IEnumerable<CustomerStock> result = await query.FindAsync();
        var list = new Dictionary<CustomerStock, PlantObject>();

        foreach (CustomerStock stock in result)
        {
            var plant = ScriptableUtils.GetPlant(stock.plantName);
            list.Add(stock, plant);
        }

        return list;
    }

    public static async UniTask<Dictionary<CustomerStock, PlantObject>> GetMyStocks()
    {
        UserModel user = await UserController.GetMyUser();
        return await GetStocks(user.ethAddress);
    }
}
