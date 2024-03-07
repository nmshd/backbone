import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';

class Dashboard extends StatelessWidget {
  const Dashboard({super.key});

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            FutureBuilder(
              future: GetIt.I.get<AdminApiClient>().clients.getClients(),
              builder: (context, snapshot) {
                if (!snapshot.hasData) return const CircularProgressIndicator();
                if (snapshot.error != null) return Text('Error: ${snapshot.error}');
                return Text(
                  'Clients: ${snapshot.data!.data.map((e) => e.displayName).join(', ')}',
                );
              },
            ),
            FutureBuilder(
              future: GetIt.I.get<AdminApiClient>().tiers.getTier('TIR00000000000000001'),
              builder: (context, snapshot) {
                if (!snapshot.hasData) return const CircularProgressIndicator();
                if (snapshot.error != null) return Text('Error: ${snapshot.error}');
                return Text(
                  'Tier ID: ${snapshot.data!.data.id} \n Tier name: ${snapshot.data!.data.name} \n Tier quotas: ${snapshot.data!.data.quotas.map((e) => e..metric.displayName).join(', ')}\n',
                );
              },
            ),
            FutureBuilder(
              future: GetIt.I.get<AdminApiClient>().quotas.getMetrics(),
              builder: (context, snapshot) {
                if (!snapshot.hasData) return const CircularProgressIndicator();
                if (snapshot.error != null) return Text('Error: ${snapshot.error}');
                return Text(
                  'Metrics: ${snapshot.data!.data.map((e) => e.displayName).join(', ')}',
                );
              },
            ),
            const SizedBox(height: 16),
            FutureBuilder(
              future: GetIt.I.get<AdminApiClient>().tiers.getTiers(),
              builder: (context, snapshot) {
                if (!snapshot.hasData) return const CircularProgressIndicator();
                return Text(
                  'Tiers: ${snapshot.data!.data.map((e) => e.name).join(', ')} \n',
                );
              },
            ),
            FutureBuilder(
              future: GetIt.I.get<AdminApiClient>().identities.getIdentity('id12Pbi7CgBHaFxge6uy1h6qUkedjQr8XHfm'),
              builder: (context, snapshot) {
                if (!snapshot.hasData) return const CircularProgressIndicator();
                if (snapshot.error != null) return Text('Error: ${snapshot.error}');
                return Text(
                  'Identity address: ${snapshot.data!.data.address}',
                );
              },
            ),
          ],
        ),
      ),
    );
  }
}

class Identities extends StatelessWidget {
  const Identities({super.key});

  @override
  Widget build(BuildContext context) {
    return const Placeholder();
  }
}

class Tiers extends StatelessWidget {
  const Tiers({super.key});

  @override
  Widget build(BuildContext context) {
    return const Placeholder();
  }
}

class Clients extends StatelessWidget {
  const Clients({super.key});

  @override
  Widget build(BuildContext context) {
    return const Placeholder();
  }
}
