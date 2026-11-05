class Optional<T> {
  final T? value;

  const new(this.value);
  const new absent() : value = null;
}
