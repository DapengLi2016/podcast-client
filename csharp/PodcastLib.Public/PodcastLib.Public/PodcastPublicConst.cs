//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

namespace Microsoft.SpeechServices.CommonLib;

public static class PodcastPublicConst
{
    public static class ArgumentDescription
    {
        public const string GenerationId = "Specify generation ID.";

        public const string ContentFileAzureBlobUrl = "Content file Azure blob URL, this parameter is conflict with ContentFilePath.";

        public const string ContentFilePath = "Content file path, this parameter is conflict with ContentFileAzureBlobUrl.";

        public const string Locale = "Podcast target generated podcast locale.";
    }
}
