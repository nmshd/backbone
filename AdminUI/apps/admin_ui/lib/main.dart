import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';

void main() {
  final client = AdminApiClient.create(
    baseUrl: const String.fromEnvironment('BASE_URL'),
    apiKey: const String.fromEnvironment('API_KEY'),
  );
  GetIt.I.registerSingleton(client);

  runApp(const MainApp());
}

class MainApp extends StatelessWidget {
  const MainApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      home: Scaffold(
        body: Center(
          child: Column(
            children: [
              FutureBuilder(
                future: GetIt.I.get<AdminApiClient>().clients.getClients(),
                builder: (context, snapshot) {
                  if (!snapshot.hasData) return const Text('No data');
                  return Text('Clients: ${snapshot.data!.data.map((e) => e.displayName).join(', ')}');
                },
              ),
              FutureBuilder(
                future: GetIt.I.get<AdminApiClient>().tiers.getTiers(),
                builder: (context, snapshot) {
                  if (!snapshot.hasData) return const Text('No data');
                  return Text('Clients: ${snapshot.data!.data.map((e) => e.name).join(', ')}');
                },
              ),
            ],
          ),
        ),
      ),
    );
  }
}
