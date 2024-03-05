import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';
import 'package:shared_preferences/shared_preferences.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final _apiKeyController = TextEditingController();
  final _apiKeyFocusNode = FocusNode();

  @override
  void initState() {
    super.initState();

    _apiKeyFocusNode.requestFocus();
  }

  @override
  void dispose() {
    _apiKeyController.dispose();
    _apiKeyFocusNode.dispose();

    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Enmeshed Admin UI - Login'),
      ),
      body: Center(
        child: SizedBox(
          width: 300,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Image.asset('assets/icon.png', width: 100, height: 100),
              const SizedBox(height: 40),
              TextField(
                controller: _apiKeyController,
                focusNode: _apiKeyFocusNode,
                decoration: const InputDecoration(
                  labelText: 'API Key',
                  border: OutlineInputBorder(),
                ),
                onSubmitted: (_) => _login(),
              ),
              const SizedBox(height: 20),
              ElevatedButton(
                onPressed: _login,
                child: const Text('Login'),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Future<void> _login() async {
    final apiKey = _apiKeyController.text.trim();
    if (apiKey.isEmpty) return;

    const baseUrl = kIsWeb ? '' : String.fromEnvironment('BASE_URL');
    print(baseUrl);
    final apiKeyValid = await AdminApiClient.validateApiKey(baseUrl: baseUrl, apiKey: apiKey);

    if (!mounted) return;

    if (!apiKeyValid) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Invalid API Key'),
          backgroundColor: Colors.red,
        ),
      );
      return;
    }

    await (await SharedPreferences.getInstance()).setString('api_key', apiKey);
    await GetIt.I.reset();

    GetIt.I.registerSingleton(await AdminApiClient.create(baseUrl: baseUrl, apiKey: apiKey));
    if (mounted) context.go('/dashboard');
  }
}
