import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';
import 'package:shared_preferences/shared_preferences.dart';

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
        title: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Image.asset('assets/icon.png', width: 30, height: 30),
            const Text('Enmeshed Admin UI'),
          ],
        ),
        leading: IconButton(
          icon: const Icon(Icons.menu),
          onPressed: () {
            setState(() {
              extended = !extended;
            });
          },
        ),
        actions: [
          IconButton(
            icon: const Icon(Icons.logout),
            onPressed: () async {
              await (await SharedPreferences.getInstance()).clear();
              await GetIt.I.reset();
              if (context.mounted) context.go('/login');
            },
          ),
          const SizedBox(width: 10),
        ],
      ),
      body: Row(
        children: [
          NavigationRail(
            extended: extended,
            destinations: const [
              NavigationRailDestination(icon: Icon(Icons.apps), label: Text('Dashboard')),
              NavigationRailDestination(icon: Icon(Icons.badge), label: Text('Identities')),
              NavigationRailDestination(icon: Icon(Icons.clear_all), label: Text('Tiers')),
              NavigationRailDestination(icon: Icon(Icons.person), label: Text('Clients')),
            ],
            selectedIndex: _selectedIndex,
            onDestinationSelected: (int index) {
              if (index == _selectedIndex) return;

              context.go(
                switch (index) {
                  0 => '/dashboard',
                  1 => '/identities',
                  2 => '/tiers',
                  3 => '/clients',
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
    if (widget.location.startsWith('/dashboard')) return 0;
    if (widget.location.startsWith('/identities')) return 1;
    if (widget.location.startsWith('/tiers')) return 2;
    if (widget.location.startsWith('/clients')) return 3;

    throw Exception();
  }
}
