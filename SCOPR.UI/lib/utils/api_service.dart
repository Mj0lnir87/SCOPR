import 'dart:convert';
import 'dart:io';
import 'package:http/http.dart' as http;
import 'package:intl/intl.dart'; // Import for date formatting
import '../data/models/country_summary.dart';

// Custom HttpClient to bypass certificate validation (FOR DEVELOPMENT ONLY)
class DevHttpOverrides extends HttpOverrides {
  @override
  HttpClient createHttpClient(SecurityContext? context) {
    return super.createHttpClient(context)
      ..badCertificateCallback =
          (X509Certificate cert, String host, int port) => true;
  }
}

class ApiService {
  static const String _baseUrl = "https://localhost:50523/api";
  static final _apiDateFormatter = DateFormat("yyyy-MM-dd");

  static Future<List<CountrySummary>> fetchCountrySummaries({
    DateTime? startDate,
    DateTime? endDate,
  }) async {
    HttpOverrides.global = DevHttpOverrides(); // Use Development override

    var uri = Uri.parse('$_baseUrl/Countries/summary');
    Map<String, String> queryParams = {};
    if (startDate != null) {
      queryParams['startDate'] = _apiDateFormatter.format(startDate);
    }
    if (endDate != null) {
      queryParams['endDate'] = _apiDateFormatter.format(endDate);
    }
    if (queryParams.isNotEmpty) {
      uri = uri.replace(queryParameters: queryParams);
    }

    Map<String, String>? headers = {
      'Content-Type': 'application/json; charset=UTF-8', // Correct Content-Type
      'Access-Control-Allow-Origin':
          '*', // Server-side configuration is the proper fix
    };

    try {
      final response = await http.get(uri, headers: headers);

      if (response.statusCode == 200) {
        // Helper function to parse the list data
        List<CountrySummary> parseSummaries(List<dynamic> data) {
          try {
            return data
                .map((e) => CountrySummary.fromJson(e as Map<String, dynamic>))
                .toList();
          } catch (e) {
            print("Error parsing individual summary item: $e");
            // Decide how to handle partially successful parsing if needed
            // For now, rethrow or return an empty list / throw specific error
            throw Exception("Failed to parse country summary data: $e");
          }
        }

        dynamic responseData = response.body;
        print('Response body type: ${responseData.runtimeType}'); // Debug print

        if (responseData is String) {
          // Decode if it's a string
          final decodedData = json.decode(responseData);
          if (decodedData is List) {
            return parseSummaries(decodedData);
          } else {
            throw Exception(
              'Expected a JSON array (List) but received type ${decodedData.runtimeType}',
            );
          }
        } else if (responseData is List) {
          // Use directly if it's already a list (common in web)
          return parseSummaries(List<dynamic>.from(responseData));
        } else {
          // Handle unexpected type
          throw Exception(
            'Unexpected response body type: ${responseData.runtimeType}',
          );
        }
      } else {
        String responseBody =
            response.body.length > 200
                ? '${response.body.substring(0, 200)}...'
                : response.body;
        throw Exception(
          'Failed to load country summaries (Status code: ${response.statusCode}, Body: $responseBody)',
        );
      }
    } catch (e, stackTrace) {
      print('Error during API call: $e');
      print(
        'Stack trace: $stackTrace',
      ); // Print stack trace for detailed debugging
      // Rethrow a more specific exception or handle accordingly
      throw Exception('Error during API call: $e');
    }
  }
}
