import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';

import '../constants.dart';

class AppTitle extends StatelessWidget {
  const AppTitle({super.key});

  @override
  Widget build(BuildContext context) {
    const textStyle = TextStyle(fontSize: 25);

    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        SvgPicture.asset(
          'assets/logo.svg',
          width: 30,
          height: 30,
        ),
        Gaps.w8,
        Text.rich(
          TextSpan(
            children: [
              TextSpan(text: 'enmeshed', style: textStyle.copyWith(fontWeight: FontWeight.bold)),
              const TextSpan(text: ' Admin UI', style: textStyle),
            ],
          ),
        ),
      ],
    );
  }
}
