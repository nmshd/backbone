import 'package:admin_ui/components/identity/identities_overview.dart';
import 'package:admin_ui/home/clients_overview/clients_overview.dart';
import 'package:flutter/material.dart';

class Identities extends StatelessWidget {
  const Identities({super.key});

  @override
  Widget build(BuildContext context) {
    return const IdentitiesOverview();
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
    return const ClientsOverview();
  }
}
