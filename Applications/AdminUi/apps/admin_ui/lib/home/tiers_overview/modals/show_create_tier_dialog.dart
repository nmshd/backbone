import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';

import '/core/core.dart';

Future<Tier?> showCreateTierDialog({required BuildContext context}) =>
    showDialog<Tier>(context: context, builder: (BuildContext context) => const _CreateTierDialog());

class _CreateTierDialog extends StatefulWidget {
  const _CreateTierDialog();

  @override
  State<_CreateTierDialog> createState() => _CreateTierDialogState();
}

class _CreateTierDialogState extends State<_CreateTierDialog> {
  final _tierNameController = TextEditingController();
  final _focusNode = FocusNode();

  String? _errorMessage;
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();

    _focusNode.requestFocus();
  }

  @override
  void dispose() {
    _tierNameController.dispose();

    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return PopScope(
      canPop: !_isLoading,
      child: AlertDialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
        title: Text(context.l10n.createTierDialog_title, textAlign: TextAlign.center),
        contentPadding: const EdgeInsets.only(left: 24, right: 24, top: 20, bottom: 32),
        content: _isLoading
            ? const Padding(
                padding: EdgeInsets.all(16),
                child: Wrap(alignment: WrapAlignment.center, children: [CircularProgressIndicator()]),
              )
            : SizedBox(
                width: 500,
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: <Widget>[
                    Padding(padding: const EdgeInsets.all(8), child: Text('*${context.l10n.required}')),
                    Gaps.h32,
                    TextField(
                      controller: _tierNameController,
                      focusNode: _focusNode,
                      onChanged: (_) {
                        if (_errorMessage == null) return;
                        setState(() => _errorMessage = null);
                      },
                      onSubmitted: _onSubmitted,
                      decoration: InputDecoration(
                        border: const OutlineInputBorder(),
                        labelText: context.l10n.name,
                        helperText: context.l10n.createTierDialog_formMessage,
                        error: _errorMessage != null
                            ? Text(
                                _errorMessage!,
                                style: Theme.of(context).textTheme.labelSmall!.copyWith(color: Theme.of(context).colorScheme.error),
                                textAlign: TextAlign.left,
                              )
                            : null,
                      ),
                    ),
                  ],
                ),
              ),
        actions: <Widget>[
          OutlinedButton(onPressed: _isLoading ? null : () => context.pop(), child: Text(context.l10n.cancel)),
          FilledButton(onPressed: _isLoading ? null : () => _onSubmitted(_tierNameController.text), child: Text(context.l10n.create)),
        ],
      ),
    );
  }

  Future<void> _onSubmitted(String name) async {
    if (name.isEmpty) {
      _setErrorMessage(context.l10n.createTierDialog_nameCannotBeEmpty);
      _focusNode.requestFocus();
      return;
    }

    setState(() => _isLoading = true);
    final response = await GetIt.I.get<AdminApiClient>().tiers.createTier(name: name);
    if (!mounted) return;

    if (response.hasData) {
      context.pop(response.data);
      _showSuccessSnackbar();
      return;
    }

    _setErrorMessage(response.error.message);
    _focusNode.requestFocus();
  }

  void _setErrorMessage(String message) => setState(() {
    _errorMessage = message;
    _isLoading = false;
  });

  void _showSuccessSnackbar() {
    final snackBar = SnackBar(
      content: Text(context.l10n.createTierDialog_tierCreatedSuccess, style: const TextStyle(color: Colors.white)),
      backgroundColor: Colors.green,
      duration: const Duration(seconds: 3),
      showCloseIcon: true,
    );
    ScaffoldMessenger.of(context).showSnackBar(snackBar);
  }
}
