export const validPositiveNumber: RegExp = /^\d+(\.\d+)?$/;
export const validNumber: RegExp = /^-?\d+(\.\d+)?$/;
export const validNegativeNumber: RegExp = /^-+\d+(\.\d+)?$/;
export const validNegativeNumberOrZero: RegExp = /^-?\d+(\.\d+)?$/;
export const createValidNumber: RegExp = /[^-\d.]/g;
export const passwordRegex = /^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[^A-Za-z0-9]).{8,}$/;
export const validEmail: RegExp = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
