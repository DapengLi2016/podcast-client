//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.Podcast.ApiSampleCode;

using CommandLine;
using Microsoft.SpeechServices.CommonLib;

[Verb("get", HelpText = "Get generation by ID.")]
public class GetOptions : BaseOptions
{
    [Option("id", Required = true, HelpText = PodcastPublicConst.ArgumentDescription.GenerationId)]
    public string Id { get; set; }
}