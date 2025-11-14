enum MessageType {
  incoming,
  outgoing
  ;

  String get name => switch (this) {
    .incoming => 'Incoming',
    .outgoing => 'Outgoing',
  };
}
