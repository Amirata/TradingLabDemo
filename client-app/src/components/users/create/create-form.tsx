import * as yup from "yup";
import {useNavigate} from "react-router-dom";
import {FormProvider, useForm} from "react-hook-form";
import {yupResolver} from "@hookform/resolvers/yup";

import apisWrapper from "../../../libs/apis-wrapper.ts";
import {BackBtn, ResetBtn, SaveBtn} from "../../ui/my-buttons.tsx";
import {FormInput} from "../../ui/my-inputs.tsx";

import {
    passwordRegex, validEmail,
} from "../../../libs/regex.ts";

const schema = yup.object({
    userName: yup.string().required("نام کاربری الزامی است."),
    email: yup.string().required("ایمیل الزامی است.")
        .test(
            "valid-email",
            "ایمیل معتبر نمی باشد.",
            (value) => {
                return validEmail.test(value);
            },
        ),
    password: yup
        .string()
        .required("رمز عبور الزامی است.")
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
            [yup.ref("password")],
            "تایید رمز عبور با رمز عبور جدید یکسان نیست.",
        ),

});

type FormData = yup.InferType<typeof schema>;


export default function CreateForm() {

    const navigate = useNavigate();

    const methods = useForm<FormData>({
        mode: "onChange",
        resolver: yupResolver(schema),
        defaultValues: {
            userName: undefined,
            email: undefined,
            password: undefined,
            confirmPassword: undefined,
        },
    });
    const onSubmit = async (data: FormData) => {
        //console.log(data);
        await apisWrapper.UserWrapper.create({
            userName: data.userName,
            email: data.email,
            password: data.password,
            role: 1
        });
        navigate("/users");
    };

    return (
        <>
            <div className="flex items-center justify-between p-5">
                <h1 className="text-white/80 font-iran-sans font-bold text-lg">افزودن کاربر</h1>
                <BackBtn path="/"/>
            </div>
            <div className="p-5">

                <div className="flex flex-col gap-4 bg-zinc-700 rounded-sm">
                    <FormProvider {...methods}>
                        <form className="p-5" onSubmit={methods.handleSubmit(onSubmit)}>
                            <div className="grid grid-cols-1 gap-2">
                                <div className="max-w-96 font-jbm-regular mt-5">
                                    <FormInput name={"userName"} label={"نام کاربری"} ltr={true}/>
                                </div>
                                <div className="max-w-96 font-jbm-regular mt-5">
                                    <FormInput name={"email"} label={"پست الکترونیکی"} ltr={true}/>
                                </div>
                                <div className="max-w-96 font-jbm-regular mt-5">
                                    <FormInput name={"password"} label={"رمز عبور"} type="password" ltr={true}/>

                                </div>
                                <div className="max-w-96 font-jbm-regular mt-5">
                                    <FormInput name={"confirmPassword"} label={"تایید رمز عبور"} type="password"
                                               ltr={true}/>
                                </div>
                            </div>
                            <div className="flex gap-4 pt-4">
                                <SaveBtn isLoading={methods.formState.isSubmitting}/>
                                <ResetBtn
                                    resetfn={
                                        () => {
                                            methods.reset();
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


