import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';
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

  bool _isButtonEnabled = false;
  bool apiKeyValid = false;
  bool _attemptedLogin = false;

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
    final screenSize = MediaQuery.of(context).size;
    final topPadding = screenSize.height > 1080 ? screenSize.height * 0.2 : screenSize.height * 0.3;

    return Scaffold(
      appBar: AppBar(
        title: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            SvgPicture.asset(
              'assets/logo.svg',
              width: 30,
              height: 30,
            ),
            const SizedBox(width: 10),
            const Text(
              'enmeshed',
              style: TextStyle(fontWeight: FontWeight.bold),
            ),
            const SizedBox(width: 10),
            const Text('Admin UI'),
          ],
        ),
        leading: const SizedBox(
          width: 40,
        ),
      ),
      body: SingleChildScrollView(
        child: Padding(
          padding: EdgeInsets.only(top: topPadding),
          child: Align(
            alignment: Alignment.topCenter,
            child: SizedBox(
              width: 500,
              height: 250,
              child: Card(
                child: SizedBox(
                  width: 300,
                  child: Padding(
                    padding: const EdgeInsets.only(top: 20, left: 25, right: 25),
                    child: Column(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        TextField(
                          controller: _apiKeyController,
                          focusNode: _apiKeyFocusNode,
                          decoration: InputDecoration(
                            labelText: 'API Key',
                            border: const OutlineInputBorder(),
                            errorBorder: const OutlineInputBorder(
                              borderSide: BorderSide(color: Colors.red, width: 2),
                            ),
                            focusedErrorBorder: const OutlineInputBorder(
                              borderSide: BorderSide(color: Colors.red, width: 2),
                            ),
                            errorText: (_attemptedLogin && !apiKeyValid) ? 'Invalid API Key' : null,
                          ),
                          onSubmitted: (_) => _login(),
                          onChanged: (text) {
                            setState(() {
                              _isButtonEnabled = text.isNotEmpty;
                              _attemptedLogin = false;
                              apiKeyValid = false;
                            });
                          },
                          obscureText: true,
                        ),
                        const SizedBox(height: 20),
                        SizedBox(
                          width: double.infinity,
                          height: 40,
                          child: ElevatedButton(
                            onPressed: _isButtonEnabled ? _login : null,
                            child: const Text('Login'),
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
              ),
            ),
          ),
        ),
      ),
      backgroundColor: const Color.fromARGB(255, 211, 228, 255),
    );
  }

  Future<void> _login() async {
    _attemptedLogin = true;
    final apiKey = _apiKeyController.text.trim();
    if (apiKey.isEmpty) return;

    const baseUrl = kIsWeb ? '' : String.fromEnvironment('base_url');
    apiKeyValid = await AdminApiClient.validateApiKey(baseUrl: baseUrl, apiKey: apiKey);

    if (!mounted) return;

    if (!apiKeyValid) {
      setState(() {});
      return;
    }

    final sp = await SharedPreferences.getInstance();
    await sp.setString('api_key', apiKey);
    await GetIt.I.reset();

    GetIt.I.registerSingleton(await AdminApiClient.create(baseUrl: baseUrl, apiKey: apiKey));
    if (mounted) context.go('/dashboard');
  }
}
