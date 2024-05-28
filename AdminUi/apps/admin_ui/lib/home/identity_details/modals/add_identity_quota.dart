import 'dart:async';

import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';

import '/core/core.dart';

Future<void> showAddIdentityQuotaDialog({required BuildContext context, required String address, required VoidCallback onQuotaAdded}) async {
  final metrics = await GetIt.I.get<AdminApiClient>().quotas.getMetrics();

  if (!context.mounted) return;

  await showDialog<void>(
    context: context,
    builder: (BuildContext context) => AddQuotaDialog(
      availableMetrics: metrics.data,
      addQuota: ({required String metricKey, required int max, required String period}) =>
          GetIt.I.get<AdminApiClient>().quotas.createIdentityQuota(address: address, metricKey: metricKey, max: max, period: period),
      onQuotaAdded: onQuotaAdded,
    ),
  );
}
