import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';

class TiersOverviewList extends StatefulWidget {
  const TiersOverviewList({super.key});

  @override
  State<TiersOverviewList> createState() => _TiersOverviewListState();
}

class _TiersOverviewListState extends State<TiersOverviewList> {
  final TextEditingController _tierNameController = TextEditingController();

  late ScrollController _scrollController;
  late List<TierOverview> _tiers;
  late String _errorMessage;

  bool _isLoading = false;

  double _boxWidth = 700;

  @override
  void initState() {
    super.initState();
    _scrollController = ScrollController();
    _errorMessage = '';
    _tiers = [];
    loadTiers();
  }

  @override
  void dispose() {
    super.dispose();
    _tierNameController.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(top: 20),
      child: Column(
        children: [
          Container(
            height: 100,
            width: _boxWidth,
            decoration: BoxDecoration(
              color: Theme.of(context).colorScheme.primary,
              boxShadow: const [
                BoxShadow(
                  color: Colors.black26,
                  blurRadius: 4,
                ),
              ],
            ),
            child: Padding(
              padding: const EdgeInsets.only(left: 15, top: 20),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Tiers',
                    style: TextStyle(color: Theme.of(context).colorScheme.onPrimary, fontSize: 30),
                  ),
                  Text(
                    'A list of all Tiers',
                    style: TextStyle(color: Theme.of(context).colorScheme.onPrimary, fontSize: 13),
                  ),
                ],
              ),
            ),
          ),
          Padding(
            padding: const EdgeInsets.symmetric(vertical: 8),
            child: SizedBox(
              width: _boxWidth,
              child: Row(
                mainAxisAlignment: MainAxisAlignment.end,
                children: [
                  IconButton(
                    icon: const Icon(Icons.add),
                    color: Theme.of(context).colorScheme.onPrimary,
                    style: ButtonStyle(
                      backgroundColor: MaterialStateProperty.all<Color>(Theme.of(context).colorScheme.primary),
                      foregroundColor: MaterialStateProperty.all<Color>(Theme.of(context).colorScheme.onPrimary),
                    ),
                    onPressed: () {
                      _showAddTierDialog(context: context);
                    },
                  ),
                ],
              ),
            ),
          ),
          if (_isLoading) const CircularProgressIndicator(),
          if (!_isLoading)
            Expanded(
              child: Card(
                elevation: 1,
                child: SizedBox(
                  width: _boxWidth,
                  child: DataTable2(
                    isVerticalScrollBarVisible: true,
                    scrollController: _scrollController,
                    showCheckboxColumn: false,
                    columns: const [
                      DataColumn2(label: Text('Name'), size: ColumnSize.L),
                      DataColumn2(label: Text('Number of Identities'), size: ColumnSize.L),
                    ],
                    rows: _tiers
                        .map(
                          (tier) => DataRow2(
                            onLongPress: () {},
                            cells: [
                              DataCell(Text(tier.name)),
                              DataCell(Text('${tier.numberOfIdentities}')),
                            ],
                          ),
                        )
                        .toList(),
                  ),
                ),
              ),
            ),
        ],
      ),
    );
  }

  void _showAddTierDialog({required BuildContext context}) {
    showDialog<void>(
      context: context,
      builder: (BuildContext dialogContext) {
        return StatefulBuilder(
          builder: (context, setStateDialog) {
            return AlertDialog(
              title: const Text('Create Tier'),
              content: SingleChildScrollView(
                child: ListBody(
                  children: <Widget>[
                    const Text('Please fill the form below to create your Tier'),
                    TextField(
                      controller: _tierNameController,
                      decoration: const InputDecoration(
                        labelText: 'Name',
                      ),
                    ),
                    if (_errorMessage.isNotEmpty)
                      Padding(
                        padding: const EdgeInsets.only(top: 8),
                        child: Text(
                          _errorMessage,
                          style: TextStyle(color: Theme.of(context).colorScheme.error),
                        ),
                      ),
                  ],
                ),
              ),
              actions: <Widget>[
                TextButton(
                  child: const Text('Add'),
                  onPressed: () {
                    final name = _tierNameController.text;
                    if (name.isNotEmpty) {
                      createTier(name).then((response) {
                        if (mounted) {
                          setState(() {
                            if (response.hasData) {
                              loadTiers();
                              Navigator.of(dialogContext).pop();
                              setState(() => _isLoading = true);
                              _showSuccessSnackbar('Tier was created successfully.');
                            } else {
                              setStateDialog(() => _errorMessage = response.error.message);
                            }
                          });
                        }
                      }).whenComplete(() => _isLoading = false);
                    } else {
                      setStateDialog(() => _errorMessage = 'Name cannot be empty.');
                    }
                  },
                ),
                TextButton(
                  child: const Text('Cancel'),
                  onPressed: () {
                    _tierNameController.text = '';
                    _errorMessage = '';
                    Navigator.of(dialogContext).pop();
                  },
                ),
              ],
            );
          },
        );
      },
    );
  }

  Future<ApiResponse<Tier>> createTier(String name) async {
    final response = await GetIt.I.get<AdminApiClient>().tiers.createTier(name: name);
    return response;
  }

  void _showSuccessSnackbar(String message) {
    final snackBar = SnackBar(
      content: Text(
        message,
        style: const TextStyle(color: Colors.white),
      ),
      backgroundColor: Colors.green,
      duration: const Duration(seconds: 3),
    );
    ScaffoldMessenger.of(context).showSnackBar(snackBar);
  }

  Future<void> loadTiers() async {
    final response = await GetIt.I.get<AdminApiClient>().tiers.getTiers();

    setState(() {
      _tiers = response.data;
    });
  }
}
