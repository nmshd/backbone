import 'dart:async';

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
        title: const AppTitle(padding: .only(left: 80)),
        actions: [
          OutlinedButton.icon(
            style: OutlinedButton.styleFrom(minimumSize: const Size(120, 35)),
            onPressed: _logout,
            icon: const Icon(Icons.logout, size: 18),
            label: const Text('Logout', style: TextStyle(fontSize: 12.5)),
          ),
          Gaps.w40,
        ],
      ),
      body: Row(
        children: [
          NavigationRail(
            labelType: .all,
            destinations: const [
              NavigationRailDestination(icon: Icon(Icons.account_circle_sharp), label: Text('Identities')),
              NavigationRailDestination(icon: Icon(Icons.cable), label: Text('Tiers')),
              NavigationRailDestination(icon: Icon(Icons.layers), label: Text('Clients')),
              NavigationRailDestination(icon: Icon(Icons.event_note), label: Text('Announcements')),
            ],
            trailing: Expanded(
              child: Align(
                alignment: .bottomCenter,
                child: Padding(
                  padding: const EdgeInsets.only(bottom: 16),
                  child: IconButton(onPressed: () => openSettingsDialog(context), icon: const Icon(Icons.settings)),
                ),
              ),
            ),
            selectedIndex: _selectedIndex,
            onDestinationSelected: (index) {
              context.go(switch (index) {
                0 => '/identities',
                1 => '/tiers',
                2 => '/clients',
                3 => '/announcements',
                _ => throw Exception(),
              });
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
    if (widget.location.startsWith('/announcements')) return 3;

    throw Exception();
  }

  Future<void> _logout() async {
    final sp = await SharedPreferences.getInstance();
    await sp.remove('api_key');
    await GetIt.I.unregisterIfRegistered<AdminApiClient>();

    if (mounted) context.go('/login');
  }
}
