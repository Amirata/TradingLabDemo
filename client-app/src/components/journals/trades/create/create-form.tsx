import * as yup from "yup";
import {useNavigate} from "react-router-dom";
import {use} from "react";
import {FormProvider, useForm} from "react-hook-form";
import {yupResolver} from "@hookform/resolvers/yup";

import apisWrapper from "../../../../libs/apis-wrapper.ts";
import {BackBtn, ResetBtn, SaveBtn} from "../../../ui/my-buttons.tsx";
import {FormDateTimeInput, FormInput} from "../../../ui/my-inputs.tsx";
import  {Plan, PositionType, Symbols} from "../../../../libs/definitions.ts";
import {FormDropdown} from "../../../ui/my-drop-downs.tsx";
import {
    validNegativeNumberOrZero,
    validNumber,
    validPositiveNumber
} from "../../../../libs/regex.ts";
import {formatDateTimeForJson} from "../../../../libs/utilities.ts";

const schema = yup.object({
    tradingPlanId: yup.string().required("پلن الزامی است."),
    symbol: yup.number().required("نماد الزامی است."),
    positionType: yup.number().required("نوع سفارش الزامی است."),
    volume: yup
        .string()
        .matches(
            /^(0\.(0[1-9]|[1-9]\d*)|[1-9]\d*(\.\d+)?)$/,
            "نوع عدد نا معتبر است. عدد باید بزرگتر مساوی 0.01 باشد.",
        )
        .required("حجم الزامی است."),
    entryPrice: yup
        .string()
        .matches(validPositiveNumber, "نوع عدد نا معتبر است. عدد باید مثبت باشد.")
        .required("قیمت ورود الزامی است."),
    closePrice: yup
        .string()
        .matches(validPositiveNumber, "نوع عدد نا معتبر است. عدد باید مثبت باشد.")
        .required("قیمت خروج الزامی است."),
    stopLossPrice: yup
        .string()
        .matches(validPositiveNumber, "نوع عدد نا معتبر است. عدد باید مثبت باشد.")
        .required("قیمت استاپ الزامی است."),
    entryDateTime: yup
        .string()
        .nullable()
        .test(
            "is-required",
            "تاریخ و زمان ورود الزامی است.",
            (value) => value !== null && value !== undefined && value !== "" && value !== "DD/MM/YYYY __:__:__",
        )
        .test(
            "is-valid-date",
            "تاریخ و زمان معتبر نیست.",
            (value) => {
                if (!value || value === "DD/MM/YYYY __:__:__") return true;
                const dateTimeRegex = /^\d{2}\/\d{2}\/\d{4} \d{2}:\d{2}:\d{2}$/;
                if (!dateTimeRegex.test(value)) return false;
                const [datePart, timePart] = value.split(" ");
                const [day, month, year] = datePart.split("/").map(Number);
                const [hours, minutes, seconds] = timePart.split(":").map(Number);

                const date = new Date(year, month - 1, day, hours, minutes, seconds);
                return (
                    date.getDate() === day &&
                    date.getMonth() === month - 1 &&
                    date.getFullYear() === year &&
                    hours >= 0 && hours <= 23 &&
                    minutes >= 0 && minutes <= 59 &&
                    seconds >= 0 && seconds <= 59
                );
            },
        ),
    closeDateTime: yup
        .string()
        .nullable()
        .test(
            "is-required",
            "تاریخ و زمان خروج الزامی است.",
            (value) => value !== null && value !== undefined && value !== "" && value !== "DD/MM/YYYY __:__:__",
        )
        .test(
            "is-valid-date",
            "تاریخ و زمان معتبر نیست.",
            (value) => {
                if (!value || value === "DD/MM/YYYY __:__:__") return true;
                const dateTimeRegex = /^\d{2}\/\d{2}\/\d{4} \d{2}:\d{2}:\d{2}$/;
                if (!dateTimeRegex.test(value)) return false;
                const [datePart, timePart] = value.split(" ");
                const [day, month, year] = datePart.split("/").map(Number);
                const [hours, minutes, seconds] = timePart.split(":").map(Number);

                const date = new Date(year, month - 1, day, hours, minutes, seconds);
                return (
                    date.getDate() === day &&
                    date.getMonth() === month - 1 &&
                    date.getFullYear() === year &&
                    hours >= 0 && hours <= 23 &&
                    minutes >= 0 && minutes <= 59 &&
                    seconds >= 0 && seconds <= 59
                );
            },
        ),
    commission: yup
        .string()
        .matches(
            validNegativeNumberOrZero,
            "نوع عدد نا معتبر است. کمیسیون باید صفر یا منفی باشد.",
        ),
    swap: yup.string().matches(validNumber, "نوع عدد نا معتبر است."),
    pips: yup
        .string()
        .matches(validNumber, "نوع عدد نا معتبر است.")
        .required("پیپ الزامی است."),
    netProfit: yup
        .string()
        .matches(validNumber, "نوع عدد نا معتبر است.")
        .required("سود الزامی است."),
    grossProfit: yup
        .string()
        .matches(validNumber, "نوع عدد نا معتبر است.")
        .required("سود خالص الزامی است."),
    balance: yup
        .string()
        .matches(validPositiveNumber, "نوع عدد نا معتبر است. عدد باید مثبت باشد.")
        .required("بالانس الزامی است."),
});

