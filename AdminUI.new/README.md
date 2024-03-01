# Admin UI

## Development Guide

### Prerequisites

-   [Flutter](https://flutter.dev/docs/get-started/install) (we are using the `stable` channel)
-   [melos](https://melos.invertase.dev/getting-started)

    TLDR: `dart pub global activate melos`

    Make sure to [add the system cache bin directory to your path](https://dart.dev/tools/pub/cmd/pub-global#running-a-script-from-your-path) (`$HOME/.pub-cache/bin` for mac and linux and `%LOCALAPPDATA%\Pub\Cache\bin` for most Windows versions).

### Getting Started

Run `melos bootstrap` to [install and link dependencies](https://melos.invertase.dev/commands/bootstrap) in all packages and apps.

### Melos scripts

For an overview of all available melos scripts, run `melos run` or `melos run --help`.
