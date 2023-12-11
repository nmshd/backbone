# Prerequisites

## Blob Storage

The Backbone uses a blob storage to store files. Currently Azure Storage Account and Goolge Cloud Storage are supported. Which one is used can be specified by setting the corresponding parameter (`modules.<moduleName>.infrastructure.blobStorage.cloudProvider`) to `Azure` or `GoogleCloud`. If you use Azure Storage Account, you need to specify a connection string for each module (see [Required environment variables](#required-environment-variables)). If you use Google Cloud Storage, you need to specify a service account with the necessary permissions (read/write).

In both cases you have to specify a `containerName` (parameter: `modules.<moduleName>.infrastructure.blobStorage.containerName`). What does it look like???????????????????????????????

## Database

## Consumer API

The Consumer API is the HTTP API you can use to create Identities and communicate with other Identities.

### Required environment variables

The Consumer API needs some sensible information. This information should not be filled via normal configuration. Instead, environment variables should be used, in combination with Kubernetes secrets or something similar.

For this, copy the following yaml and paste it into the `consumerapi.env` section of your `values.yaml` file. Fill the placeholders ("`...`") with the corresponding values.

k create secret generic -n ablage-integration challenges-sql-connectionstring --from-literal=VALUE='Server=10.11.4.7;Port=5432;User ID=challenges_integration;Password="jphYAG2736Mt05cs";Database=nmshd_bkb_integration;'
k create secret generic -n ablage-integration devices-sql-connectionstring --from-literal=VALUE='Server=10.11.4.7;Port=5432;User ID=devices_integration;Password="jphYAG2736Mt05cs";Database=nmshd_bkb_integration;'
k create secret generic -n ablage-integration files-sql-connectionstring --from-literal=VALUE='Server=10.11.4.7;Port=5432;User ID=files_integration;Password="jphYAG2736Mt05cs";Database=nmshd_bkb_integration;'
k create secret generic -n ablage-integration messages-sql-connectionstring --from-literal=VALUE='Server=10.11.4.7;Port=5432;User ID=messages_integration;Password="jphYAG2736Mt05cs";Database=nmshd_bkb_integration;'
k create secret generic -n ablage-integration quotas-sql-connectionstring --from-literal=VALUE='Server=10.11.4.7;Port=5432;User ID=quotas_integration;Password="jphYAG2736Mt05cs";Database=nmshd_bkb_integration;'
k create secret generic -n ablage-integration relationships-sql-connectionstring --from-literal=VALUE='Server=10.11.4.7;Port=5432;User ID=relationships_integration;Password="jphYAG2736Mt05cs";'Database=nmshd_bkb_integration;
k create secret generic -n ablage-integration synchronization-sql-connectionstring --from-literal=VALUE='Server=10.11.4.7;Port=5432;User ID=synchronization_integration;Password="jphYAG2736Mt05cs";'Database=nmshd_bkb_integration;
k create secret generic -n ablage-integration tokens-sql-connectionstring --from-literal=VALUE='Server=10.11.4.7;Port=5432;User ID=tokens_integration;Password="jphYAG2736Mt05cs";Database=nmshd_bkb_integration;'

k delete secret -n ablage-integration challenges-sql-connectionstring
k delete secret -n ablage-integration devices-sql-connectionstring
k delete secret -n ablage-integration files-sql-connectionstring
k delete secret -n ablage-integration messages-sql-connectionstring
k delete secret -n ablage-integration quotas-sql-connectionstring
k delete secret -n ablage-integration relationships-sql-connectionstring
k delete secret -n ablage-integration synchronization-sql-connectionstring
k delete secret -n ablage-integration tokens-sql-connectionstring

```yaml
- name: "authentication__jwtSigningCertificate"
  valueFrom:
    secretKeyRef:
      name: "..."
      key: "..."
- name: "modules__devices__infrastructure__azureNotificationHub__connectionString"
  valueFrom:
    secretKeyRef:
      name: "..."
      key: "..."
- name: "modules__devices__infrastructure__sqlDatabase__connectionString"
  valueFrom:
    secretKeyRef:
      name: "..."
      key: "..."
- name: "modules__files__infrastructure__sqlDatabase__connectionString"
  valueFrom:
    secretKeyRef:
      name: "..."
      key: "..."
- name: "modules__messages__infrastructure__sqlDatabase__connectionString"
  valueFrom:
    secretKeyRef:
      name: "..."
      key: "..."
- name: "modules__relationships__infrastructure__sqlDatabase__connectionString"
  valueFrom:
    secretKeyRef:
      name: "..."
      key: "..."
- name: "modules__synchronization__infrastructure__sqlDatabase__connectionString"
  valueFrom:
    secretKeyRef:
      name: "..."
      key: "..."
- name: "modules__tokens__infrastructure__sqlDatabase__connectionString"
  valueFrom:
    secretKeyRef:
      name: "..."
      key: "..."
```

The following subsections describe the environment variables in more detail.

#### `authentication__jwtSigningCertificate`

The JWT Signing Certificate is a PKCS#12 certificate used to sign the JSON Web Tokens (JWTs) created by the Consumer API.

If you don't already have a certificate, you can use the following commands to generate one:

```
openssl req -x509 -newkey rsa:2048 -keyout myKey.pem -out cert.pem -days 365000 -nodes -subj /CN=CLIGetDefaultPolicy
jwtSigningCertificate=$(openssl pkcs12 -export -inkey myKey.pem -in cert.pem -passout pass: | base64)
echo $jwtSigningCertificate
```

Use it to e.g. create a Kubernetes secret:

```
kubectl create secret generic jwt-signing-certificate --from-literal=VALUE=$jwtSigningCertificate --namespace=<target-namespace>
```

Note that for **development scenarios** you can omit this environment variable. The Consumer API will then generate a random certificate at startup. But as soon as you want to scale the Backbone horizontally, you need to provide a certificate. Otherwise, each instance of the Consumer API will generate its own certificate. So if a user receives a JWT from one instance, he cannot use it to authenticate against another instance.

#### `modules__devices__infrastructure__azureNotificationHub__connectionString`

Azure Notification Hub is a service of Microsoft Azure. The Backbone uses this service to send push notifications to mobile devices. The connection string looks as follows:

```
Endpoint=sb://<namespace>.servicebus.windows.net/;SharedAccessKeyName=<shared-access-key-name>;SharedAccessKey=<shared-access-key>
```

Note that if you don't need push notifications (e.g. in development/test scenarios) you can instead set the push notification provider to "Dummy" (parameter: `global.configuration.modules.devices.infrastructure.pushNotifications`). This will disable push notifications.

#### `modules__*__infrastructure__sqlDatabase__connectionString`

Environment variables with this pattern contain the database connection strings for the different modules of the Backbone. It is recommended to have a separate user and therefore different connection strings for each of them.

**For PostgreSQL:**

If you want to use PostgreSQL, a connection string with basic information looks as follows:

```
Server=<db-server-address>;Database=<database-name>;User ID=<db-username>;Password=<db-password>
```

See https://www.npgsql.org/doc/connection-string-parameters.html for a list of all available parameters.

**For SQL Server:**

If you want to use SQL Server, the most basic connection string looks like:

```
Server=<db-server-address>;Database=<database-name>;User Id=<db-username>;Password=<db-password>
```

See https://www.connectionstrings.com/sql-server/ for a list of all available parameters.
