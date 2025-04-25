import * as yup from "yup";
import {useNavigate} from "react-router-dom";
import {use} from "react";
import {FormProvider, useForm} from "react-hook-form";
import {yupResolver} from "@hookform/resolvers/yup";

import apisWrapper from "../../../libs/apis-wrapper.ts";
import {BackBtn, ResetBtn, SaveBtn} from "../../ui/my-buttons.tsx";
import {FormInput} from "../../ui/my-inputs.tsx";
import {User} from "../../../libs/definitions.ts";
import {
    validEmail,
} from "../../../libs/regex.ts";

const schema = yup.object({
    id: yup.string().required("شناسه الزامی است."),
    userName: yup.string().required("نام کاربری الزامی است."),
    email: yup.string().required("ایمیل الزامی است.")
        .test(
            "valid-email",
            "ایمیل معتبر نمی باشد.",
            (value) => {
                return validEmail.test(value);
            },
        ),

});

type FormData = yup.InferType<typeof schema>;

type props = {
    dataPromise: Promise<User>;
};


export default function UpdateForm({dataPromise}: props) {

    const userData = use(dataPromise);
    const navigate = useNavigate();

    const methods = useForm<FormData>({
        mode: "onChange",
        resolver: yupResolver(schema),
        defaultValues: {
            id: userData.id,
            userName: userData.userName,
            email: userData.email,
        },
    });
    const onSubmit = async (data: FormData) => {
        //console.log(data);
        await apisWrapper.UserWrapper.update({
            id: userData.id,
            userName: data.userName,
            email: data.email,
        }, userData.id);
        navigate("/users");
    };

    return (
        <>
            <div className="flex items-center justify-between p-5">
                <h1 className="text-white/80 font-iran-sans font-bold text-lg">ویرایش کاربر
                    <span className="pr-2">{userData.userName}</span> </h1>
                <BackBtn path="/users"/>
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


