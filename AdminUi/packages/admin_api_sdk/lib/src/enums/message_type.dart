enum MessageType {
  incoming('Incoming'),
  outgoing('Outgoing');

  final String name;
  const MessageType(this.name);
}
