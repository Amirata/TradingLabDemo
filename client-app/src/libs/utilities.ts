import { DayOfWeek } from "./definitions";

export function camelToPascal(camelStr: string) {
  if (!camelStr) return ""; // Handle empty string or null input

  return camelStr.charAt(0).toUpperCase() + camelStr.slice(1);
}

export function capitalize(s: string) {
  return s ? s.charAt(0).toUpperCase() + s.slice(1).toLowerCase() : "";
}

export function textWithLineBreaks(text: string) {
  return text.split(/\r?\n/);
}

export const days: DayOfWeek[] = [
  "Monday",
  "Tuesday",
  "Wednesday",
  "Thursday",
  "Friday",
  "Saturday",
  "Sunday",
];

export function getEnumName<T extends object>(enumObj: T, value: T[keyof T]): keyof T | undefined {
  return Object.keys(enumObj).find(
      key => enumObj[key as keyof T] === value
  ) as keyof T | undefined;
}

export const formatDate = (dateString: string, showTime: boolean = false): string => {
  const options: Intl.DateTimeFormatOptions = {
    year: "numeric",
    month: "numeric",
    day: "numeric",
  };

  if (showTime) {
    Object.assign(options, {
      hour: "numeric",
      minute: "numeric",
      second: "numeric",
    });
  }

  return new Intl.DateTimeFormat(undefined, options).format(new Date(dateString));
};

export const formatNumber = (num: number): string => {
  if (isNaN(num)) return "";

  const roundedNum = num.toFixed(2); // Round to two decimal places
  const [integer, decimal] = roundedNum.split(".");

  const formattedInteger = new Intl.NumberFormat("en-US", {
    useGrouping: true,
  }).format(Number(integer.replace(/\s/g, "")));

  return `${formattedInteger}.${decimal}`;
};

// تابع فرمت تاریخ و زمان
export const formatDateTime=(date?: Date): string => {
  const targetDate = date || new Date(); // اگه تاریخ ندادیم، تاریخ فعلی رو بگیر

  const year = targetDate.getFullYear();
  const month = String(targetDate.getMonth() + 1).padStart(2, '0'); // +1 چون ماه از 0 شروع می‌شه
  const day = String(targetDate.getDate()).padStart(2, '0');
  const hours = String(targetDate.getHours()).padStart(2, '0');
  const minutes = String(targetDate.getMinutes()).padStart(2, '0');
  const seconds = String(targetDate.getSeconds()).padStart(2, '0');

  return `${year}/${month}/${day} ${hours}:${minutes}:${seconds}`;
}

export const formatDateTimeForm = (date?: Date): string => {
  const targetDate = date && !isNaN(date.getTime()) ? date : new Date();

  const year = targetDate.getFullYear();
  const month = String(targetDate.getMonth() + 1).padStart(2, "0");
  const day = String(targetDate.getDate()).padStart(2, "0");
  const hours = String(targetDate.getHours()).padStart(2, "0");
  const minutes = String(targetDate.getMinutes()).padStart(2, "0");
  const seconds = String(targetDate.getSeconds()).padStart(2, "0");

  // تغییر ترتیب به DD/MM/YYYY HH:MM:SS
  return `${day}/${month}/${year} ${hours}:${minutes}:${seconds}`;
};

export const timeToSeconds=(time: string): number =>{
  const [hours, minutes, seconds] = time.split(":").map(Number);
  return hours * 3600 + minutes * 60 + seconds;
}

  export const formatDateTimeForJson = (dateStr: string) => {
    if (!dateStr || dateStr === "DD/MM/YYYY __:__:__") return null; // یا "" بسته به نیاز سرور
    const [datePart, timePart] = dateStr.split(" ");
    const [day, month, year] = datePart.split("/");
    return `${year}-${month}-${day}T${timePart}`; // مثلاً "2025-03-03T12:34:56"
  };

export const getDateRange = (startYear: number, endYear: number): { startDate: string, endDate: string } => {

  if (!Number.isInteger(startYear) || !Number.isInteger(endYear) || startYear > endYear) {
    throw new Error("سال‌ها باید اعداد صحیح باشند و سال شروع باید کوچکتر یا مساوی سال پایان باشد");
  }


  const startDate = `${startYear}-01-01`;


  const endDate = `${endYear}-12-31`;

  return {
    startDate,
    endDate
  };
}



