//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.Podcast.ApiSampleCode;

using CommandLine;

[Verb("delete", HelpText = "Delete generation by ID.")]
public class DeleteOptions : BaseOptions
{
    [Option("id", Required = true, HelpText = "Specify generation ID.")]
    public string Id { get; set; }
}

