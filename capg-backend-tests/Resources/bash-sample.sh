#!/bin/bash

# Bash Script Sample: User Greeting and Number Check

# 1. Define variables
echo "=== Bash Script Sample ==="

# 2. Get user input
read -p "Enter your name: " username
read -p "Enter a number: " userNumber

# 3. Use conditional logic (If/Else)
if [ $userNumber -eq 0 ]; then
    echo "You entered zero."
elif [ $userNumber -gt 0 ]; then
    echo "$username, you entered a positive number: $userNumber"
else
    echo "$username, you entered a negative number."
fi

# 4. Use a loop (For Loop)
echo "Counting down from 3:"
for i in 3 2 1; do
    echo "$i..."
    sleep 1
done
echo "Done!"

# 5. String manipulation
greeting="Hello, $username"
echo "Final Greeting: ${greeting}"   