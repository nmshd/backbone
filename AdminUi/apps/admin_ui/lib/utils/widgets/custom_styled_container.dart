import 'package:flutter/material.dart';

class CustomStyledContainer extends StatelessWidget {
  final Widget child;
  final double width;
  final EdgeInsetsGeometry padding;

  const CustomStyledContainer({
    required this.child,
    this.width = 400.0,
    this.padding = const EdgeInsets.all(25),
    super.key,
  });

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: width,
      child: Padding(
        padding: padding,
        child: child,
      ),
    );
  }
}
