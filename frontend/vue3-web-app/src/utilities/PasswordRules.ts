
export let passwordRules = {
    required: (value: string) => !!value || 'Required',
    hasUpper: (value: string) => /[A-Z]/.test(value) || 'Requires an uppercase letter',
    hasLower: (value: string) => /[a-z]/.test(value) || 'Requires a lowercase letter',
    hasDigit: (value: string) => /\d/.test(value) || 'Requires a digit',
    hasSymbol: (value: string) => /[^a-zA-Z0-9]/.test(value) || 'Requires a symbol',
    min: (value: string) => value.length >= 8 || 'Min 8 characters',
    confirmMatch: (value: string, valueMatch: string) => value === valueMatch || 'Passwords do not match'
}
