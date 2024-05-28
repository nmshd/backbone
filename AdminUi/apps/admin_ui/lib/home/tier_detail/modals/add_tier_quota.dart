import 'dart:async';

import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';

import '/core/core.dart';

Future<void> showAddQuotaDialog({required BuildContext context, required String tierId, required VoidCallback onQuotaAdded}) async {
  final metrics = await GetIt.I.get<AdminApiClient>().quotas.getMetrics();

  if (!context.mounted) return;

  await showDialog<void>(
    context: context,
    builder: (BuildContext context) => AddQuotaDialog(
      availableMetrics: metrics.data,
      addQuota: ({required String metricKey, required int max, required String period}) =>
          GetIt.I.get<AdminApiClient>().quotas.createTierQuota(tierId: tierId, metricKey: metricKey, max: max, period: period),
      onQuotaAdded: onQuotaAdded,
    ),
  );
}
