import {useEffect, useState} from "react";
import * as yup from "yup";
import {FormProvider, useForm} from "react-hook-form";
import {yupResolver} from "@hookform/resolvers/yup";
import PageLayout from "../layouts/page-layout.tsx";
import ProtectedRoute from "../components/auth/protected-route.tsx";
import apisWrapper from "../libs/apis-wrapper.ts";
import {ChangePassword, User} from "../libs/definitions.ts";

import {passwordRegex} from "../libs/regex.ts";

import {BackBtn, ResetBtn, SaveBtn} from "../components/ui/my-buttons.tsx";
import {FormInput} from "../components/ui/my-inputs.tsx";
import {toast} from "react-toastify";

const schema = yup.object({
    oldPassword: yup.string().required("رمز عبور فعلی الزامی است."),

    newPassword: yup
        .string()
        .required("رمز عبور جدید الزامی است.")
        .test(
            "password-complex",
            "رمز عبور باید حداقل 8 کاراکتر باشد و شامل حداقل یک حرف بزرگ، یک حرف کوچک، عدد و کاراکتر خاص باشد.",
            (value) => {
                return passwordRegex.test(value);
            },
        ),

    confirmPassword: yup
        .string()
        .required("تایید رمز عبور فعلی الزامی است.")
        .oneOf(
            [yup.ref("newPassword")],
            "تایید رمز عبور با رمز عبور جدید یکسان نیست.",
        ),
});

type FormData = yup.InferType<typeof schema>;

export default function UserSettingsPage() {


    const [user, setUser] = useState<User | undefined>(undefined);


    const methods = useForm<FormData>({
        resolver: yupResolver(schema),
        defaultValues: {
            newPassword: undefined,
            confirmPassword: "",
            oldPassword: "",
        },
    });
    useEffect(() => {
        if (methods.formState.isSubmitSuccessful) {

            toast.success(<span dir="rtl">رمز عبور با موفقیت تغییر کرد.</span>);
        }
    }, [methods.formState.isSubmitSuccessful]);

    const onSubmit = async (data: FormData) => {

        await apisWrapper.requests.post<string, ChangePassword>(
            "accounts/changePassword",
            {
                newPassword: data.newPassword,
                confirmPassword: data.confirmPassword,
                oldPassword: data.oldPassword,
            },
        );
        methods.reset();
    };

    useEffect(() => {
        apisWrapper.requests.get<User>("/accounts/getUser").then((r) => setUser(r));
    }, []);

    const handleReset = () => {
        methods.reset();
    }

    return (
        <ProtectedRoute>
            <PageLayout>
                <div className="flex items-center justify-between p-5">
                    <h1 className="text-white/80 font-iran-sans font-bold text-lg">افزودن پلن</h1>
                    <BackBtn path="/"/>
                </div>
                <div className="flex flex-col gap-1 bg-zinc-800 p-5 rounded-sm">

                    <div className="flex w-52 flex-col text-zinc-300 font-iran-sans text-sm gap-1">
                        <div className="flex items-center bg-zinc-700 p-2 rounded-sm">
                            <span className="pl-2">نام کاربری:</span>
                            <span className="font-jbm-regular">{user?.userName}</span>
                        </div>
                        <div className="flex items-center bg-zinc-700 p-2 rounded-sm">
                            <span className="pl-2">ایمیل:</span>
                            <span className="font-jbm-regular">{user?.email}</span>
                        </div>

                    </div>
                    <FormProvider {...methods}>
                        <form onSubmit={methods.handleSubmit(onSubmit)}>
                            <div className="flex  flex-col gap-4 font-secondary bg-zinc-700 p-5 rounded-sm">
                                <div className="lg:w-1/3 md:w-1/2">
                                    <FormInput name={"oldPassword"} label={"رمز عبور فعلی"} type="password" ltr={true}/>
                                    <FormInput name={"newPassword"} label={"رمز عبور جدید"} type="password" ltr={true}/>
                                    <FormInput name={"confirmPassword"} label={"تایید رمز عبور جدید"} type="password"
                                               ltr={true}/>
                                </div>

                                <div className="flex gap-4 pt-4">
                                    <SaveBtn isLoading={methods.formState.isSubmitting}/>
                                    <ResetBtn resetfn={handleReset}/>
                                </div>
                            </div>
                        </form>
                    </FormProvider>
                </div>
            </PageLayout>
        </ProtectedRoute>
    );
}
