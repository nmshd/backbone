enum MessageType {
  incoming,
  outgoing;

  String get name => switch (this) {
    MessageType.incoming => 'Incoming',
    MessageType.outgoing => 'Outgoing',
  };
}
