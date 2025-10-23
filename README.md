# Backbone

[![GitHub Actions CI](https://github.com/nmshd/backbone/workflows/Publish/badge.svg)](https://github.com/nmshd/backbone/actions?query=workflow%3APublish)
[![Artifact Hub](https://img.shields.io/endpoint?url=https://artifacthub.io/badge/repository/enmeshed-backbone)](https://artifacthub.io/packages/helm/enmeshed-backbone/backbone-helm-chart)

The Enmeshed Backbone embraces all central services required by the Enmeshed platform to work. It consists of the underlying infrastructure, its hosted services, and the libraries used within the services.

## Documentation

You can find the documentation for the Enmeshed Backbone on [enmeshed.eu](https://enmeshed.eu/explore/backbone).

## Feedback

Please file any bugs or feature requests by creating an [issue](https://github.com/nmshd/feedback/issues).

Share your feedback with the Enmeshed team by contributing to the [discussions](https://github.com/nmshd/feedback/discussions).

## Contribute

Contribution to this project is highly appreciated. Head over to our [contribution guide](https://github.com/nmshd/.github/blob/main/CONTRIBUTING.md) to learn more.

## Development

### Logging

Log event IDs are random 6-digit numbers and can be generated using `./scripts/linux/generate_log_event_id.sh` or `./scripts/windows/generate_log_event_id.ps1`. Simply run the scripts to generate a random 6-digit positive integer.

## License

[AGPL-3.0-or-later](LICENSE)
