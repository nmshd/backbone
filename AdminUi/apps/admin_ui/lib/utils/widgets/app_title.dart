import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';

class AppTitle extends StatelessWidget {
  const AppTitle({super.key});

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        SvgPicture.asset(
          'assets/logo.svg',
          width: 30,
          height: 30,
        ),
        const SizedBox(width: 10),
        RichText(
          text: TextSpan(
            children: [
              TextSpan(
                text: 'enmeshed',
                style: TextStyle(
                  fontWeight: FontWeight.bold,
                  color: Theme.of(context).colorScheme.scrim,
                  fontSize: 25,
                ),
              ),
              TextSpan(
                text: ' Admin UI',
                style: TextStyle(
                  color: Theme.of(context).colorScheme.scrim,
                  fontSize: 25,
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }
}
