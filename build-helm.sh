helm dependency build helm 
helm template awesome-backbone helm --values values.override.yaml --debug > dist.yml
