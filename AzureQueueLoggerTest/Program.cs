using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NK.Logger.AzureBlob;
using NK.Logger.AzureQueue;

class Program
{
    static void Main()
    {
        // ServiceProvider を構成（シングルトンで登録）
        using var serviceProvider = new ServiceCollection()
            .AddLogging(builder =>
            {
                builder.ClearProviders(); // 既存の Logger をクリア
                builder.AddConsole();

                // AzureBlobLoggerProvider をシングルトンで登録
                builder.Services.AddSingleton<ILoggerProvider>(provider =>
                    new AzureQueueLoggerProvider("UseDevelopmentStorage=true", "test"));
            })
            .BuildServiceProvider();

        // ロガーの取得（シングルトンで管理される）
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        // ログの出力テスト
        logger.LogInformation("AzureBlobLoggerProvider のテスト開始");
        try
        {
            Console.WriteLine("処理中...");
            throw new Exception("テストエラーが発生しました！");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "エラーが発生しました。");
        }
        finally
        {
            logger.LogInformation("AzureBlobLoggerProvider のテスト終了");
        }

        Console.WriteLine("ログ出力が完了しました。Azure Blob Storage を確認してください。");
    }
}
