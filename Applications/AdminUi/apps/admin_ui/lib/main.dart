import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:flutter_gen/gen_l10n/app_localizations.dart';
import 'package:go_router/go_router.dart';
import 'package:logger/logger.dart';
import 'package:watch_it/watch_it.dart';

import 'core/models/models.dart';
import 'core/theme/theme.dart';
import 'home/home.dart';
import 'screens/screens.dart';
import 'setup/setup_desktop.dart' if (dart.library.html) 'setup/setup_web.dart' if (dart.library.js_interop) 'setup/setup_web.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();

  GetIt.I.registerSingleton(Logger());
  GetIt.I.registerSingleton(await ThemeModeModel.create());

  await setup();

  GoRouter.optionURLReflectsImperativeAPIs = true;

  dataTableShowLogs = false;

  runApp(const AdminUiApp());
}

final _rootNavigatorKey = GlobalKey<NavigatorState>();
final _shellNavigatorKey = GlobalKey<NavigatorState>();

final _router = GoRouter(
  initialLocation: '/splash',
  navigatorKey: _rootNavigatorKey,
  routes: [
    GoRoute(path: '/index.html', redirect: (_, __) => '/splash'),
    GoRoute(
      parentNavigatorKey: _rootNavigatorKey,
      path: '/splash',
      builder: (context, state) => SplashScreen(redirect: state.uri.queryParameters['redirect']),
    ),
    GoRoute(
      parentNavigatorKey: _rootNavigatorKey,
      path: '/login',
      builder: (context, state) => LoginScreen(redirect: state.uri.queryParameters['redirect']),
    ),
    GoRoute(parentNavigatorKey: _rootNavigatorKey, path: '/error', builder: (context, state) => ErrorScreen(errorMessage: state.extra.toString())),
    ShellRoute(
      navigatorKey: _shellNavigatorKey,
      parentNavigatorKey: _rootNavigatorKey,
      redirect: (context, state) => GetIt.I.isRegistered<AdminApiClient>() ? null : '/splash?redirect=${Uri.encodeComponent(state.matchedLocation)}',
      routes: [
        GoRoute(
          parentNavigatorKey: _shellNavigatorKey,
          path: '/identities',
          pageBuilder: (context, state) => const NoTransitionPage(child: IdentitiesOverview()),
          routes: [
            GoRoute(
              parentNavigatorKey: _shellNavigatorKey,
              path: ':address',
              pageBuilder: (context, state) => NoTransitionPage(child: IdentityDetails(address: state.pathParameters['address']!)),
              routes: [
                GoRoute(
                  parentNavigatorKey: _shellNavigatorKey,
                  path: 'deletion-process-details/:deletionProcessId',
                  pageBuilder:
                      (context, state) => NoTransitionPage(
                        child: DeletionProcessDetails(
                          address: state.pathParameters['address']!,
                          deletionProcessId: state.pathParameters['deletionProcessId']!,
                        ),
                      ),
                ),
                GoRoute(
                  parentNavigatorKey: _shellNavigatorKey,
                  path: 'deletion-process-audit-logs',
                  pageBuilder:
                      (context, state) => NoTransitionPage(child: DeletionProcessAuditLogDetails(identityAddress: state.pathParameters['address']!)),
                ),
              ],
            ),
          ],
        ),
        GoRoute(
          parentNavigatorKey: _shellNavigatorKey,
          path: '/tiers',
          pageBuilder: (context, state) => const NoTransitionPage(child: TiersOverview()),
          routes: [
            GoRoute(
              parentNavigatorKey: _shellNavigatorKey,
              path: ':id',
              pageBuilder: (context, state) => NoTransitionPage(child: TierDetail(tierId: state.pathParameters['id']!)),
            ),
          ],
        ),
        GoRoute(
          parentNavigatorKey: _shellNavigatorKey,
          path: '/clients',
          pageBuilder: (context, state) => const NoTransitionPage(child: ClientsOverview()),
          routes: [
            GoRoute(
              parentNavigatorKey: _shellNavigatorKey,
              path: ':clientId',
              pageBuilder: (context, state) => NoTransitionPage(child: ClientDetails(clientId: state.pathParameters['clientId']!)),
            ),
          ],
        ),
      ],
      builder: (context, state, child) => HomeScreen(location: state.fullPath!, child: child),
    ),
  ],
);

class AdminUiApp extends StatelessWidget with WatchItMixin {
  const AdminUiApp({super.key});

  @override
  Widget build(BuildContext context) {
    final ThemeMode themeMode = watchValue((ThemeModeModel x) => x.themeMode);

    return MaterialApp.router(
      title: 'Admin UI',
      themeMode: themeMode,
      theme: ThemeData(useMaterial3: true, colorScheme: lightColorScheme, cardTheme: cardThemeLight, extensions: [lightCustomColors]),
      darkTheme: ThemeData(useMaterial3: true, colorScheme: darkColorScheme, cardTheme: cardThemeDark, extensions: [darkCustomColors]),
      debugShowCheckedModeBanner: false,
      routerConfig: _router,
      localizationsDelegates: AppLocalizations.localizationsDelegates,
      supportedLocales: AppLocalizations.supportedLocales,
    );
  }
}
