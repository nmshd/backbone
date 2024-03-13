import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';

class Dashboard extends StatefulWidget {
  const Dashboard({super.key});

  @override
  State<Dashboard> createState() => _DashboardState();
}

class _DashboardState extends State<Dashboard> {
  var _pageNumber = 0;
  var _pageSize = 1;

  void _fetchData(int pageNumber, int pageSize) {
    setState(() {
      _pageSize = pageSize;
      _pageNumber = pageNumber;
    });
  }

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
                  'Tier ID: ${snapshot.data!.data.id} \n Tier name: ${snapshot.data!.data.name} \n Tier quotas: ${snapshot.data!.data.quotas.map((e) => e.metric.key)}\n',
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
                if (snapshot.error != null) return Text('Error: ${snapshot.error}');
                return Text(
                  'Identity created at: ${snapshot.data?.data.createdAt} \n',
                );
              },
            ),
            FutureBuilder(
              future: GetIt.I.get<AdminApiClient>().identities.getIdentities(
                    filter: IdentityOverviewFilter(
                      // address: 'id12Pbi7CgBHaFxge6uy1h6qUkedjQr8XHfm',
                      // tiers: ['TIRJs0LlenSqLFy4pCcr'],
                      createdAt: FilterOperatorValue(
                        FilterOperator.lessThan,
                        '2024-02-21T09:25:06.88502+01:00',
                      ),
                      lastLoginAt: FilterOperatorValue(
                        FilterOperator.lessThan,
                        '2024-03-11T09:25:06.88502+01:00',
                      ),
                    ),
                    pageNumber: _pageNumber,
                    pageSize: _pageSize,
                  ),
              builder: (context, snapshot) {
                if (snapshot.connectionState == ConnectionState.waiting) {
                  return const CircularProgressIndicator();
                } else if (snapshot.hasError) {
                  return Text('Error: ${snapshot.error}');
                } else if (!snapshot.hasData || snapshot.data!.data.isEmpty) {
                  return const Text('No data available.');
                } else {
                  final addresses = snapshot.data!.data.map((e) => e.address).toList();
                  return Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const Text('Identities:'),
                      Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: addresses.map(Text.new).toList(),
                      ),
                      Row(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          ElevatedButton(
                            onPressed: _pageNumber > 0 ? () => _fetchData(_pageNumber - 1, _pageSize) : null,
                            child: const Text('Previous'),
                          ),
                          const SizedBox(width: 10),
                          ElevatedButton(
                            onPressed: addresses.length == _pageSize ? () => _fetchData(_pageNumber + 1, _pageSize) : null,
                            child: const Text('Next'),
                          ),
                        ],
                      ),
                    ],
                  );
                }
              },
            ),
            FutureBuilder(
              future: GetIt.I.get<AdminApiClient>().relationships.getRelationshipsByParticipantAddress('id12Pbi7CgBHaFxge6uy1h6qUkedjQr8XHfm', 0, 10),
              builder: (context, snapshot) {
                if (snapshot.error != null) return Text('Error: ${snapshot.error}');
                return Text(
                  'Relationships count: ${snapshot.data?.data.length} \n ',
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
