using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MoralisUnity;
using MoralisUnity.Platform.Queries;
using UnityEngine;

public class KUDStockController
{
    public static async UniTask<Dictionary<KUDStock, PlantObject>> GetStocks(string _userAddress)
    {
        MoralisQuery<KUDStock> farmer = await Moralis.Query<KUDStock>();

        farmer = farmer.WhereEqualTo("kudAddress", _userAddress);
        IEnumerable<KUDStock> result = await farmer.FindAsync();
        var list = new Dictionary<KUDStock, PlantObject>();

        foreach (KUDStock stock in result){
          var plant = ScriptableUtils.GetPlant(stock.plantName) ;
          list.Add(stock, plant);
        }

        return list;
    }

    public static async UniTask<Dictionary<KUDStock, PlantObject>> GetMyStocks()
    {
        UserModel user = await UserController.GetMyUser();
        return await GetStocks(user.ethAddress);
    }

    public static async void SellStock(KUDStock _stock, int _amount, int _price)
    {
        var stockTobeSold = Moralis.Client.Create<KUDShop>();
        MoralisQuery<KUDStock> stockQ = await Moralis.Query<KUDStock>();
        IEnumerable<KUDStock> result = await stockQ.FindAsync();
        KUDStock selectedStock = null;

        foreach (KUDStock stock in result)
        {
            if(stock.kudAddress == _stock.kudAddress && stock.plantName == _stock.plantName)
            {
                selectedStock = stock;
            }
        }

        if(selectedStock != null && selectedStock.harvestedDate == _stock.harvestedDate)
        {
            selectedStock.quantity -= _amount;
            stockTobeSold.stockId = _stock.stockId;
            stockTobeSold.farmerAddress = _stock.farmerAddress;
            stockTobeSold.kudAddress = _stock.kudAddress;
            stockTobeSold.plantName = _stock.plantName;
            stockTobeSold.quantity = _amount;
            stockTobeSold.harvestedDate = _stock.harvestedDate;
            stockTobeSold.expirationDate = _stock.expirationDate;
            stockTobeSold.condition = _stock.condition;
            stockTobeSold.plantPrice = _price;

            await stockTobeSold.SaveAsync();

            if(selectedStock.quantity > 0)
            {
                var successz = await selectedStock.SaveAsync();

                if(successz)
                {
                    Debug.Log("FarmerStock has been decreased, updated to DB");
                }
                else
                {
                    Debug.Log("Failed to update to FarmerStock DB");
                }
            }
            else
            {
                await selectedStock.DeleteAsync();
                Debug.Log("Stock reached 0, deleted from Database");
            }
        }
    }
}