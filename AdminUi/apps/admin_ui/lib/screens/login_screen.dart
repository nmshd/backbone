import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_ui/styles/widgets/app_title.dart';
import 'package:admin_ui/styles/widgets/custom_elevated_button.dart';
import 'package:admin_ui/styles/widgets/custom_styled_container.dart';
import 'package:admin_ui/styles/widgets/custom_text_field.dart';
import 'package:admin_ui/theme/colors/custom_colors.dart';
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

  bool _isButtonEnabled = false;
  bool _isApiKeyValid = false;
  bool _hasAttemptedLogin = false;

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
    final _customColors = Theme.of(context).extension<CustomColors>();

    return Scaffold(
      appBar: AppBar(
        title: const AppTitle(),
        leading: const SizedBox(
          width: 40,
        ),
      ),
      body: Center(
        child: Card(
          child: CustomStyledContainer(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                SizedBox(
                  height: 100,
                  child: CustomTextField(
                    controller: _apiKeyController,
                    focusNode: _apiKeyFocusNode,
                    label: 'API Key',
                    obscureText: true,
                    errorText: (_hasAttemptedLogin && !_isApiKeyValid) ? 'Invalid API Key' : null,
                    onChanged: (text) {
                      setState(() {
                        _isButtonEnabled = text.isNotEmpty;
                        _hasAttemptedLogin = false;
                        _isApiKeyValid = false;
                      });
                    },
                    onSubmitted: (_) => _login(),
                  ),
                ),
                const SizedBox(height: 20),
                CustomElevatedButton(
                  text: 'Login',
                  isEnabled: _isButtonEnabled,
                  onPressed: _login,
                  backgroundColor: Theme.of(context).colorScheme.primary,
                  textColor: _customColors?.onNeutralvariant,
                ),
              ],
            ),
          ),
        ),
      ),
      backgroundColor: Theme.of(context).colorScheme.inversePrimary,
    );
  }

  Future<void> _login() async {
    _hasAttemptedLogin = true;
    final apiKey = _apiKeyController.text.trim();
    if (apiKey.isEmpty) return;

    const baseUrl = kIsWeb ? '' : String.fromEnvironment('base_url');
    _isApiKeyValid = await AdminApiClient.validateApiKey(baseUrl: baseUrl, apiKey: apiKey);

    if (!mounted) return;

    if (!_isApiKeyValid) {
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
