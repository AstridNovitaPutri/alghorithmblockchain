using System;
using System.Collections.Generic;
using MoralisUnity;
using MoralisUnity.Platform.Objects;
using MoralisUnity.Web3Api.Models;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using UnityEngine;
using UnityEngine.UI;

public class SmartContract : MonoBehaviour
{

  public const string ContractAddress = "0x1E40242d88082800A5230a5DA2223074CE5d5f36";
  public const string functionName = "getGreeting";

  public Button transferButton;
  public Button callSmartContractButton;
  public Button smartContractTransactionButton;

  private async void Start()
  {
    List<ChainEntry> chains = Moralis.SupportedChains;
    foreach (var chain in chains)
    {
      Debug.Log("Chain ID : " + chain.ChainId + ", Decimals : " + chain.Decimals + ", EnumValue : " + chain.EnumValue + ", Name : " + chain.Name + ", Symbol : " + chain.Symbol);
    }
    //transferButton.onClick.AddListener(SendRawETH);
    callSmartContractButton.onClick.AddListener(RunSmartContract);
    smartContractTransactionButton.onClick.AddListener(SmartContractTransaction);
        DebugBalance();
  }

  public async void DebugBalance()
  {
    var user = await UserController.GetMyUser();
    var address = user.ethAddress;
    NativeBalance balancepolygon = await Moralis.Web3Api.Account.GetNativeBalance(address.ToLower(), ChainList.mumbai);
    NativeBalance balancemumbai = await Moralis.Web3Api.Account.GetNativeBalance(address.ToLower(), ChainList.mumbai);
    Debug.Log($"GetNativeBalance Balance: {balancepolygon.Balance}");
    Debug.Log($"GetNativeBalance Balance: {balancemumbai.Balance}");
  }

  public static async void SendRawETH(MoralisUser _from, string _toAddress, float _value, Action _callback){
    MoralisUser user = await Moralis.GetUserAsync();
    float transferAmount = _value / GlobalConst.MATIC_TO_IDR;
    string fromAddress = _from.authData["moralisEth"]["id"].ToString();
    string toAddress = _toAddress;
    TransactionInput txnRequest = new TransactionInput(){
      Data = String.Empty,
      From = fromAddress,
      To = toAddress,
      Value = new HexBigInteger(UnitConversion.Convert.ToWei(transferAmount))
    };
    try{
      //string txnHash = await Moralis.Web3Client.Eth.TransactionManager.SendTransactionAsync(txnRequest);
      bool txnComplete = Moralis.Web3Client.Eth.TransactionManager.SendTransactionAsync(txnRequest).IsCompletedSuccessfully;
      if(txnComplete){
        _callback();
      }
      //Debug.Log($"Transfered {transferAmount} ETH from {fromAddress} to {toAddress}.  TxnHash: {txnHash}");
    }
    catch (Exception exp){
      Debug.Log($"Transfer of {transferAmount} ETH from {fromAddress} to {toAddress} failed! with error {exp}");
    }
  }

  public static object[] GetAbiObject()
  {
    object[] newAbi = new object[3];

    object[] cInputParams = new object[0];
    newAbi[0] = new { inputs = cInputParams, name = "", stateMutability = "nonpayable", type = "constructor" };

    object[] gInputParams = new object[0];
    object[] gOutputParams = new object[1];
    gOutputParams[0] = new { internalType = "string", name = "", type = "string" };
    newAbi[1] = new { inputs = gInputParams, outputs = gOutputParams, name = "getGreeting", stateMutability = "view", type = "function" };

    object[] sInputParams = new object[1];
    sInputParams[0] = new { internalType = "string", name = "greeting", type = "string" };
    object[] sOutputParams = null;
    newAbi[2] = new { inputs = sInputParams, outputs = sOutputParams, name = "setGreeting", stateMutability = "nonpayable", type = "function" };

    return newAbi;
  }

  public async void RunSmartContract()
  {
    // Function ABI input parameters
    object[] inputParams = new object[1];
    inputParams[0] = new { internalType = "uint256", name = "id", type = "uint256" };
    // Function ABI Output parameters
    object[] outputParams = new object[1];
    outputParams[0] = new { internalType = "string", name = "", type = "string" };
    // Function ABI
    object[] abi = new object[1];
    abi[0] = new { inputs = inputParams, name = "uri", outputs = outputParams, stateMutability = "view", type = "function" };

    // Define request object
    RunContractDto rcd = new RunContractDto()
    {
      Abi = abi,
      Params = new { id = "15310200874782" }
    };

    //var resp = await Moralis.Web3Api.Native.RunContractFunction("0x698d7D745B7F5d8EF4fdB59CeB660050b3599AC3", "uri", rcd, ChainList.mumbai);

    //Debug.Log($"Contract Function returned: {resp}");
  }

  public async void SmartContractTransaction()
  {
    // Function ABI input parameters
    object[] inputParams = new object[1];
    inputParams[0] = new { internalType = "uint256", name = "id", type = "uint256" };
    // Function ABI Output parameters
    object[] outputParams = new object[1];
    outputParams[0] = new { internalType = "string", name = "", type = "string" };
    // Function ABI
    object[] abi = new object[1];
    abi[0] = new { inputs = inputParams, name = "uri", outputs = outputParams, stateMutability = "view", type = "function" };

    // Define request object
    RunContractDto rcd = new RunContractDto()
    {
      Abi = abi,
      Params = new { id = "15310200874782" }
    };

    //string resp = await Moralis.Web3Api.Native.RunContractFunction("0xCcC1c23f5bA573d6650633e4EdB7e6EE4eFA3879", "uri", rcd, ChainList.mumbai);

    //Debug.Log($"Contract Function returned: {resp}");
  }

  public async void ExecuteContractFunction()
  {
    string ABI = "[{\"inputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"inputs\":[],\"name\":\"getGreeting\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"greeting\",\"type\":\"string\"}],\"name\":\"setGreeting\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"greeting\",\"type\":\"string\"}],\"name\":\"setGreetingSafe\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";
    string ContractAddress = "0x1E40242d88082800A5230a5DA2223074CE5d5f36";
    string FunctioName = "getGreeting";
    object[] inputParams = GetAbiObject();
    HexBigInteger value = new HexBigInteger("0x0");
    HexBigInteger gas = new HexBigInteger("800000");
    HexBigInteger gasprice = new HexBigInteger("230000");
    try
    {
      string result = await Moralis.ExecuteContractFunction(
        contractAddress: ContractAddress,
        abi: ABI,
        functionName: FunctioName,
        args: inputParams,
        value: value,
        gas: gas,
        gasPrice: gasprice);
      Debug.Log("Txhash :" + result);
    }
    catch (Exception error)
    {
      Debug.Log("Error :" + error);
    }
  }

}