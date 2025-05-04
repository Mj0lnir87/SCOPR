import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../data/models/country_summary.dart';
import '../utils/api_service.dart';
import '../utils/export_service.dart';
import '../widgets/data_table_widget.dart';
import '../widgets/mobile_list_view.dart';

class MyHomePage extends StatefulWidget {
  const MyHomePage({super.key, required this.title});
  final String title;
  @override
  State<MyHomePage> createState() => _MyHomePageState();
}

class _MyHomePageState extends State<MyHomePage> {
  late Future<List<CountrySummary>> _countriesFuture;
  List<CountrySummary> _countries = [];

  // --- Date Range State ---
  DateTime? _startDate;
  DateTime? _endDate;
  final DateFormat _dateFormatter = DateFormat('yyyy-MM-dd'); // Formatter

  // --- UI Interaction State ---
  List<bool> _expandedItems = [];
  List<bool> _expandedPhoneCodes = [];
  List<bool> _selectedRows = [];
  bool? _isAllSelected = false;

  @override
  void initState() {
    super.initState();
    // Set initial dates (e.g., last 30 days) or leave null for no initial filter
    _endDate = DateTime.now();
    _startDate = _endDate!.subtract(const Duration(days: 30));
    _loadCountries(); // Initial load
  }

  // --- Date Picker Methods ---
  Future<void> _selectStartDate(BuildContext context) async {
    final DateTime now = DateTime.now();
    final DateTime? picked = await showDatePicker(
      context: context,
      initialDate:
          _startDate ??
          now.subtract(const Duration(days: 30)), // Sensible initial
      firstDate: DateTime(2000), // Or your earliest possible date
      // lastDate cannot be in the future, and cannot be after the current _endDate
      lastDate: _endDate != null && _endDate!.isBefore(now) ? _endDate! : now,
    );
    if (picked != null && picked != _startDate) {
      // Ensure picked start date is not after end date (double check)
      if (_endDate != null && picked.isAfter(_endDate!)) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Start date cannot be after the end date.'),
          ),
        );
        return; // Don't update if invalid
      }
      setState(() {
        _startDate = picked;
      });
      _loadCountries(); // Reload data with new date
    }
  }

  Future<void> _selectEndDate(BuildContext context) async {
    final DateTime now = DateTime.now();
    final DateTime? picked = await showDatePicker(
      context: context,
      initialDate: _endDate ?? now, // Sensible initial
      // firstDate cannot be before the current _startDate
      firstDate: _startDate ?? DateTime(2000), // Or your earliest possible date
      // lastDate cannot be in the future
      lastDate: now,
    );
    if (picked != null && picked != _endDate) {
      // Ensure picked end date is not before start date
      if (_startDate != null && picked.isBefore(_startDate!)) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('End date cannot be before the start date.'),
          ),
        );
        return; // Don't update if invalid
      }
      setState(() {
        _endDate = picked;
      });
      _loadCountries(); // Reload data with new date
    }
  }

  void _loadCountries() {
    // Pass selected dates to the API service
    _countriesFuture = ApiService.fetchCountrySummaries(
      startDate: _startDate,
      endDate: _endDate,
    );
    // Reset state lists when reloading
    setState(() {
      _countries = []; // Clear previous data while loading
      _expandedItems = [];
      _expandedPhoneCodes = [];
      _selectedRows = [];
      _isAllSelected = false;
    });
    _countriesFuture
        .then((data) {
          setState(() {
            _countries = data;
            _expandedItems = List.generate(_countries.length, (_) => false);
            _expandedPhoneCodes = List.generate(
              _countries.length,
              (_) => false,
            );
            _selectedRows = List.generate(_countries.length, (_) => false);
            _isAllSelected = false;
          });
        })
        .catchError((error) {
          print("Error loading countries: $error");
          if (mounted) {
            // Check if widget is still in the tree
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(content: Text('Failed to load countries: $error')),
            );
          }
          // Ensure state lists are empty on error
          setState(() {
            _countries = [];
            _expandedItems = [];
            _expandedPhoneCodes = [];
            _selectedRows = [];
            _isAllSelected = false;
          });
        });
  }

  // --- Interaction Handlers (remain largely the same, operate on initialized lists) ---
  void _handleExpansionChanged(int index) {
    if (index < _expandedItems.length) {
      setState(() {
        _expandedItems[index] = !_expandedItems[index];
      });
    }
  }

  void _handleSelectionChanged(int index, bool value) {
    if (index < _selectedRows.length) {
      setState(() {
        _selectedRows[index] = value;
        if (_selectedRows.every((isSelected) => isSelected)) {
          _isAllSelected = true;
        } else if (_selectedRows.any((isSelected) => isSelected)) {
          _isAllSelected = null;
        } else {
          _isAllSelected = false;
        }
      });
    }
  }

  void _handlePhoneCodeExpansionChanged(int index) {
    setState(() {
      // Ensure index is valid
      if (index >= 0 && index < _expandedPhoneCodes.length) {
        _expandedPhoneCodes[index] = !_expandedPhoneCodes[index];
      }
    });
  }

  void _handleSelectAllChanged(bool? value) {
    final bool currentlyAllSelected = _isAllSelected == true;
    final bool selectAll = !currentlyAllSelected;
    setState(() {
      _isAllSelected = selectAll;
      _selectedRows = List.generate(
        _countries.length,
        (_) => selectAll,
      ); // Update based on _countries length
    });
  }

  String _getDisplayPhoneCode(int index, List<String>? phoneCodes) {
    // Join the list into a single string, handling null
    final displayString =
        phoneCodes?.join(', ') ??
        ''; // Join with comma and space, default to empty string if null

    if (index < _expandedPhoneCodes.length &&
        (displayString.length <= 50 || _expandedPhoneCodes[index])) {
      // If length is short OR the item is expanded, show the full string
      return displayString;
    } else if (displayString.length > 50) {
      // If length is long AND the item is not expanded, truncate
      return '${displayString.substring(0, 50)}...';
    } else {
      // Default case (shouldn't be reached often with the logic above, but safe to include)
      return displayString;
    }
  }

  @override
  Widget build(BuildContext context) {
    final screenWidth = MediaQuery.of(context).size.width;
    final bool isMobileSize = screenWidth < 600;

    return Scaffold(
      appBar: AppBar(
        // ... (title, actions) ...
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
        title: Text(widget.title),
        actions: [
          IconButton(
            icon: Icon(Icons.refresh),
            onPressed: _loadCountries,
            tooltip: 'Refresh Data',
          ),
        ],
      ),
      body: Column(
        // Wrap body content in a Column
        children: [
          // --- Date Range Selection UI ---
          Padding(
            padding: const EdgeInsets.all(8.0),
            child: Wrap(
              // Use Wrap for better responsiveness
              spacing: 8.0, // Horizontal space between elements
              runSpacing: 4.0, // Vertical space between lines
              alignment: WrapAlignment.center,
              children: [
                ElevatedButton.icon(
                  icon: Icon(Icons.calendar_today),
                  label: Text(
                    _startDate == null
                        ? 'Start Date'
                        : _dateFormatter.format(_startDate!),
                  ),
                  onPressed: () => _selectStartDate(context),
                ),
                ElevatedButton.icon(
                  icon: Icon(Icons.calendar_today),
                  label: Text(
                    _endDate == null
                        ? 'End Date'
                        : _dateFormatter.format(_endDate!),
                  ),
                  onPressed: () => _selectEndDate(context),
                ),
              ],
            ),
          ),
          // --- Data Display Area ---
          Expanded(
            // Make FutureBuilder take remaining space
            child: FutureBuilder<List<CountrySummary>>(
              future: _countriesFuture,
              builder: (context, snapshot) {
                if (snapshot.connectionState == ConnectionState.waiting) {
                  return const Center(child: CircularProgressIndicator());
                } else if (snapshot.hasError) {
                  return Center(child: Text('Error: ${snapshot.error}'));
                } else if (!snapshot.hasData || snapshot.data!.isEmpty) {
                  return const Center(
                    child: Text(
                      'No countries found for the selected date range.',
                    ),
                  );
                } else {
                  // Data loaded successfully
                  // Safety check for state list lengths (important after reload)
                  if (_selectedRows.length != _countries.length) {
                    _expandedItems = List.generate(
                      _countries.length,
                      (_) => false,
                    );
                    _expandedPhoneCodes = List.generate(
                      _countries.length,
                      (_) => false,
                    );
                    _selectedRows = List.generate(
                      _countries.length,
                      (_) => false,
                    );
                    _isAllSelected = false;
                    // Return a temporary loading indicator while state is reset
                    // This prevents errors trying to build lists with mismatched lengths
                    WidgetsBinding.instance.addPostFrameCallback((_) {
                      if (mounted) {
                        setState(() {}); // Trigger rebuild after state is set
                      }
                    });
                    return const Center(child: CircularProgressIndicator());
                  }

                  return Center(
                    // Keep Center or remove if Column handles alignment
                    child: Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: <Widget>[
                        if (!isMobileSize)
                          Expanded(
                            child: Padding(
                              padding: const EdgeInsets.fromLTRB(
                                16.0,
                                0,
                                16.0,
                                16.0,
                              ), // Adjust padding
                              child: ExpandableDataTable(
                                data: _countries,
                                expandedItems: _expandedPhoneCodes,
                                selectedRows: _selectedRows,
                                isAllSelected: _isAllSelected,
                                onExpansionChanged: _handleExpansionChanged,
                                onPhoneCodeExpansionChanged:
                                    _handlePhoneCodeExpansionChanged,
                                onSelectionChanged: _handleSelectionChanged,
                                onSelectAllChanged: _handleSelectAllChanged,
                                getDisplayPhoneCode: _getDisplayPhoneCode,
                              ),
                            ),
                          ),
                        if (isMobileSize)
                          Expanded(
                            child: MobileDataList(
                              data: _countries,
                              expandedItems: _expandedItems,
                              expandedPhoneCodes: _expandedPhoneCodes,
                              selectedRows: _selectedRows,
                              isAllSelected: _isAllSelected,
                              onExpansionChanged: _handleExpansionChanged,
                              onPhoneCodeExpansionChanged:
                                  _handlePhoneCodeExpansionChanged,
                              onSelectionChanged: _handleSelectionChanged,
                              onSelectAllChanged: _handleSelectAllChanged,
                              getDisplayPhoneCode: _getDisplayPhoneCode,
                            ),
                          ),
                      ],
                    ),
                  );
                }
              },
            ),
          ),
        ],
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: () {
          if (_countries.isEmpty) {
            // Don't show export if no data
            ScaffoldMessenger.of(
              context,
            ).showSnackBar(const SnackBar(content: Text('No data to export.')));
            return;
          }
          showModalBottomSheet(
            context: context,
            builder: (BuildContext context) {
              return SafeArea(
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: <Widget>[
                    ListTile(
                      leading: const Icon(Icons.picture_as_pdf),
                      title: const Text('Export to PDF'),
                      onTap: () async {
                        Navigator.pop(context);
                        try {
                          // Pass CountrySummary list to export service
                          await ExportService.exportToPdf(
                            _countries,
                            _selectedRows,
                            _startDate,
                            _endDate,
                          );
                          print(
                            "PDF Export function finished",
                          ); // 2. Add print statement here
                          // Optional: Show success message
                          if (mounted) {
                            // Check if widget is still mounted
                            ScaffoldMessenger.of(context).showSnackBar(
                              const SnackBar(
                                content: Text(
                                  'PDF export started (check downloads/share).',
                                ),
                              ),
                            );
                          }
                        } catch (e, s) {
                          print("Error during PDF export: $e"); // 3. Log error
                          print("Stack trace: $s"); // 3. Log stack trace
                          // Optional: Show error message to user
                          if (mounted) {
                            // Check if widget is still mounted
                            ScaffoldMessenger.of(context).showSnackBar(
                              SnackBar(content: Text('PDF Export failed: $e')),
                            );
                          }
                        }
                      },
                    ),
                    ListTile(
                      leading: const Icon(Icons.table_chart),
                      title: const Text('Export to CSV'),
                      onTap: () async {
                        print("CSV Export tapped"); // 1. Add print statement
                        Navigator.pop(context); // Close bottom sheet
                        try {
                          // Pass CountrySummary list to export service
                          await ExportService.exportToCsv(
                            _countries,
                            _selectedRows,
                            _startDate,
                            _endDate,
                          );
                          print(
                            "CSV Export function finished",
                          ); // 2. Add print statement
                          // Optional: Show success message
                          if (mounted) {
                            // Check if widget is still mounted
                            ScaffoldMessenger.of(context).showSnackBar(
                              const SnackBar(
                                content: Text(
                                  'CSV export started (check downloads/share).',
                                ),
                              ),
                            );
                          }
                        } catch (e, s) {
                          print("Error during CSV export: $e"); // 3. Log error
                          print("Stack trace: $s"); // 3. Log stack trace
                          // Optional: Show error message to user
                          if (mounted) {
                            // Check if widget is still mounted
                            ScaffoldMessenger.of(context).showSnackBar(
                              SnackBar(content: Text('CSV Export failed: $e')),
                            );
                          }
                        }
                      },
                    ),
                  ],
                ),
              );
            },
          );
        },
        backgroundColor: Theme.of(context).colorScheme.primary,
        tooltip: 'Export',
        child: const Icon(Icons.share),
      ),
    );
  }
}
