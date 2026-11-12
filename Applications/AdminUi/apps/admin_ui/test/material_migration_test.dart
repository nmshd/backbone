import 'dart:async';

import 'package:admin_ui/core/models/theme_mode_model.dart';
import 'package:admin_ui/core/theme/theme.dart';
import 'package:admin_ui/core/widgets/filters/multi_select.dart';
import 'package:admin_ui/main.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart' as legacy;
import 'package:flutter_test/flutter_test.dart';
import 'package:get_it/get_it.dart';
import 'package:material_ui/material_ui.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:sn_progress_dialog/progress_dialog.dart';

void main() {
  tearDown(() async {
    await GetIt.I.reset();
  });

  for (final locale in ['en', 'de']) {
    for (final mode in [ThemeMode.light, ThemeMode.dark]) {
      testWidgets('renders localized tables and legacy controls in $locale with ${mode.name} theme', (tester) async {
        tester.platformDispatcher.localeTestValue = Locale(locale);
        addTearDown(tester.platformDispatcher.clearLocaleTestValue);
        SharedPreferences.setMockInitialValues({'theme_mode': mode.name});
        GetIt.I.registerSingleton(await ThemeModeModel.create());

        await tester.pumpWidget(const AdminUiApp());
        await tester.pumpAndSettle();

        final source = _TableSource();
        addTearDown(source.dispose);
        List<String>? selection;
        final navigator = tester.state<NavigatorState>(find.byType(Navigator).first)
          ..push<void>(
            MaterialPageRoute(
              builder: (context) => Scaffold(
                body: Column(
                  children: [
                    MultiSelectFilter(
                      label: 'Filter',
                      options: const [(label: 'Option A', value: 'a')],
                      onOptionSelected: (value) => selection = value,
                    ),
                    Expanded(
                      child: PaginatedDataTable2(
                        columns: const [DataColumn2(label: Text('Items'))],
                        source: source,
                        rowsPerPage: 2,
                        availableRowsPerPage: const [2],
                        showFirstLastButtons: true,
                      ),
                    ),
                    Builder(
                      builder: (context) => FilledButton(
                        onPressed: () {
                          final dialog = ProgressDialog(context: context);
                          unawaited(dialog.show(msg: 'Working', max: 1));
                        },
                        child: const Text('Show progress'),
                      ),
                    ),
                  ],
                ),
              ),
            ),
          );
        await tester.pumpAndSettle();

        final tableContext = tester.element(find.byType(PaginatedDataTable2));
        final expectedTheme = mode == ThemeMode.dark ? darkTheme : lightTheme;
        expect(Theme.of(tableContext).colorScheme, expectedTheme.colorScheme);
        expect(legacy.Theme.of(tableContext).colorScheme.primary, expectedTheme.colorScheme.primary);
        expect(legacy.Theme.of(tableContext).brightness, expectedTheme.brightness);
        // The app currently supports English; other system locales fall back to it.
        expect(Localizations.localeOf(tableContext).languageCode, 'en');
        expect(find.text('Item 0'), findsOneWidget);
        expect(find.text('Item 2'), findsNothing);

        final localizations = MaterialLocalizations.of(tableContext);
        expect(localizations.nextPageTooltip, 'Next page');
        await tester.tap(find.byTooltip(localizations.nextPageTooltip));
        await tester.pumpAndSettle();
        expect(find.text('Item 2'), findsOneWidget);
        expect(find.text('Item 0'), findsNothing);

        await tester.tap(find.byType(legacy.InputDecorator));
        await tester.pumpAndSettle();
        await tester.tap(find.text('Option A').last);
        await tester.pumpAndSettle();
        expect(selection, ['a']);
        navigator.pop();
        await tester.pumpAndSettle();

        await tester.tap(find.text('Show progress'));
        await tester.pump();
        await tester.pump(const Duration(milliseconds: 500));
        expect(find.text('Working'), findsOneWidget);
        navigator.pop();
        await tester.pumpAndSettle();
        expect(tester.takeException(), isNull);

        navigator.pop();
        await tester.pumpAndSettle();
        await tester.pumpWidget(const SizedBox.shrink());
      });
    }
  }
}

class _TableSource extends DataTableSource {
  @override
  DataRow getRow(int index) => DataRow.byIndex(index: index, cells: [DataCell(Text('Item $index'))]);

  @override
  bool get isRowCountApproximate => false;

  @override
  int get rowCount => 4;

  @override
  int get selectedRowCount => 0;
}
