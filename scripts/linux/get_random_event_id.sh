#!/bin/bash

while true; do
    # Generate a random number in the range [0, 899999]
    NUMBER=$(( $(dd if=/dev/urandom bs=3 count=1 2>/dev/null | od -An -i | tr -d '[:space:]') % 900000 ))

    # Adjust the number to the range [100000, 999999]
    NUMBER=$(( NUMBER + 100000 ))

    echo $NUMBER
    break
done
