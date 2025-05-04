import 'dart:convert';

List<CountrySummary> countrySummaryFromJson(String str) =>
    (json.decode(str) as List<dynamic>)
        .map((e) => CountrySummary.fromJson(e as Map<String, dynamic>))
        .toList();

String countrySummaryToJson(List<CountrySummary> data) =>
    json.encode(List<dynamic>.from(data.map((x) => x.toJson())));

class CountrySummary {
  final String? countryCode;
  final String? countryName;
  final List<String>?
  phoneCodes; // Assuming phoneCodes is a single string like "+1, +44"
  final String? capital;
  final double? averagePopulation;
  final Currency? currency;
  final String? flag; // Assuming flag is a URL or identifier string
  final double? averageExchangeRate;
  final DateTime? startDate;
  final DateTime? endDate;

  CountrySummary({
    this.countryCode,
    this.countryName,
    this.phoneCodes,
    this.capital,
    this.averagePopulation,
    this.currency,
    this.flag,
    this.averageExchangeRate,
    this.startDate,
    this.endDate,
  });

  factory CountrySummary.fromJson(Map<String, dynamic> json) {
    // Helper function for safe number parsing
    double? parseDouble(dynamic value) {
      if (value == null) return null;
      if (value is double) return value;
      if (value is int) return value.toDouble();
      if (value is String) return double.tryParse(value);
      return null;
    }

    // Helper function for safe date parsing
    DateTime? parseDateTime(dynamic value) {
      if (value == null || value is! String) return null;
      DateTime? dt = DateTime.tryParse(value);
      // Add other formats if needed
      return dt;
    }

    // Helper function for safe list of strings parsing
    List<String>? parseStringList(dynamic value) {
      if (value == null || value is! List) return null;
      // Ensure all elements in the list are strings
      return List<String>.from(value.map((item) => item.toString()));
    }

    return CountrySummary(
      countryCode: json["countryCode"] as String?,
      countryName: json["countryName"] as String?,
      phoneCodes: parseStringList(json["phoneCodes"]), // Updated parsing logic
      capital: json["capital"] as String?,
      averagePopulation: parseDouble(json["averagePopulation"]),
      currency:
          json["currency"] == null || json["currency"] is! Map<String, dynamic>
              ? null
              : Currency.fromJson(json["currency"] as Map<String, dynamic>),
      flag: json["flag"] as String?,
      averageExchangeRate: parseDouble(json["averageExchangeRate"]),
      startDate: parseDateTime(json["startDate"]),
      endDate: parseDateTime(json["endDate"]),
    );
  }

  Map<String, dynamic> toJson() => {
    "countryCode": countryCode,
    "countryName": countryName,
    "phoneCodes": phoneCodes, // Directly use the list
    "capital": capital,
    "averagePopulation": averagePopulation,
    "currency": currency?.toJson(),
    "flag": flag,
    "averageExchangeRate": averageExchangeRate,
    "startDate": startDate?.toIso8601String(),
    "endDate": endDate?.toIso8601String(),
  };

  // Helper to format population (example)
  String get formattedPopulation {
    if (averagePopulation == null) return 'N/A';
    if (averagePopulation! >= 1e9) {
      return '${(averagePopulation! / 1e9).toStringAsFixed(1)}B';
    } else if (averagePopulation! >= 1e6) {
      return '${(averagePopulation! / 1e6).toStringAsFixed(1)}M';
    } else if (averagePopulation! >= 1e3) {
      return '${(averagePopulation! / 1e3).toStringAsFixed(1)}K';
    }
    return averagePopulation!.toStringAsFixed(0);
  }

  // Helper to format exchange rate
  String get formattedExchangeRate {
    if (averageExchangeRate == null || currency?.symbol == null) {
      return 'N/A'; // Not enough data to format
    }
    // Assuming EUR (€) as the base currency for display purposes
    final baseSymbol = '€';
    final targetSymbol = currency!.symbol!;
    // Format rate to 3 decimal places
    final rateFormatted = averageExchangeRate!.toStringAsFixed(3);

    // Construct the desired string format
    return '$baseSymbol${1} = $targetSymbol$rateFormatted';
  }
}

class Currency {
  final String? code;
  final String? name;
  final String? symbol;

  Currency({this.code, this.name, this.symbol});

  factory Currency.fromJson(Map<String, dynamic> json) =>
      Currency(code: json["code"], name: json["name"], symbol: json["symbol"]);

  Map<String, dynamic> toJson() => {
    "code": code,
    "name": name,
    "symbol": symbol,
  };
}
