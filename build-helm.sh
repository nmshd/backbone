helm dependency update helm # CAUTION: use build instead of update in CI
helm template awesome-backbone helm --values values.override.yaml --debug > dist.yml
# cat dist.yml | kubeconform --strict 