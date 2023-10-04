using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using MoralisUnity;
using MoralisUnity.Platform.Queries;
using UnityEngine;

public class FarmerShopController {

  public static async void TestCreate(FarmerShop _shopItem, int _amount){
    var kudStock = Moralis.Client.Create<KUDStock>();
    await kudStock.SaveAsync();
  }

  public static async void TestCreateLala(FarmerStock _shopItem){
    var kudStock = Moralis.Client.Create<FarmerStock>();
    kudStock.condition = _shopItem.condition;
    kudStock.expirationDate = _shopItem.expirationDate;
    kudStock.farmerAddress = _shopItem.farmerAddress;
    kudStock.plantName = _shopItem.plantName;
    kudStock.totalHarvested = _shopItem.totalHarvested;
    kudStock.harvestedDate = _shopItem.harvestedDate;
    Debug.Log("Test Lalal Callaed");
    await kudStock.SaveAsync();
  }

    public static async void TestCreateLili(FarmerShop _shopItem, int _amount){
    var kudStock = Moralis.Client.Create<FarmerShop>();
    await kudStock.SaveAsync();
  }

  public static async UniTask<Dictionary<FarmerShop, PlantObject>> GetShopItems(bool isFarmer){
        var moralisUser = await Moralis.GetUserAsync();
        MoralisQuery<FarmerShop> farmer = await Moralis.Query<FarmerShop>();

        if(isFarmer)
            farmer = farmer.WhereEqualTo("farmerAddress", moralisUser.ethAddress);

        IEnumerable<FarmerShop> result = await farmer.FindAsync();
        var list = new Dictionary<FarmerShop, PlantObject>();

        foreach (FarmerShop item in result){
          var plant = ScriptableUtils.GetPlant(item.plantName) ;
          list.Add(item, plant);
        }
        return list;
  }

  // public static async void KUDBuyItem(KUDStocks _kudStock){
  //   var kudStock = Moralis.Client.Create<KUDStockModel>();
  //   var user = await Moralis.GetUserAsync();
  //   MoralisQuery<FarmerShop> itemQ = await Moralis.Query<FarmerShop>();
  //   itemQ = itemQ.WhereEqualTo("stockId", _kudStock.stockId);
  //   IEnumerable<FarmerShop> result = await itemQ.FindAsync();
  //   foreach (FarmerShop stock in result){
  //     await stock.DeleteAsync();
  //   }
  //   kudStock.stockId = _kudStock.stockId;
  //   kudStock.farmerOwner = _kudStock.owner;
  //   kudStock.plantName = _kudStock.plant.plantName;
  //   kudStock.kudAddress = user.ethAddress;
  //   kudStock.quantity = _kudStock.amount;
  //   kudStock.harvestedDate = _kudStock.harvestedDate;
  //   kudStock.expirationDate = _kudStock.expirationDate;
  //   kudStock.condition = _kudStock.condition;
  //   kudStock.plantPrice = _kudStock.boughtPrice;
  //   kudStock.boughtTime = DateTime.Now.Date;
  // }

  public static async void KUDBuyItem(FarmerShop _shopItem, int _amount){
    var transaction = Moralis.Client.Create<KUDStock>();
    var user = await Moralis.GetUserAsync();
    MoralisQuery<FarmerShop> itemQ = await Moralis.Query<FarmerShop>();
    itemQ = itemQ.WhereEqualTo("stockId", _shopItem.stockId);
    FarmerShop item = await itemQ.FirstOrDefaultAsync();
    item.quantity -= _amount;
    transaction.stockId = _shopItem.stockId;
    transaction.boughtTime = DateTime.Now.ToUniversalTime();
    transaction.condition = _shopItem.condition;
    transaction.expirationDate = _shopItem.expirationDate;
    transaction.farmerAddress = _shopItem.farmerAddress;
    transaction.harvestedDate = _shopItem.harvestedDate;
    transaction.quantity = _amount;
    transaction.plantPrice = _shopItem.price;
    transaction.kudAddress = user.ethAddress;
    transaction.plantName = _shopItem.plantName;
    await transaction.SaveAsync();
    if(item.quantity > 0){
      var successz = await item.SaveAsync();
      if(successz){
        Debug.Log("FarmerShop has been decreased, updated to DB");
      }else{
        Debug.Log("Failed to update to FarmerShop DB");
      }
    }else{
      await item.DeleteAsync();
      Debug.Log("Shop item reached 0, deleted from Database");
    }  
  }

