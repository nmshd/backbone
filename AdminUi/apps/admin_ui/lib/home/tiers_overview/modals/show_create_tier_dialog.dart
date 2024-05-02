import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';

import '/core/core.dart';

Future<Tier?> showCreateTierDialog({required BuildContext context}) => showDialog<Tier>(
      context: context,
      builder: (BuildContext context) => const _CreateTierDialog(),
    );

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
        title: const Text('Create Tier'),
        content: _isLoading
            ? const Padding(
                padding: EdgeInsets.all(16),
                child: Wrap(alignment: WrapAlignment.center, children: [CircularProgressIndicator()]),
              )
            : Column(
                mainAxisSize: MainAxisSize.min,
                children: <Widget>[
                  const Text('Please fill the form below to create your Tier'),
                  Gaps.h16,
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
                      labelText: 'Name',
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
        actions: <Widget>[
          OutlinedButton(
            onPressed: _isLoading ? null : () => context.pop(),
            child: const Text('Cancel'),
          ),
          FilledButton(
            onPressed: _isLoading ? null : () => _onSubmitted(_tierNameController.text),
            child: const Text('Add'),
          ),
        ],
      ),
    );
  }

  Future<void> _onSubmitted(String name) async {
    if (name.isEmpty) {
      _setErrorMessage('Name cannot be empty.');
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
    const snackBar = SnackBar(
      content: Text(
        'Tier was created successfully.',
        style: TextStyle(color: Colors.white),
      ),
      backgroundColor: Colors.green,
      duration: Duration(seconds: 3),
    );
    ScaffoldMessenger.of(context).showSnackBar(snackBar);
  }
}
