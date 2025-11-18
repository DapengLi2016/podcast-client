
# Podcast generation client sample code for python

# Prerepuest
## Tested OS:
    Ubuntu 24.04.1 LTS
    Windows 11 Enterprise
## Python version:
    3.11.10
## Dependency modules:
    pip3 install termcolor
    pip3 install orjson
    pip3 install urllib3
    pip3 install requests
    pip3 install pydantic

# Platform dependency:
## VS Code
### Create environment with command, Ctrl+Shift+P:
    Python: Create Environment
    Python: Select Interpreter
    Python: 3.11.10
    
### Debug 
    Copy .\.vscode\launch_sample_podcast.json file to .\.vscode\launch.json
    And replace the placeholder with actual vaules like: sub_key, target_locale, id, input_file_url etc.

# Conda support:
    conda create -n Podcast_ClientSampleCode python=3.11.10
    conda activate Podcast_ClientSampleCode

# File Description
| Files | Description |
| --- | --- |
| [main_podcast.py](main_podcast.py)  | client tool main definition |
| [generation_client.py](microsoft_client_podcast/generation_client.py)  | Podcast client definition  |
| [generation_dataclass.py](microsoft_client_podcast/generation_dataclass.py)  | Podcast data contract definition  |
| [generation_enum.py](microsoft_client_podcast/generation_enum.py)  | Podcast enum definition  |
| [generation_const.py](microsoft_client_podcast/generation_const.py)  | Podcast constant definition  |

# Usage for command line tool:
## Usage
Run main.py with command in below pattern:
    python main_podcast.py --api-version 2026-01-01-preview --region eastus --sub_key [YourSpeechresourceKey] [SubCommands] [args...]
## Supported API version
| API version | Description |
| --- | --- |
| 2026-01-01-preview | Public preview version |

## Global parameters
| Argument name | Description | 
| --- | --- |
| region | region of the speech resource |
| sub-key | speech resource key |
| api-version | API version, supported version: 2026-01-01-preview |

## Sub commands definition
| SubCommand | Description |
| --- | --- |
| create_generation_and_wait_until_terminated  | Create podcast generation and wait until iteration terminated |
| get  | Request get translation by ID API |
| list  | Request list translations API |
| delete  | Request delete translation API |

## HTTP client library
Podcast client is defined as class PodcastClient in file [podcast_client.py](microsoft_client_podcast/podcast_client.py)
### Function definitions:
| Function | Description |
| --- | --- |
| create_generation_and_wait_until_terminated | Create podcast generation and wait until iteration terminated |
| request_get_generation  | Query get generation GET API |
| request_list_generations  | Query list generations LIST API |
| request_delete_generation  | Delete generation DELETE API |

# Usage sample for client class:
```
    client = PodcastClient(
        region = "eastus",
        sub_key = "[YourSpeechresourceKey]",
    )
    success, error, translation, iteration = client.create_generation_and_wait_until_terminated(
        input_file_url = "https://xx.blob.core.windows.net/users/xx/xx.mp4?sv=xx",
        target_locale = "en-US",
        focus = "xx",
    )
    if not success:
        return
    print(colored("success", 'green'))
```
Reference function handle_create_generation_and_wait_until_terminated in [main_podcast.py](main_podcast.py)
