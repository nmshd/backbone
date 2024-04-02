import 'package:admin_ui/theme/colors/color_schemes.dart';
import 'package:admin_ui/theme/colors/custom_colors.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'home/home.dart';
import 'screens/screens.dart';
import 'setup/setup_desktop.dart' if (dart.library.html) 'setup/setup_web.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();

  await setup();

  runApp(const AdminUiApp());
}

final _rootNavigatorKey = GlobalKey<NavigatorState>();
final _shellNavigatorKey = GlobalKey<NavigatorState>();

final _router = GoRouter(
  initialLocation: '/splash',
  navigatorKey: _rootNavigatorKey,
  routes: [
    GoRoute(
      parentNavigatorKey: _rootNavigatorKey,
      path: '/splash',
      builder: (context, state) => const SplashScreen(),
    ),
    GoRoute(
      parentNavigatorKey: _rootNavigatorKey,
      path: '/login',
      builder: (context, state) => const LoginScreen(),
    ),
    ShellRoute(
      navigatorKey: _shellNavigatorKey,
      parentNavigatorKey: _rootNavigatorKey,
      routes: [
        GoRoute(
          parentNavigatorKey: _shellNavigatorKey,
          path: '/dashboard',
          pageBuilder: (context, state) => const NoTransitionPage(child: Dashboard()),
        ),
        GoRoute(
          parentNavigatorKey: _shellNavigatorKey,
          path: '/identities',
          pageBuilder: (context, state) => const NoTransitionPage(child: Identities()),
        ),
        GoRoute(
          parentNavigatorKey: _shellNavigatorKey,
          path: '/tiers',
          pageBuilder: (context, state) => const NoTransitionPage(child: Tiers()),
        ),
        GoRoute(
          parentNavigatorKey: _shellNavigatorKey,
          path: '/clients',
          pageBuilder: (context, state) => const NoTransitionPage(child: Clients()),
        ),
      ],
      builder: (context, state, child) => HomeScreen(
        location: state.fullPath!,
        child: child,
      ),
    ),
  ],
);

class AdminUiApp extends StatelessWidget {
  const AdminUiApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp.router(
      theme: ThemeData(
        useMaterial3: true,
        colorScheme: lightColorScheme,
        extensions: [lightCustomColors],
      ),
      darkTheme: ThemeData(
        useMaterial3: true,
        colorScheme: darkColorScheme,
        extensions: [darkCustomColors],
      ),
      debugShowCheckedModeBanner: false,
      routerConfig: _router,
    );
  }
}
