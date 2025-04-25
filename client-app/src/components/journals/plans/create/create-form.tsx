import * as yup from "yup";
import {useNavigate} from "react-router-dom";
import {use, useEffect} from "react";
import {FormProvider, useForm} from "react-hook-form";
import {yupResolver} from "@hookform/resolvers/yup";

import apisWrapper from "../../../../libs/apis-wrapper.ts";
import {BackBtn, ResetBtn, SaveBtn} from "../../../ui/my-buttons.tsx";
import {FormInput, FormTimeInput} from "../../../ui/my-inputs.tsx";
import {useAuth} from "../../../../hooks/auth/use-auth.ts";
import {PaginatedResult, Technic} from "../../../../libs/definitions.ts";
import {FormDropdownMultiSelect} from "../../../ui/my-drop-downs.tsx";
import {FormCheckboxGroup} from "../../../ui/my-checkboxes.tsx";
import {days, timeToSeconds} from "../../../../libs/utilities.ts";
import NoDataPage from "../../../../pages/no-data-page.tsx";

const schema = yup.object({
        name: yup.string().required("نام الزامی است."),
        fromTime: yup
            .string()
            .nullable()
            .test(
                "fromTime-required-with-toTime",
                "هر دو از ساعت تا ساعت باید مقادیر داشته باشند یا هر دو خالی باشند.",
                function (value) {

                    const {toTime} = this.parent;

                    if (!value && toTime == "__:__:__") return true;
                    if (!value && toTime || value == "__:__:__" && toTime !== "__:__:__" && toTime) return false;

                    return true;
                },
            )
            .test(
                "valid-time-format", // Unique test name
                "فرمت زمان نامعتبر است. مقدار باید به صورت HH:mm:ss باشد.", // Error message
                function (value) {
                    if (!value || value == "__:__:__") return true; // Allow null values

                    // Regex pattern to match HH:mm:ss format (24-hour format)
                    const timeRegex = /^([01]\d|2[0-3]):[0-5]\d:[0-5]\d$/;

                    return timeRegex.test(value);
                }),
        toTime: yup
            .string()
            .nullable()
            .test(
                "toTime-required-with-fromTime",
                "هر دو از ساعت تا ساعت باید مقادیر داشته باشند یا هر دو خالی باشند.",
                function (value) {
                    const {fromTime} = this.parent;

                    if (!value && fromTime == "__:__:__") return true;
                    if (!value && fromTime || value == "__:__:__" && fromTime !== "__:__:__" && fromTime) return false;

                    return true;
                },
            )
            .test(
                "valid-time-format", // Unique test name
                "فرمت زمان نامعتبر است. مقدار باید به صورت HH:mm:ss باشد.", // Error message
                function (value) {
                    if (!value || value == "__:__:__") return true; // Allow null values

                    // Regex pattern to match HH:mm:ss format (24-hour format)
                    const timeRegex = /^([01]\d|2[0-3]):[0-5]\d:[0-5]\d$/;

                    return timeRegex.test(value);
                }).test(
                "time-order",
                "زمان پایان باید بزرگ‌تر از زمان شروع باشد.",
                function (value) {
                    const {fromTime} = this.parent;

                    // اگر یکی از فیلدها خالی یا "__:__:__" باشه، این تست اعمال نمی‌شه
                    if (!fromTime || !value || fromTime === "__:__:__" || value === "__:__:__") {
                        return true;
                    }

                    // تبدیل زمان به ثانیه برای مقایسه
                    const fromTimeSeconds = timeToSeconds(fromTime);
                    const toTimeSeconds = timeToSeconds(value);

                    if (fromTimeSeconds >= toTimeSeconds) {
                        return false;
                    }

                    return true;
                }
            ),
        selectedDays:
            yup.array().of(yup.string()),
        technics:
            yup
                .array()
                .of(yup.string().required("نوع انتخاب شده باید string باشد."))
                .min(1, "لطفا حداقل یک گزینه انتخاب کنید."),
    })
