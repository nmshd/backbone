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
                return Text('Clients: ${snapshot.data!.data.map((e) => e.displayName).join(', ')}');
              },
            ),
            const SizedBox(height: 16),
            FutureBuilder(
              future: GetIt.I.get<AdminApiClient>().tiers.getTiers(),
              builder: (context, snapshot) {
                if (!snapshot.hasData) return const CircularProgressIndicator();
                return Text('Tiers: ${snapshot.data!.data.map((e) => e.name).join(', ')}');
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
