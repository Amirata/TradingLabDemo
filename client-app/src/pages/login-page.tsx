import React, {useEffect} from "react";
import {useNavigate} from "react-router-dom";
import * as yup from "yup";
import {FormProvider, useForm} from "react-hook-form";
import {yupResolver} from "@hookform/resolvers/yup";


import apisWrapper from "../libs/apis-wrapper.ts";
import {useAuth} from "../hooks/auth/use-auth";
import {Login, LoginResult} from "../libs/definitions.ts";
import {LoginBtn} from "../components/ui/my-buttons.tsx";
import {FormInput} from "../components/ui/my-inputs.tsx";
import {toastError} from "../components/toastify/toast.tsx";
import {AxiosError} from "axios";


const schema = yup.object({
    userName: yup.string().required("نام کاربری الزامی است."),
    password: yup.string().required("رمز عبور الزامی است."),
});

type FormData = yup.InferType<typeof schema>;

const LoginPage: React.FC = () => {

    const navigate = useNavigate();
    const {login, isAuthenticated} = useAuth();

    useEffect(() => {
        if (isAuthenticated) {
            navigate("/");
        }
    }, [isAuthenticated]);


    const methods = useForm<FormData>({
        resolver: yupResolver(schema),
        defaultValues: {
            userName: undefined,
            password: undefined,
        },
    });

    useEffect(() => {
        if (methods.formState.isSubmitSuccessful) {
            navigate("/");
        }
    }, [methods.formState.isSubmitSuccessful]);

    const onSubmit = async (data: FormData) => {
        try {
            const {token, refreshToken} = await apisWrapper.requests.post<
                LoginResult,
                Login
            >("accounts/login", {
                userName: data.userName,
                password: data.password,
            });

            login(token, refreshToken);
        } catch (error:any) {
            if (error instanceof AxiosError && error.response?.status === 401) {
                toastError("نام کاربری یا رمز عبور اشتباه است.");
            }
            else{
                toastError("خطای برقراری اتصال با سرور.");
            }
            //console.error("Login failed:", error);
        }
    };

    return (
        <div
            className="flex justify-center items-center bg-zinc-900 h-screen font-iran-sans bg-[url('/5.png')] bg-cover bg-center"
            dir={"rtl"}>
            <div className="p-5 backdrop-blur-sm bg-zinc-700/80 rounded-lg w-80">
                <FormProvider {...methods}>
                    <form className="p-5" onSubmit={methods.handleSubmit(onSubmit)}>
                        <div className="flex flex-col gap-2">
                            <div className="pb-5" dir="ltr">
                                <h1 className="font-jbm-regular font-bold text-white text-lg">TradingLab</h1>
                            </div>
                            <FormInput name={"userName"} label={"نام کاربری"} ltr={true}/>
                            <FormInput name={"password"} label={"رمزعبور"} type="password" ltr={true}/>

                        </div>
                        <div className="flex gap-4 pt-4">
                            <LoginBtn isLoading={methods.formState.isSubmitting}/>
                        </div>
                    </form>
                </FormProvider>
            </div>
        </div>
    );
};

export default LoginPage;
