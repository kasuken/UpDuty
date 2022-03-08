using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

string connectionString = "DefaultEndpointsProtocol=https;AccountName=updutydevstorage;AccountKey=MGj/u1qK1/Svbh7SpnLj1MBMvGfZTTZNB0an7W09qxDyqNbIPZ+Lk2/piJbg7OhHk3kwhSasI0XDmMsA4NpxjA==;EndpointSuffix=core.windows.net";

QueueClient queue = new QueueClient(connectionString, "mystoragequeue");

await InsertMessageAsync(queue, "this is a delayed message");

//if (args.Length > 0)
//{
//    string value = String.Join(" ", args);
//    await InsertMessageAsync(queue, value);
//    Console.WriteLine($"Sent: {value}");
//}
//else
//{
//    string value = await RetrieveNextMessageAsync(queue);
//    Console.WriteLine($"Received: {value}");
//}

Console.Write("Press Enter...");
Console.ReadLine();

static async Task InsertMessageAsync(QueueClient theQueue, string newMessage)
{
    if (null != await theQueue.CreateIfNotExistsAsync())
    {
        Console.WriteLine("The queue was created.");
    }

    await theQueue.SendMessageAsync(Base64Encode(newMessage), TimeSpan.FromMinutes(3), null);
}

static async Task<string> RetrieveNextMessageAsync(QueueClient theQueue)
{
    if (await theQueue.ExistsAsync())
    {
        QueueProperties properties = await theQueue.GetPropertiesAsync();

        if (properties.ApproximateMessagesCount > 0)
        {
            QueueMessage[] retrievedMessage = await theQueue.ReceiveMessagesAsync(1);
            string theMessage = retrievedMessage[0].Body.ToString();
            await theQueue.DeleteMessageAsync(retrievedMessage[0].MessageId, retrievedMessage[0].PopReceipt);

            await theQueue.SendMessageAsync(theMessage, TimeSpan.FromMinutes(5), null);

            return theMessage;
        }
        else
        {
            Console.Write("The queue is empty. Attempt to delete it? (Y/N) ");
            string response = Console.ReadLine();

            if (response.ToUpper() == "Y")
            {
                await theQueue.DeleteIfExistsAsync();
                return "The queue was deleted.";
            }
            else
            {
                return "The queue was not deleted.";
            }
        }
    }
    else
    {
        return "The queue does not exist. Add a message to the command line to create the queue and store the message.";
    }
}

static string Base64Encode(string plainText)
{
    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
    return System.Convert.ToBase64String(plainTextBytes);
}

static string Base64Decode(string base64EncodedData)
{
    var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
    return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
}