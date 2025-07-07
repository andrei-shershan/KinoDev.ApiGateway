namespace KinoDev.ApiGateway.Infrastructure.HttpClients.Abstractions;

public interface IStorageServiceClient
{
    Task<string> UploadFileAsync(string fileName, byte[] bytes);
}