// ignore_for_file: avoid_print is's only an example file

import 'dart:io';

import 'package:admin_api_sdk/admin_api_sdk.dart';

void main() async {
  final baseUrl = Platform.environment['BASE_URL']!;
  final apiKey = Platform.environment['API_KEY']!;

  await AdminApiClient.validateApiKey(baseUrl: baseUrl, apiKey: apiKey);

  final client = await AdminApiClient.create(baseUrl: baseUrl, apiKey: apiKey);

  final basicTierId = (await client.tiers.getTiers()).data.firstWhere((element) => element.name == 'Basic').id;

  final clients = await client.clients.getClients();
  print(clients.data.length);

  final newClient = await client.clients.createClient(defaultTier: basicTierId);
  print(newClient.data.clientId);

  final clientInfo = await client.clients.getClient(newClient.data.clientId);
  print(clientInfo.data.displayName);

  final updatedClient = await client.clients.updateClient(newClient.data.clientId, defaultTier: basicTierId, maxIdentities: 100);
  print(updatedClient.data.maxIdentities);

  await client.clients.deleteClient(newClient.data.clientId);

  final identitiesPaged = await client.identities.getIdentities(pageSize: 2);
  print(identitiesPaged.isPaged ? identitiesPaged.pagination : 'Not paged');
}
