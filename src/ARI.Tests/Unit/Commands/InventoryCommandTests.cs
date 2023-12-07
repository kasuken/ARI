﻿using ARI.Commands.Settings;
using ARI.Commands;
using Cake.Core;
using Spectre.Console.Cli;
using ARI.Services.Markdown;

namespace ARI.Tests.Unit.Commands;

[TestFixture]
public class InventoryCommandTests
{
    [TestCase(false)]
    [TestCase(true)]
    public async Task ExecuteAsync(bool skipTenantOverview)
    {
        // Given
        var context = new CommandContext(
            Substitute.For<IRemainingArguments>(),
            nameof(InventoryCommand),
            null
            );
        var settings = new InventorySettings
        {
            TenantId = Constants.Tenant.Id,
            OutputPath = "/home/docs",
            SkipTenantOverview = skipTenantOverview
        };
        var (
            cakeContext,
            logger,
            tenantService,
            subscriptionService,
            resourceGroupService,
            resourceService,
            markdownServices
            ) = ARIServiceProviderFixture.GetRequiredService<ICakeContext, ILogger<InventoryCommand>, TenantService, SubscriptionService, ResourceGroupService, ResourceService, IEnumerable<MarkdownServiceBase>>(
                services => services
                                .AddCakeFakes(
                                    fileSystem => fileSystem.CreateDirectory(settings.OutputPath)
                                )
            );

        var command = new InventoryCommand(
            cakeContext,
            logger,
            tenantService,
            subscriptionService,
            resourceGroupService,
            resourceService,
            markdownServices
            );

        // When
        var result = new
        {
            ExitCode = await command.ExecuteAsync(
                            context,
                            settings
                        ),
            Output = cakeContext.FileSystem.FromDirectoryPath(settings.OutputPath)
        };

        // Then
        await Verify(result);
    }
}

