import 'package:flutter/material.dart';
import 'screens/home_screen.dart'; // Import the home screen

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'SCOPR UI',
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(
          seedColor: const Color.fromARGB(255, 58, 183, 85),
        ),
        useMaterial3: true, // Recommended for new projects
      ),
      home: const MyHomePage(
        title: 'SCOPR UI Test',
      ), // Use MyHomePage from home_screen.dart
    );
  }
}
