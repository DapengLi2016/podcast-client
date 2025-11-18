# Copyright (c) Microsoft. All rights reserved.
# Licensed under the MIT license. See LICENSE.md file in the project root for full license information.

import locale
from datetime import datetime
from dataclasses import dataclass
from urllib3.util import Url
from typing import Optional

from microsoft_speech_client_common.client_common_enum import (
    OperationStatus, OneApiState
)

from microsoft_client_podcast.podcast_enum import (
    ContentSourceKind
)

from microsoft_speech_client_common.client_common_dataclass import (
    StatelessResourceBaseDefinition, StatefulResourceBaseDefinition
)


@dataclass(kw_only=True)
class PodcastGenerationContent:
    url: Url = None
    text: str = None
    kind: Optional[ContentSourceKind] = None


@dataclass(kw_only=True)
class PodcastGenerationConfig:
    locale: locale
    focus: Optional[str] = None

@dataclass(kw_only=True)
class PodcastGenerationOutput:
    audioFileUrl: Url

@dataclass(kw_only=True)
class PodcastGenerationDefinition(StatefulResourceBaseDefinition):
    content: PodcastGenerationContent = None
    config: Optional[PodcastGenerationConfig] = None
    output: PodcastGenerationOutput = None

@dataclass(kw_only=True)
class PagedGenerationDefinition:
    value: list[PodcastGenerationDefinition]
    nextLink: Optional[Url] = None

