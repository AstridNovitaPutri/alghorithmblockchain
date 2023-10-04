using Cysharp.Threading.Tasks;
using MoralisUnity;
using MoralisUnity.Platform.Queries;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FarmerStockController
{
    public static async UniTask<Dictionary<FarmerStock, PlantObject>> GetStocks(string _userAddress)
    {
        MoralisQuery<FarmerStock> farmer = await Moralis.Query<FarmerStock>();

        farmer = farmer.WhereEqualTo("farmerAddress", _userAddress);
        IEnumerable<FarmerStock> result = await farmer.FindAsync();
        var list = new Dictionary<FarmerStock, PlantObject>();

        foreach (FarmerStock stock in result)
        {
            var plant = ScriptableUtils.GetPlant(stock.plantName);
            list.Add(stock, plant);
        }
        return list;
    }

    public static async UniTask<Dictionary<FarmerStock, PlantObject>> GetMyStocks()
    {
        UserModel user = await UserController.GetMyUser();
        return await GetStocks(user.ethAddress);
    }

    public static async void AddFarmerStock(FarmerStock _stock, int _amount)
    {
        MoralisQuery<FarmerStock> stockQ = await Moralis.Query<FarmerStock>();
        IEnumerable<FarmerStock> result = await stockQ.FindAsync();
        FarmerStock selectedStock = null;
        foreach (FarmerStock stock in result)
        {
            if (stock.farmerAddress == _stock.farmerAddress && stock.plantName == _stock.plantName)
            {
                selectedStock = stock;
            }
        }

        if (selectedStock != null && selectedStock.harvestedDate == DateTime.Now.ToUniversalTime().Date)
        {
            selectedStock.totalHarvested += _amount;
            Debug.Log("Total of " + selectedStock.plantName + " is " + selectedStock.totalHarvested);
            if (selectedStock.totalHarvested > 0)
            {
                var successz = await selectedStock.SaveAsync();
                if (successz)
                {
                    Debug.Log("FarmerStock updated to DB");
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
            return;
        }

        var newStock = Moralis.Client.Create<FarmerStock>();
        var user = await Moralis.GetUserAsync();
        newStock = _stock;
        newStock.totalHarvested += _amount;
        Debug.Log("New Stock " + newStock.condition + newStock.harvestedDate + newStock.expirationDate + newStock.plantName + newStock.farmerAddress);
        var success = await newStock.SaveAsync();
        if (success)
        {
            Debug.Log("FarmerStock created to DB");
        }
        else
        {
            Debug.Log("Failed to create to FarmerStock DB");
        }
    }

    public static async void TestFarmerStock()
    {
        MoralisQuery<FarmerStock> stockQ = await Moralis.Query<FarmerStock>();
        IEnumerable<FarmerStock> result = await stockQ.FindAsync();
        var user = await UserController.GetMyUser();
        foreach (FarmerStock stock in result)
        {
            if (stock.farmerAddress == user.ethAddress)
            {
                Debug.Log("True!");
            }
            else
            {
                Debug.Log("False");
            }
        }
    }

    public static async void SellStock(FarmerStock _stock, int _amount, int _price)
    {
        var stockTobeSold = Moralis.Client.Create<FarmerShop>();
        MoralisQuery<FarmerStock> stockQ = await Moralis.Query<FarmerStock>();
        IEnumerable<FarmerStock> result = await stockQ.FindAsync();
        FarmerStock selectedStock = null;
        foreach (FarmerStock stock in result)
        {
            if (stock.farmerAddress == _stock.farmerAddress && stock.objectId == _stock.objectId)
            {
                selectedStock = stock;
            }
        }
        if (selectedStock != null && selectedStock.harvestedDate == _stock.harvestedDate)
        {
            selectedStock.totalHarvested -= _amount;
            stockTobeSold.stockId = DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") + DateTime.Now.Millisecond;
            stockTobeSold.farmerAddress = _stock.farmerAddress;
            stockTobeSold.plantName = _stock.plantName;
            stockTobeSold.quantity = _amount;
            stockTobeSold.harvestedDate = _stock.harvestedDate;
            stockTobeSold.expirationDate = _stock.expirationDate;
            stockTobeSold.condition = _stock.condition;
            stockTobeSold.price = _price;
            await stockTobeSold.SaveAsync();
            if (selectedStock.totalHarvested > 0)
            {
                var successz = await selectedStock.SaveAsync();
                if (successz)
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

    public static async void DeleteStock()
    {
        MoralisQuery<FarmerStock> stockQ = await Moralis.Query<FarmerStock>();
    }

}