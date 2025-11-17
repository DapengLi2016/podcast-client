//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.ComponentModel.DataAnnotations;

namespace Microsoft.SpeechServices.Cris.Http.DTOs.Public.Podcast.Public20260101Preview;

public class PodcastGeneration : StatefulResourceBase
{
    public PodcastGenerationConfig Config { get; set; }

    public PodcastGenerationOutput Output { get; set; }

    public PodcastGenerationContent Content { get; set; }

    public string FailureReason { get; set; }
}
