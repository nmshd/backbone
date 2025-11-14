import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';

class AppTitle extends StatelessWidget {
  final EdgeInsetsGeometry? padding;

  const AppTitle({this.padding, super.key});

  @override
  Widget build(BuildContext context) {
    const textStyle = TextStyle(fontSize: 25);

    final row = Row(
      mainAxisSize: .min,
      spacing: 8,
      children: [
        SvgPicture.asset('assets/logo.svg', width: 30, height: 30),
        Text.rich(
          TextSpan(
            children: [
              TextSpan(
                text: 'enmeshed',
                style: textStyle.copyWith(fontWeight: .bold),
              ),
              const TextSpan(text: ' Backbone Admin UI', style: textStyle),
            ],
          ),
        ),
      ],
    );

    if (padding == null) return row;

    return Padding(padding: padding!, child: row);
  }
}