    public static async void KUDBuyItemBug(FarmerShop _shopItem, int _amount){
    var kudStock = Moralis.Client.Create<KUDStock>();
    var user = await Moralis.GetUserAsync();
    MoralisQuery<FarmerShop> itemQ = await Moralis.Query<FarmerShop>();
    itemQ = itemQ.WhereEqualTo("stockId", _shopItem.stockId);
    FarmerShop item = await itemQ.FirstOrDefaultAsync();
    item.quantity -= _amount;
    kudStock.stockId = _shopItem.stockId;
    kudStock.boughtTime = DateTime.Now.ToUniversalTime();
    kudStock.farmerAddress = _shopItem.farmerAddress;
    kudStock.kudAddress = user.ethAddress;
    kudStock.plantName = _shopItem.plantName;
    kudStock.quantity = _amount;
    kudStock.harvestedDate = _shopItem.harvestedDate;
    kudStock.expirationDate = _shopItem.expirationDate;
    kudStock.condition = _shopItem.condition;
    kudStock.plantPrice = _shopItem.price;
    Debug.Log("ISINYA stockId: " + kudStock.stockId);
    Debug.Log("ISINYA boughtTime: " + kudStock.boughtTime);
    Debug.Log("ISINYA farmerOwner: " + kudStock.farmerAddress);
    Debug.Log("ISINYA kudAddress: " + kudStock.kudAddress);
    Debug.Log("ISINYA plantName: " + kudStock.plantName);
    Debug.Log("ISINYA quantity: " + kudStock.quantity);
    Debug.Log("ISINYA harvestedDate: " + kudStock.harvestedDate);
    Debug.Log("ISINYA expirationDate: " + kudStock.expirationDate);
    Debug.Log("ISINYA condition: " + kudStock.condition);
    Debug.Log("ISINYA plantPrice: " + kudStock.plantPrice);
    await kudStock.SaveAsync();
    if(item.quantity > 0){
      var successz = await item.SaveAsync();
      if(successz){
        Debug.Log("FarmerShop has been decreased, updated to DB");
      }else{
        Debug.Log("Failed to update to FarmerShop DB");
      }
    }else{
      await item.DeleteAsync();
      Debug.Log("Shop item reached 0, deleted from Database");
    }    
  }

  //   public static async void SellStock(ShopInventory _stock){
  //   var stockTobeSold = Moralis.Client.Create<FarmerShopModel>();
  //   var user = await Moralis.GetUserAsync();
  //   MoralisQuery<FarmerStock> stockQ = await Moralis.Query<FarmerStock>();
  //   stockQ = stockQ.WhereEqualTo("plantName", _stock.plant.plantName).WhereEqualTo("farmerAddress", _stock.owner);
  //   IEnumerable<FarmerStock> result = await stockQ.FindAsync();
  //   foreach (FarmerStock stock in result){
  //     if(stock.harvestedDate == _stock.harvestedDate){
  //       stock.totalHarvested -= _stock.quantity;
  //       var success = await stock.SaveAsync();
  //     }
  //   }
  //   stockTobeSold.stockId = DateTime.Now.ToString().Replace("/","").Replace(":","").Replace(" ","") + DateTime.Now.Millisecond;
  //   stockTobeSold.farmerAddress = user.ethAddress;
  //   stockTobeSold.plantName = _stock.plant.plantName;
  //   stockTobeSold.quantity = _stock.quantity;
  //   stockTobeSold.harvestedDate = _stock.harvestedDate;
  //   stockTobeSold.expirationDate = _stock.expirationDate;
  //   stockTobeSold.condition = _stock.condition;
  //   stockTobeSold.price = _stock.price;
  //   await stockTobeSold.SaveAsync();
  // }


}