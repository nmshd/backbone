import 'package:admin_ui/theme/colors/color_schemes.dart';
import 'package:admin_ui/theme/colors/custom_color.dart';
import 'package:admin_ui/theme/theme_manager.dart';
import 'package:dynamic_color/dynamic_color.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'home/home.dart';
import 'pages/pages.dart';
import 'setup/setup_desktop.dart' if (dart.library.html) 'setup/setup_web.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();

  await setup();

  runApp(const MainApp());
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

class MainApp extends StatelessWidget {
  const MainApp({super.key});

  @override
  Widget build(BuildContext context) {
    return DynamicColorBuilder(
      builder: (ColorScheme? lightDynamic, ColorScheme? darkDynamic) {
        ColorScheme lightScheme;
        ColorScheme darkScheme;

        if (lightDynamic != null && darkDynamic != null) {
          lightScheme = lightDynamic.harmonized();
          lightCustomColors = lightCustomColors.harmonized(lightScheme);

          darkScheme = darkDynamic.harmonized();
          darkCustomColors = darkCustomColors.harmonized(darkScheme);
        } else {
          lightScheme = lightColorScheme;
          darkScheme = darkColorScheme;
        }

        return MaterialApp.router(
          theme: ThemeData(
            useMaterial3: true,
            colorScheme: lightScheme,
            extensions: [lightCustomColors],
          ),
          darkTheme: ThemeData(
            useMaterial3: true,
            colorScheme: darkScheme,
            extensions: [darkCustomColors],
          ),
          debugShowCheckedModeBanner: false,
          routerConfig: _router,
        );
      },
    );
  }
}
