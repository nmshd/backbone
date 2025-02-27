import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '/core/core.dart';

class LoginScreen extends StatefulWidget {
  final String? redirect;

  const LoginScreen({required this.redirect, super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final _apiKeyController = TextEditingController();
  final _apiKeyFocusNode = FocusNode();

  bool? _isApiKeyValid;

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
      backgroundColor: Theme.of(context).colorScheme.inversePrimary,
      appBar: AppBar(title: const AppTitle(), centerTitle: false, leading: Gaps.w40),
      body: Center(
        child: SizedBox(
          width: 400,
          child: Card(
            child: Padding(
              padding: const EdgeInsets.all(25),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  TextField(
                    controller: _apiKeyController,
                    focusNode: _apiKeyFocusNode,
                    obscureText: true,
                    decoration: InputDecoration(
                      labelText: 'API Key',
                      border: const OutlineInputBorder(),
                      errorText: _isApiKeyValid == false ? 'Invalid API Key' : null,
                      helperText: '',
                    ),
                    onChanged: (_) {
                      if (_isApiKeyValid == null) return setState(() {});

                      setState(() => _isApiKeyValid = null);
                    },
                    onSubmitted: (_) => _login(),
                  ),
                  Gaps.h24,
                  FilledButton(
                    style: FilledButton.styleFrom(minimumSize: const Size.fromHeight(45)),
                    onPressed: _apiKeyController.text.isNotEmpty ? _login : null,
                    child: const Text('Login'),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }

  Future<void> _login() async {
    final apiKey = _apiKeyController.text;
    if (apiKey.isEmpty) return;

    final baseUrl = GetIt.I<AppConfig>().baseUrl;
    final isApiKeyValid = await AdminApiClient.validateApiKey(baseUrl: baseUrl, apiKey: apiKey);

    if (!mounted) return;

    if (!isApiKeyValid) {
      setState(() => _isApiKeyValid = false);

      _apiKeyFocusNode.requestFocus();

      return;
    }

    final sp = await SharedPreferences.getInstance();
    await sp.setString('api_key', apiKey);

    await GetIt.I.unregisterIfRegistered<AdminApiClient>();
    GetIt.I.registerSingleton(await AdminApiClient.create(baseUrl: baseUrl, apiKey: apiKey));
    if (mounted) context.go(widget.redirect != null ? Uri.decodeComponent(widget.redirect!) : '/identities');
  }
}