;

type FormData = yup.InferType<typeof schema>;

type props = {
    dataPromise: Promise<PaginatedResult<Technic>>;
};


export default function CreateForm({dataPromise}: props) {

    const data = use(dataPromise);

    const navigate = useNavigate();

    const {user} = useAuth();

    const methods = useForm<FormData>({
        mode: "onChange",
        resolver: yupResolver(schema),
        defaultValues: {
            name: "",
            fromTime: null,
            toTime: null,
            selectedDays: [],
            technics: [],
        },
    });
    const {watch, trigger} = methods;
    const fromTime = watch("fromTime");
    const toTime = watch("toTime");

    useEffect(() => {
        if (fromTime !== undefined) trigger("toTime");
    }, [fromTime, trigger]);

    useEffect(() => {
        if (toTime !== undefined) trigger("fromTime");
    }, [toTime, trigger]);

    const onSubmit = async (data: FormData) => {
        //console.log(data);
        await apisWrapper.PlanWrapper.create({
            userId: user?.id as string,
            name: data.name,
            fromTime: data.fromTime == "__:__:__" ? null : data.fromTime as string | null,
            toTime: data.toTime == "__:__:__" ? null : data.toTime as string | null,
            selectedDays: data.selectedDays as string[],
            technics: data.technics as string[]
        });
    };

    useEffect(() => {
        if (methods.formState.isSubmitSuccessful) {
            navigate("/plans");
        }
    }, [methods.formState.isSubmitSuccessful]);

    return (
        data.data.length !== 0 ?
            <>
                <div className="flex items-center justify-between p-5">
                    <h1 className="text-white/80 font-iran-sans font-bold text-lg">افزودن پلن</h1>
                    <BackBtn path="/plans"/>
                </div>
                <div className="p-5">

                    <div className="flex flex-col gap-4 bg-zinc-700 rounded-sm">
                        <FormProvider {...methods}>
                            <form className="p-5" onSubmit={methods.handleSubmit(onSubmit)}>

                                <div className="md:w-1/3 font-iran-sans-fa-num">
                                    <FormInput name={"name"} label={"نام"}/>
                                </div>
                                <div className="md:w-1/3 font-jbm-regular mt-5">
                                    <FormTimeInput name="fromTime" label="از ساعت" dir={"ltr"}/>
                                </div>
                                <div className="md:w-1/3 font-jbm-regular mt-5">
                                    <FormTimeInput name="toTime" label="تا ساعت" dir={"ltr"}/>
                                </div>
                                <div className="md:w-1/3 font-iran-sans-fa-num mt-5">
                                    <FormDropdownMultiSelect
                                        name="technics"
                                        label="تکنیک ها"
                                        options={
                                            data.data.map(item => {
                                                    return {label: item.name, value: item.id}
                                                }
                                            )
                                        }
                                    />
                                </div>
                                <div className="w-full md:w-1/2 font-jbm-regular mt-5">
                                    <FormCheckboxGroup name="selectedDays" label="روزهای هفته" options={
                                        days.map(item => {
                                                return {label: item, value: item}
                                            }
                                        )
                                    }/>
                                </div>

                                <div className="flex gap-4 pt-4">
                                    <SaveBtn isLoading={methods.formState.isSubmitting}/>
                                    <ResetBtn
                                        resetfn={
                                            () => {
                                                methods.reset();
                                                methods.setValue("fromTime", null, {shouldValidate: true});
                                                methods.setValue("toTime", null, {shouldValidate: true});
                                            }
                                        }/>

                                </div>
                            </form>
                        </FormProvider>
                    </div>
                </div>
            </>
            : <NoDataPage message="تکنیکی وجود ندارد. لطفا از بخش تکنیک ها حداقل یک مورد ثبت کنید!"/>
    );
}

