namespace Valour.Net.PlanetEconomy;

public static class PlanetEconomyExtensionMethods
{
    /// <summary>
    /// Sends <paramref name="amount"/> to the currency account with id <paramref name="toId"/> 
    /// </summary>
    /// <param name="amount">The amount to send</param>
    /// <param name="toId">The new prefix</param>
    /// <param name="extra">Extra data</param>
    /// <param name="force">Whether or not to force this transaction even if the Fromid account lacks enough currency.</param>
  //  public static async Task<PlanetTransaction> Send(this PlanetCurrencyAccount account, long toId, decimal amount, string extradata, bool force = false)
  //  {
      //  var request = new PlanetTransactionRequest() {
      //      Id = 0,
      //      PlanetId = account.PlanetId,
      //      FromId = account.Id,
      //      ToId = toId,
     //       Amount = amount,
     //       Data = extradata,
   //         Force = force
   //     };
//
   //     return await ValourNetClient.SendTransactionRequestAsync(request);
    //}

    /// <summary>
    /// Sends <paramref name="amount"/> to the currency account with id <paramref name="toId"/> .
    /// Does not wait for transaction to be executed
    /// </summary>
    /// <param name="amount">The amount to send</param>
    /// <param name="toId">The new prefix</param>
    /// <param name="extra">Extra data</param>
    /// <param name="force">Whether or not to force this transaction even if the Fromid account lacks enough currency.</param>
   // public static async Task SendWithoutWaiting(this PlanetCurrencyAccount account, long toId, decimal amount, string extradata, bool force = false)
  //  {
     //   var request = new PlanetTransactionRequest() {
     //       Id = 0,
     //       PlanetId = account.PlanetId,
     //       FromId = account.Id,
     //       ToId = toId,
    //        Amount = amount,
     //       Data = extradata,
    //       Force = force
     //   };
        //
    //    await request.SendAsync();
    //}
}