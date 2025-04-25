import {SVGProps} from "react";

export type PaginationResult<T> = {
    pageIndex: number;
    pageSize: number;
    count: number;
    totalPage: number;
    data: T[];
};

export type Technic = {
    id: string;
    name: string;
    description: string;
    images: string[];
};

export type Plan = {
    id: string;
    name: string;
    fromTime: string | null;
    toTime: string | null;
    selectedDays: string[];
    technics: Technic[];
};
export type TablePlan = {
    id: string;
    name: string;
    fromTime: string | null;
    toTime: string | null;
    selectedDays: string[];
};
export type SelectedPlan = {
    id: string,
    name: string
}

export type CreatePlan = {
    name: string;
    fromTime: string | null;
    toTime: string | null;
    selectedDays: string[];
    technics: string[];
    userId: string;
};

export type UpdatePlan = {
    id: string;
    name: string;
    fromTime: string | null;
    toTime: string | null;
    selectedDays: string[];
    technics: string[];
    removedTechnics: string[];
};

export type Trade = {
    id: string;
    tradingPlanId: string;
    symbol: number;
    positionType: number;
    volume: number;
    entryPrice: number;
    closePrice: number;
    stopLossPrice: number;
    entryDateTime: string;
    closeDateTime: string;
    commission: number;
    swap: number;
    pips: number;
    netProfit: number;
    grossProfit: number;
    balance: number;
    tradingPlanName: string;
};

export type CreateTrade = {
    tradingPlanId: string;
    symbol: number;
    positionType: number;
    volume: number;
    entryPrice: number;
    closePrice: number;
    stopLossPrice: number;
    entryDateTime: string;
    closeDateTime: string;
    commission: number;
    swap: number;
    pips: number;
    netProfit: number;
    grossProfit: number;
    balance: number;
};

export type UpdateTrade = {
    id: string;
    tradingPlanId: string;
    symbol: number;
    positionType: number;
    volume: number;
    entryPrice: number;
    closePrice: number;
    stopLossPrice: number;
    entryDateTime: string;
    closeDateTime: string;
    commission: number;
    swap: number;
    pips: number;
    netProfit: number;
    grossProfit: number;
    balance: number;
};

export type Calendar = {
    date: string;
    count: number;
    level: number;
};

export type ChartBalance = {
    dateTime: string;
    balance: number;
}

export type ChartNetProfit = {
    date: string;
    netProfit: number;
}

export type GrossAndNetForEachSymbol = {
    symbol: number;
    netProfit: number;
    grossProfit: number;
}

export type GrossAndNetForEachSymbolForEachDayOfWeek = {
    dayOfWeek: number;
    netProfit: number;
    grossProfit: number;
}

export type CalendarData = {
    calendar: Calendar[];
    riskToRewardMean: number;
    winRate: number;
    totalTradeCount: number;
    totalWinTradeCount: number;
    totalLossTradeCount: number;
    netProfit: number;
    grossProfit: number;
};

export type Login = {
    userName: string;
    password: string;
};

export type LoginResult = {
    token: string;
    refreshToken: string;
};

export type User = {
    id: string;
    userName: string;
    email: string;
};

export type CreateUser = {
    userName: string;
    email: string;
    password: string;
    role: number;
};

export type UpdateUser = {
    id: string;
    userName: string;
    email: string;
};

export type ChangePassword = {
    oldPassword: string;
    newPassword: string;
    confirmPassword: string;
}
export type IconSvgProps = SVGProps<SVGSVGElement> & {
    size?: number;
};

export type DayOfWeek =
    | "Monday"
    | "Tuesday"
    | "Wednesday"
    | "Thursday"
    | "Friday"
    | "Saturday"
    | "Sunday";

export const PositionType = {
    Long: 0,
    Short: 1,
};

export const DayOfWeekEnum = {
    Sunday:0,
    Monday:1,
    Tuesday:2,
    Wednesday:3,
    Thursday:4,
    Friday:5,
    Saturday:6
}

export const Symbols = {
    // Major Pairs
    EurUsd: 0, // Euro / US Dollar
    GbpUsd: 1, // British Pound / US Dollar
    UsdJpy: 2, // US Dollar / Japanese Yen
    UsdChf: 3, // US Dollar / Swiss Franc
    AudUsd: 4, // Australian Dollar / US Dollar
    UsdCad: 5, // US Dollar / Canadian Dollar
    NzdUsd: 6, // New Zealand Dollar / US Dollar

    // Major Indices
    Sp500: 7, // S&P 500
    Us30: 8, // Dow Jones Industrial Average
    Us100: 9, //nasdaq 100 index

    // Metals
    XauUsd: 10, //gold vs dollar
    XagUsd: 11, //silver vs dollar
};

export interface PaginationDto {
    pageNumber: number;
    pageSize: number;
}

export interface PaginatedResult<T> {
    data: T[];
    currentPage: number;
    totalPages: number;
    totalCount: number;
    pageSize: number;
    hasPreviousPage: boolean;
    hasNextPage: boolean;
}

export interface Sort {
    sortOrder: string;
    sortBy: string;
    order: number;
}

export interface QueryDto {
    pageNumber: number;
    pageSize: number;
    search: string;
    sorts: Sort[];
}

export interface QueryIdDto<T> {
    id: T;
    pageNumber: number;
    pageSize: number;
    search: string;
    sorts: Sort[];
}



