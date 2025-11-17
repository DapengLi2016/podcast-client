//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.Podcast.ApiSampleCode;

using CommandLine;
using Microsoft.SpeechServices.CommonLib;
using Microsoft.SpeechServices.CommonLib.Public.Interface;
using Microsoft.SpeechServices.Cris.Http.DTOs.Public.Podcast.Public20260101Preview;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

internal class Program
{
    static async Task<int> Main(string[] args)
    {
        var types = LoadVerbs();

        var exitCode = await Parser.Default.ParseArguments(args, types)
            .MapResult(
                options => RunAndReturnExitCodeAsync(options),
                _ => Task.FromResult(1));

        if (exitCode == 0)
        {
            Console.WriteLine("Process completed successfully.");
        }
        else
        {
            Console.WriteLine($"Failure with exit code: {exitCode}");
        }

        return exitCode;
    }

    static async Task<int> RunAndReturnExitCodeAsync(object options)
    {
        var optionsBase = options as BaseOptions;
        ArgumentNullException.ThrowIfNull(optionsBase);
        try
        {
            return await DoRunAndReturnExitCodeAsync(optionsBase).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to run with exception: {e.Message}");
            return CommonPublicConst.ExistCodes.GenericError;
        }
    }

    static async Task<int> DoRunAndReturnExitCodeAsync(BaseOptions baseOptions)
    {
        ArgumentNullException.ThrowIfNull(baseOptions);
        var regionConfig = new ApimApiRegionConfig(baseOptions.Region);

        var httpConfig = new PodcastPublicPreviewHttpClientConfig(
            regionConfig: regionConfig,
            subKey: baseOptions.SubscriptionKey,
            customDomainName: baseOptions.CustomDomainName,
            managedIdentityClientId: baseOptions.ManagedIdentityClientId == Guid.Empty ? null : baseOptions.ManagedIdentityClientId)
        {
            ApiVersion = string.IsNullOrEmpty(baseOptions.ApiVersion) ?
                CommonPublicConst.ApiVersions.ApiVersion20260101Preview : baseOptions.ApiVersion,
        };

        var generationClient = new GenerationClient(httpConfig);

        switch (baseOptions)
        {
            case CreateGenerationAndWaitUntilTerminatedOptions options:
                {
                    ContentSourceKind? kind = null;
                    string text = null;
                    Uri url = null;
                    if (!string.IsNullOrEmpty(options.ContentFilePath))
                    {
                        kind = ContentSourceKind.Text;
                        text = await File.ReadAllTextAsync(options.ContentFilePath).ConfigureAwait(false);
                    }
                    else if (!string.IsNullOrEmpty(options.ContentFileAzureBlobUrl?.OriginalString))
                    {
                        kind = ContentSourceKind.AzureStorageBlobPublicUrl;
                        url = options.ContentFileAzureBlobUrl;
                    }
                    else
                    {
                        throw new InvalidDataException($"Please specify contentLocalFilePath or contentFileAzureBlobUrl");
                    }

                    var generation = new PodcastGeneration()
                    {
                        Id = string.IsNullOrWhiteSpace(options.Id) ?
                            Guid.NewGuid().ToString() : options.Id,
                        DisplayName = options.Id,
                        Description = options.Id,
                        Content = new PodcastGenerationContent()
                        {
                            Kind = kind.Value,
                            Text = text,
                            Url = url,
                        },
                        Config = new PodcastGenerationConfig()
                        {
                            Locale = options.TargetLocale,
                            Focus = options.Focus,
                        }
                    };

                    generation = await generationClient.CreateGenerationAndWaitUntilTerminatedAsync(
                        generation: generation).ConfigureAwait(false);

                    Console.WriteLine();
                    Console.WriteLine("Created generation:");
                    Console.WriteLine(JsonConvert.SerializeObject(
                        generation,
                        Formatting.Indented,
                        CommonPublicConst.Json.WriterSettings));
                    break;
                }

            case ListOptions options:
                {
                    var translations = await generationClient.ListGenerationsAsync().ConfigureAwait(false);
                    Console.WriteLine(JsonConvert.SerializeObject(
                        translations,
                        Formatting.Indented,
                        CommonPublicConst.Json.WriterSettings));
                    break;
                }

            case GetOptions options:
                {
                    var translation = await generationClient.GetGenerationAsync(
                        options.Id).ConfigureAwait(false);
                    Console.WriteLine(JsonConvert.SerializeObject(
                        translation,
                        Formatting.Indented,
                        CommonPublicConst.Json.WriterSettings));
                    break;
                }

            case DeleteOptions options:
                {
                    var response = await generationClient.DeleteGenerationAsync(
                        options.Id).ConfigureAwait(false);
                    Console.WriteLine(response.StatusCode);
                    break;
                }

            default:
                throw new NotSupportedException();
        }

        return CommonPublicConst.ExistCodes.NoError;
    }

    //load all types using Reflection
    private static Type[] LoadVerbs()
    {
        return Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetCustomAttribute<VerbAttribute>() != null).ToArray();
    }
}
