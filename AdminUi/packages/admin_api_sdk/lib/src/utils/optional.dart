class Optional<T> {
  final T? value;

  const Optional(this.value);
  const Optional.absent() : value = null;
}
