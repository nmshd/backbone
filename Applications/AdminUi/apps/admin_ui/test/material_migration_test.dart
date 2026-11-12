import 'package:admin_ui/core/models/theme_mode_model.dart';
import 'package:admin_ui/core/theme/theme.dart';
import 'package:admin_ui/main.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:get_it/get_it.dart';
import 'package:material_ui/material_ui.dart';
import 'package:shared_preferences/shared_preferences.dart';

void main() {
  tearDown(() async {
    await GetIt.I.reset();
  });

  for (final locale in ['en', 'de']) {
    for (final mode in [ThemeMode.light, ThemeMode.dark]) {
      testWidgets('renders localized tables in $locale with ${mode.name} theme', (tester) async {
        tester.platformDispatcher.localeTestValue = Locale(locale);
        addTearDown(tester.platformDispatcher.clearLocaleTestValue);
        SharedPreferences.setMockInitialValues({'theme_mode': mode.name});
        GetIt.I.registerSingleton(await ThemeModeModel.create());

        await tester.pumpWidget(const AdminUiApp());
        await tester.pumpAndSettle();

        final source = _TableSource();
        addTearDown(source.dispose);
        final navigator = tester.state<NavigatorState>(find.byType(Navigator).first)
          ..push<void>(
            MaterialPageRoute(
              builder: (context) => Scaffold(
                body: PaginatedDataTable2(
                  columns: const [DataColumn2(label: Text('Items'))],
                  source: source,
                  rowsPerPage: 2,
                  availableRowsPerPage: const [2],
                  showFirstLastButtons: true,
                ),
              ),
            ),
          );
        await tester.pumpAndSettle();

        final tableContext = tester.element(find.byType(PaginatedDataTable2));
        final expectedTheme = mode == ThemeMode.dark ? darkTheme : lightTheme;
        expect(Theme.of(tableContext).colorScheme, expectedTheme.colorScheme);
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
