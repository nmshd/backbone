import 'package:admin_ui/components/components.dart';
import 'package:flutter/material.dart';

class Dashboard extends StatefulWidget {
  const Dashboard({super.key});

  @override
  State<Dashboard> createState() => _DashboardState();
}

class _DashboardState extends State<Dashboard> {
  @override
  Widget build(BuildContext context) {
    return const Placeholder();
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
    return const TierOverviewList();
  }
}

class Clients extends StatelessWidget {
  const Clients({super.key});

  @override
  Widget build(BuildContext context) {
    return const Placeholder();
  }
}
