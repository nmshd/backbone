import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '/core/core.dart';

class HomeScreen extends StatefulWidget {
  final Widget child;
  final String location;

  const HomeScreen({required this.child, required this.location, super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  bool extended = false;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const AppTitle(),
        leading: IconButton(
          icon: const Icon(Icons.menu),
          onPressed: () {
            setState(() {
              extended = !extended;
            });
          },
        ),
        actions: [
          SizedBox(
            height: 35,
            width: 120,
            child: OutlinedButton(
              onPressed: _logout,
              child: const Row(
                children: [
                  Icon(Icons.logout, size: 18),
                  Gaps.w4,
                  Text('Logout', style: TextStyle(fontSize: 12.5)),
                ],
              ),
            ),
          ),
          Gaps.w40,
        ],
      ),
      body: Row(
        children: [
          NavigationRail(
            extended: extended,
            destinations: const [
              NavigationRailDestination(icon: Icon(Icons.account_circle_sharp), label: Text('Identities')),
              NavigationRailDestination(icon: Icon(Icons.cable), label: Text('Tiers')),
              NavigationRailDestination(icon: Icon(Icons.layers), label: Text('Clients')),
            ],
            selectedIndex: _selectedIndex,
            onDestinationSelected: (int index) {
              if (index == _selectedIndex) return;

              context.go(
                switch (index) {
                  0 => '/identities',
                  1 => '/tiers',
                  2 => '/clients',
                  _ => throw Exception(),
                },
              );
            },
          ),
          Expanded(child: widget.child),
        ],
      ),
    );
  }

  int get _selectedIndex {
    if (widget.location.startsWith('/identities')) return 0;
    if (widget.location.startsWith('/tiers')) return 1;
    if (widget.location.startsWith('/clients')) return 2;

    throw Exception();
  }

  Future<void> _logout() async {
    final sp = await SharedPreferences.getInstance();
    await sp.remove('api_key');
    await GetIt.I.unregisterIfRegistered<AdminApiClient>();

    if (mounted) context.go('/login');
  }
}
