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

class _HomeScreenState extends State<HomeScreen> with SingleTickerProviderStateMixin {
  bool extended = false;

  late AnimationController _controller;
  late Animation<double> _widthAnimation;
  final maxWidth = 300.0;
  final minWidth = 64.0;

  @override
  void initState() {
    super.initState();
    _controller = AnimationController(
      duration: const Duration(milliseconds: 300),
      reverseDuration: const Duration(milliseconds: 300),
      vsync: this,
    )..addStatusListener((status) {
        if (status == AnimationStatus.completed) {
          extended = true;
        } else if (status == AnimationStatus.dismissed) {
          extended = false;
        }
      });
    _widthAnimation = Tween<double>(begin: minWidth, end: maxWidth).animate(_controller);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const AppTitle(),
        leading: IconButton(
          icon: const Icon(Icons.menu),
          onPressed: toggleDrawer,
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
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          AnimatedBuilder(
            animation: _controller,
            builder: (context, child) {
              return Padding(
                padding: const EdgeInsets.only(top: 8, right: 8, bottom: 8),
                child: SizedBox(
                  width: _widthAnimation.value,
                  height: 200,
                  child: NavigationDrawer(
                    elevation: 0,
                    backgroundColor: Theme.of(context).colorScheme.outline.withAlpha(60),
                    children: [
                      Gaps.h8,
                      buildHeaderSection('Functionalities'),
                      if (_widthAnimation.value == maxWidth)
                        const Padding(
                          padding: EdgeInsets.only(left: 8, right: 8),
                          child: Divider(),
                        ),
                      buildNavigationTile(context, 'Identities', Icons.account_circle_sharp, isSelected: true),
                      buildNavigationTile(context, 'Tiers', Icons.cable, index: 1),
                      buildNavigationTile(context, 'Clients', Icons.layers, index: 2),
                    ],
                  ),
                ),
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

  void navigate(int index) {
    switch (index) {
      case 0:
        context.go('/identities');
      case 1:
        context.go('/tiers');
      case 2:
        context.go('/clients');
      default:
        throw Exception('Invalid index');
    }
  }

  void toggleDrawer() {
    if (extended) {
      _controller.reverse();
    } else {
      _controller.forward();
    }
  }

  Future<void> _logout() async {
    final sp = await SharedPreferences.getInstance();
    await sp.remove('api_key');
    await GetIt.I.unregisterIfRegistered<AdminApiClient>();

    if (mounted) context.go('/login');
  }

  Widget buildHeaderSection(String title) {
    return _widthAnimation.value == maxWidth
        ? Padding(
            padding: const EdgeInsets.only(left: 12),
            child: Text(
              title,
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.bold,
              ),
            ),
          )
        : Padding(
            padding: const EdgeInsets.symmetric(vertical: 18),
            child: Container(),
          );
  }

  Widget buildNavigationTile(BuildContext context, String title, IconData icon, {int index = 0, bool isSelected = false}) {
    final isSelected = _selectedIndex == index;

    final borderRadius = _widthAnimation.value > minWidth ? BorderRadius.circular(40) : BorderRadius.circular(100);

    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
      child: InkWell(
        onTap: () => navigate(index),
        splashColor: Theme.of(context).colorScheme.secondaryContainer.withOpacity(0.3),
        borderRadius: borderRadius,
        child: DecoratedBox(
          decoration: BoxDecoration(
            color: isSelected ? Theme.of(context).colorScheme.secondaryContainer : Colors.transparent,
            borderRadius: borderRadius,
          ),
          child: Padding(
            padding: const EdgeInsets.symmetric(vertical: 8),
            child: Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 8),
                  child: Icon(
                    icon,
                    size: 30,
                    color: isSelected ? Theme.of(context).colorScheme.onSecondaryContainer : Theme.of(context).colorScheme.onSecondaryContainer,
                  ),
                ),
                if (_widthAnimation.value == maxWidth) ...[
                  Gaps.w16,
                  Text(
                    title,
                    style: TextStyle(
                      color: isSelected ? Theme.of(context).colorScheme.onSecondaryContainer : Theme.of(context).colorScheme.onSurface,
                      fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
                    ),
                  ),
                ] else
                  Container(),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
