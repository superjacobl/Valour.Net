using Valour.Sdk.Models.Economy;

namespace Valour.Net.PlanetEconomy;

public static class PlanetEconomyExtensionMethods
{
    // TODO
    // make this work with oauth tokens!
    public static async Task<TaskResult<Transaction>> SendAsync(this Transaction transaction, string oauthtoken)
    {
        Node node;

        //if (transaction is IPlanetItem planetItem)
            //node = await NodeManager.GetNodeForPlanetAsync(planetItem.PlanetId);
        //else
            node = await NodeManager.GetNodeForPlanetAsync(transaction.PlanetId);

        if (oauthtoken is not null)
        {
            var http = new HttpClient()
            {
                BaseAddress = new Uri("https://app.valour.gg")
            };

            http.DefaultRequestHeaders.Add("Authorization", oauthtoken);
            // Set node to primary node for main http client
            http.DefaultRequestHeaders.Add("X-Server-Select", (await NodeManager.GetNodeForPlanetAsync(transaction.PlanetId)).Name);

            return await ValourClient.PostAsyncWithResponse<Transaction>("api/eco/transactions", transaction, http);
        }

        var result = await node.PostAsyncWithResponse<Transaction>("api/eco/transactions", transaction);
        return result;
    }

    /// <summary>
    /// Sends <paramref name="amount"/> to the currency account with id <paramref name="toId"/> 
    /// </summary>
    /// <param name="amount">The amount to send</param>
    /// <param name="toId">The new prefix</param>
    /// <param name="extra">Extra data</param>
    /// <param name="force">Whether or not to force this transaction even if the Fromid account lacks enough currency.</param>
    public static async Task<Transaction> Send(this EcoAccount sender, EcoAccount receiver, decimal amount, string description, string extradata, bool force = false)
    {
        var request = new Transaction() {
            PlanetId = sender.PlanetId,
            UserFromId = sender.UserId,
            AccountFromId = sender.Id,
            UserToId = receiver.UserId,
            AccountToId = receiver.Id,
            Amount = amount,
            Description = description,
            Data = extradata,
            Fingerprint = Guid.NewGuid().ToString()
        };

        return await ValourNetClient.SendTransactionRequestAsync(request);
    }

    /// <summary>
    /// Sends <paramref name="amount"/> to the currency account with id <paramref name="toId"/> .
    /// Does not wait for transaction to be executed
    /// </summary>
    /// <param name="amount">The amount to send</param>
    /// <param name="toId">The new prefix</param>
    /// <param name="extra">Extra data</param>
    /// <param name="force">Whether or not to force this transaction even if the Fromid account lacks enough currency.</param>
    public static async Task SendWithoutWaiting(this EcoAccount sender, EcoAccount receiver, decimal amount, string description, string extradata, bool force = false)
    {
        var request = new Transaction()
        {
            PlanetId = sender.PlanetId,
            UserFromId = sender.UserId,
            AccountFromId = sender.Id,
            UserToId = receiver.UserId,
            AccountToId = receiver.Id,
            Amount = amount,
            Description = description,
            Data = extradata,
            Fingerprint = Guid.NewGuid().ToString()
        };

        //await request.SendAsync();
    }
}