type FormData = yup.InferType<typeof schema>;

type props = {
    dataPromise: Promise<Plan>;
};


export default function CreateForm({dataPromise}: props) {

    const planData = use(dataPromise);
    const navigate = useNavigate();

    const methods = useForm<FormData>({
        mode: "onChange",
        resolver: yupResolver(schema),
        defaultValues: {
            tradingPlanId: planData.id,
            symbol: undefined,
            positionType: undefined,
            volume: undefined,
            entryPrice: undefined,
            closePrice: undefined,
            stopLossPrice: undefined,
            entryDateTime: null,
            closeDateTime: null,
            commission: undefined,
            swap: undefined,
            pips: undefined,
            netProfit: undefined,
            grossProfit: undefined,
            balance: undefined,
        },
    });
    const onSubmit = async (data: FormData) => {
        //console.log(data);
        await apisWrapper.TradeWrapper.create({
            tradingPlanId: data.tradingPlanId,
            symbol: data.symbol,
            positionType: data.positionType,
            volume: Number(data.volume),
            entryPrice: Number(data.entryPrice),
            closePrice: Number(data.closePrice),
            stopLossPrice: Number(data.stopLossPrice),
            entryDateTime: formatDateTimeForJson(data.entryDateTime as string) as string,
            closeDateTime: formatDateTimeForJson(data.closeDateTime as string) as string,
            commission: Number(data.commission ?? 0),
            swap: Number(data.swap ?? 0),
            pips: Number(data.pips),
            netProfit: Number(data.netProfit),
            grossProfit: Number(data.grossProfit),
            balance: Number(data.balance),
        });
        navigate("/");
    };

    return (
        <>
            <div className="flex items-center justify-between p-5">
                <h1 className="text-white/80 font-iran-sans font-bold text-lg">افزودن معامله به پلن {planData.name} </h1>
                <BackBtn path="/"/>
            </div>
            <div className="p-5">

                <div className="flex flex-col gap-4 bg-zinc-700 rounded-sm">
                    <FormProvider {...methods}>
                        <form className="p-5" onSubmit={methods.handleSubmit(onSubmit)}>
                            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-2">
                                <div className="font-iran-sans mt-5">

                                    <FormDropdown
                                        name="symbol"
                                        label="نماد"
                                        options={
                                            Object.entries(Symbols).map(([key, value]) => {
                                                    return {label: key, value: value}
                                                }
                                            )
                                        }
                                    />
                                </div>
                                <div className="font-iran-sans mt-5">

                                    <FormDropdown
                                        name="positionType"
                                        label="نوع سفارش"
                                        options={
                                            Object.entries(PositionType).map(([key, value]) => {
                                                    return {label: key, value: value}
                                                }
                                            )
                                        }
                                    />
                                </div>
                                <div className=" font-jbm-regular mt-5">
                                    <FormInput name={"volume"} label={"حجم"} ltr={true}/>
                                </div>
                                <div className=" font-jbm-regular mt-5">
                                    <FormInput name={"entryPrice"} label={"قیمت ورود"} ltr={true}/>
                                </div>
                                <div className=" font-jbm-regular mt-5">
                                    <FormInput name={"closePrice"} label={"قیمت خروج"} ltr={true}/>
                                </div>
                                <div className=" font-jbm-regular mt-5">
                                    <FormInput name={"stopLossPrice"} label={"قیمت استاپ"} ltr={true}/>
                                </div>
                                <div className=" font-jbm-regular mt-5">
                                    <FormInput name={"commission"} label={"کمیسیون"} ltr={true}/>
                                </div>
                                <div className=" font-jbm-regular mt-5">
                                    <FormInput name={"swap"} label={"سواپ"} ltr={true}/>
                                </div>
                                <div className=" font-jbm-regular mt-5">
                                    <FormInput name={"pips"} label={"پیپ"} ltr={true}/>
                                </div>
                                <div className=" font-jbm-regular mt-5">
                                    <FormInput name={"netProfit"} label={"سود و زیان"} ltr={true}/>
                                </div>
                                <div className=" font-jbm-regular mt-5">
                                    <FormInput name={"grossProfit"} label={"سود و زیان خالص"} ltr={true}/>
                                </div>
                                <div className=" font-jbm-regular mt-5">
                                    <FormInput name={"balance"} label={"بالانس"} ltr={true}/>
                                </div>
                                <div className=" font-jbm-regular mt-5">
                                    <FormDateTimeInput name="entryDateTime" label="زمان ورود" dir={"ltr"}/>
                                </div>
                                <div className=" font-jbm-regular mt-5">
                                    <FormDateTimeInput name="closeDateTime" label="زمان خروج" dir={"ltr"}/>
                                </div>
                            </div>
                            <div className="flex gap-4 pt-4">
                                <SaveBtn isLoading={methods.formState.isSubmitting}/>
                                <ResetBtn
                                    resetfn={
                                        () => {
                                            methods.reset();
                                            methods.setValue("entryDateTime", "DD/MM/YYYY __:__:__", {shouldValidate: false});
                                            methods.setValue("closeDateTime", "DD/MM/YYYY __:__:__", {shouldValidate: false});
                                        }
                                    }/>

                            </div>
                        </form>
                    </FormProvider>
                </div>
            </div>
        </>
    );
}


