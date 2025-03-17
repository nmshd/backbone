import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:enmeshed_ui_kit/enmeshed_ui_kit.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';
import 'package:shared_preferences/shared_preferences.dart';

class SplashScreen extends StatefulWidget {
  final String? redirect;

  const SplashScreen({required this.redirect, super.key});

  @override
  State<SplashScreen> createState() => _SplashScreenState();
}

class _SplashScreenState extends State<SplashScreen> {
  @override
  void initState() {
    super.initState();

    _route();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            SvgPicture.asset('assets/logo.svg', width: 200, height: 200),
            Gaps.h40,
            const SizedBox(width: 300, child: LinearProgressIndicator()),
          ],
        ),
      ),
    );
  }

  Future<void> _route() async {
    await GetIt.I.allReady();
    final sp = await SharedPreferences.getInstance();

    if (!sp.containsKey('api_key')) return _navigateToLogin();

    final apiKey = sp.getString('api_key')!;
    const baseUrl = kIsWeb ? '' : String.fromEnvironment('base_url');

    final isValid = await AdminApiClient.validateApiKey(baseUrl: baseUrl, apiKey: apiKey);
    if (!isValid) return _navigateToLogin();

    GetIt.I.registerSingleton(await AdminApiClient.create(baseUrl: baseUrl, apiKey: apiKey));

    if (!mounted) return;

    if (widget.redirect != null) return context.go(Uri.decodeComponent(widget.redirect!));

    context.go('/identities');
  }

  void _navigateToLogin() => context.go(widget.redirect != null ? '/login?redirect=${widget.redirect!}' : '/login');
}
