import axios, {
    AxiosError,
    AxiosProgressEvent,
    AxiosRequestConfig,
    AxiosResponse,
    InternalAxiosRequestConfig,
} from "axios";
import qs from "qs";
import {getRefreshToken, getToken, setRefreshToken, setToken} from "./token-service.ts";
import {history} from "./history.ts";
import {
    CalendarData,
    ChartBalance,
    ChartNetProfit,
    CreatePlan,
    CreateTrade,
    CreateUser,
    GrossAndNetForEachSymbol,
    GrossAndNetForEachSymbolForEachDayOfWeek,
    PaginatedResult,
    Plan,
    QueryDto,
    QueryIdDto,
    TablePlan,
    Technic,
    Trade,
    UpdatePlan,
    UpdateTrade,
    UpdateUser,
    User,
} from "./definitions.ts";

import {toastError} from "../components/toastify/toast.tsx";


// تنظیمات اولیه Axios
axios.defaults.baseURL = import.meta.env.VITE_APP_API_URL as string;

// تابع کمکی برای تأخیر (فقط برای توسعه)
const pause = (duration: number) =>
    import.meta.env.MODE === "development"
        ? new Promise((resolve) => setTimeout(resolve, duration))
        : Promise.resolve();

// تابع کمکی برای افزودن هدر Authorization
const attachAuthHeader = (config: InternalAxiosRequestConfig) => {
    const token = getToken();
    if (token && config.headers) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
};

// تنظیم interceptor درخواست
axios.interceptors.request.use(attachAuthHeader, (error: AxiosError) =>
    Promise.reject(error)
);

// تابع کمکی برای استخراج داده از پاسخ
const responseBody = <T>(response: AxiosResponse<T>) => response?.data;

// مدیریت رفرش توکن
const handleTokenRefresh = async (error: AxiosError) => {

    const refreshToken = getRefreshToken();
    if (!refreshToken || error.config?.url === "accounts/refresh") {
        history.push("/login");
        return Promise.reject(error);
    }

    try {
        const response = await axios.post<{ token: string; refreshToken: string }>(
            "accounts/refresh",
            {refreshToken}
        );
        const {token: newAccessToken, refreshToken: newRefreshToken} = response.data;
        setToken(newAccessToken);
        setRefreshToken(newRefreshToken);

        if (error.config?.headers) {
            error.config.headers.Authorization = `Bearer ${newAccessToken}`;
        }
        return axios.request(error.config as InternalAxiosRequestConfig);
    } catch (refreshError) {
        console.error("Refresh token failure:", refreshError);
        history.push("/login");
        return Promise.reject(refreshError);
    }
};

// تنظیم interceptor پاسخ
axios.interceptors.response.use(
    async (response) => {
        await pause(500); // تأخیر فقط در حالت توسعه اعمال می‌شه
        return response;
    },
    async (error: AxiosError) => {
        //if (!error.response?.data) return;

        if (!error.response) return Promise.reject(error);
        const {data, status} = error.response as AxiosResponse;

        switch (status) {
            case 400:
                if (data.errors) {
                    toastError("خطایی در داده های ورودی وجود دارد.");
                    throw Object.values(data.errors).flat() as string[];
                }
                break;
            case 401:
                return handleTokenRefresh(error);
            case 404:
                toastError("رکورد مورد نظر یافت نشد.");
                break;
            case 500:
                toastError("خطایی در سمت سرور پیش آمده.");
                break;
                // router.navigate("/server-error", { state: { error: data } });
        }
        return Promise.reject(error.response);
    }
);

// توابع درخواست عمومی
const requests = {
    get: <T>(url: string, config?: AxiosRequestConfig) => axios.get<T>(url, config).then(responseBody),
    getQueryParam: <T>(url: string, params: object) =>
        axios
            .get<T>(url, {
                params,
                paramsSerializer: (p) =>
                    qs.stringify(p, {arrayFormat: "indices", encodeValuesOnly: true, allowDots: true}),
            })
            .then(responseBody),
    post: <T, B>(url: string, body: B) =>
        axios.post<T>(url, body, {headers: {"Content-Type": "application/json"}}).then(responseBody),
    postForm: (url: string, body: FormData, onProgress?: (percent: number) => void) =>
        axios
            .post<FormData>(url, body, {
                headers: {"Content-Type": "multipart/form-data"},
                onUploadProgress: onProgress
                    ? (e: AxiosProgressEvent) => e.total && onProgress(Math.round((e.loaded * 100) / e.total))
                    : undefined,
            })
            .then(responseBody),
    put: <T>(url: string, body: T) => axios.put<T>(url, body).then(responseBody),
    putForm: (url: string, body: FormData, onProgress?: (percent: number) => void) =>
        axios
            .put<FormData>(url, body, {
                headers: {"Content-Type": "multipart/form-data"},
                onUploadProgress: onProgress
                    ? (e: AxiosProgressEvent) => e.total && onProgress(Math.round((e.loaded * 100) / e.total))
                    : undefined,
            })
            .then(responseBody),
    del: <T>(url: string) => axios.delete<T>(url).then(responseBody),
};


