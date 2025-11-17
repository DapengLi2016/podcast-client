//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.Podcast.ApiSampleCode;

using CommandLine;
using Microsoft.SpeechServices.CommonLib;
using Microsoft.SpeechServices.Podcast.ApiSampleCode;
using System.Globalization;

[Verb("createGenerationAndWaitUntilTerminated", HelpText = "Create generation and wait until terminated.")]
public class CreateGenerationAndWaitUntilTerminatedOptions : BaseOptions
{
    [Option("id", Required = false, HelpText = PodcastPublicConst.ArgumentDescription.GenerationId)]
    public string Id { get; set; }

    [Option("contentFileAzureBlobUrl", Required = false, HelpText = PodcastPublicConst.ArgumentDescription.ContentFileAzureBlobUrl)]
    public Uri ContentFileAzureBlobUrl { get; set; }

    [Option("contentFilePath", Required = false, HelpText = PodcastPublicConst.ArgumentDescription.ContentFilePath)]
    public string ContentFilePath { get; set; }

    [Option("targetLocale", Required = false, HelpText = PodcastPublicConst.ArgumentDescription.Locale)]
    public CultureInfo TargetLocale { get; set; }

    [Option("focus", Required = false, HelpText = PodcastPublicConst.ArgumentDescription.Locale)]
    public string Focus { get; set; }
}