const TradeWrapper = {
    list: (queryDto: QueryIdDto<string>) =>
        requests.getQueryParam<PaginatedResult<Trade>>("trade", queryDto),
    create: (data: CreateTrade) => requests.post<null, CreateTrade>("trade", data),
    getById: (id: string) => requests.get<Trade>(`trade/${id}`),
    //details: (id: number) => requests.get<Trade>(`trade/${id}`),
    delete: (id: string) => requests.del(`trade/${id}`),
    update: (data: UpdateTrade, id: string) => requests.put<UpdateTrade>(`trade/${id}`, data),
};

const UserWrapper = {
    list: (queryDto: QueryDto) =>
        requests.getQueryParam<PaginatedResult<User>>("Users/GetUsers", queryDto),
    create: (data: CreateUser) => requests.post<null, CreateUser>("Users/CreateUser", data),
    getById: (id: string) => requests.get<User>(`Users/GetUserById/${id}`),
    delete: (id: string) => requests.del(`Users/DeleteUser/${id}`),
    update: (data: UpdateUser, id: string) => requests.put<UpdateUser>(`Users/UpdateUser/${id}`, data),
};

const TradeAnalyseWrapper = {
    getChartBalance: (planId: string, config?: AxiosRequestConfig) =>
        requests.get<ChartBalance[]>(`trade-analyse/GetChartBalance/${planId}`, config),
    getChartNetProfit: (planId: string, config?: AxiosRequestConfig) =>
        requests.get<ChartNetProfit[]>(`trade-analyse/GetChartNetProfit/${planId}`, config),
    getGrossAndNetForEachSymbol: (planId: string, fromDate: string | null, toDate: string | null, config?: AxiosRequestConfig) =>
        requests.get<GrossAndNetForEachSymbol[]>(`trade-analyse/GetGrossAndNetForEachSymbol/${planId}?fromDate=${fromDate}&toDate=${toDate}`, config),
    getGrossAndNetForEachSymbolForEachDayOfWeek: (planId: string, fromDate: string | null, toDate: string | null, symbol: number | null, config?: AxiosRequestConfig) =>
        requests.get<GrossAndNetForEachSymbolForEachDayOfWeek[]>(`trade-analyse/GetGrossAndNetForEachSymbolForEachDayOfWeek/${planId}?fromDate=${fromDate}&toDate=${toDate}&symbol=${symbol == null ? '' : symbol}`, config),
    getCalenderData: (planId: string, selectedYear: number, config?: AxiosRequestConfig) =>
        requests.get<CalendarData>(`trade-analyse/${planId}/${selectedYear}`, config),
    getGetTradeYears: (planId: string, config?: AxiosRequestConfig) =>
        requests.get<number[]>(`trade-analyse/${planId}`, config)
};

const TechnicWrapper = {
    list: (queryDto: QueryDto) =>
        requests.getQueryParam<PaginatedResult<Technic>>("trading-technic", queryDto),
    create: (formData: FormData, onProgress?: (percent: number) => void) =>
        requests.postForm("trading-technic", formData, onProgress),
    getById: (id: string) => requests.get<Technic>(`trading-technic/${id}`),
    update: (formData: FormData, id: string, onProgress?: (percent: number) => void) =>
        requests.putForm(`trading-technic/${id}`, formData, onProgress),
    delete: (id: string) => requests.del(`trading-technic/${id}`),
};

const PlanWrapper = {
    list: (queryDto: QueryDto) =>
        requests.getQueryParam<PaginatedResult<TablePlan>>("trading-plan", queryDto),
    create: (data: CreatePlan) => requests.post<null, CreatePlan>("trading-plan", data),
    getById: (id: string) => requests.get<Plan>(`trading-plan/${id}`),
    update: (data: UpdatePlan, id: string) => requests.put<UpdatePlan>(`trading-plan/${id}`, data),
    delete: (id: string) => requests.del(`trading-plan/${id}`),
};

const apisWrapper = {
    requests,
    TradeWrapper,
    TechnicWrapper,
    PlanWrapper,
    TradeAnalyseWrapper,
    UserWrapper
};

export default apisWrapper;