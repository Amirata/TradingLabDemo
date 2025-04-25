import * as yup from "yup";
import {useNavigate} from "react-router-dom";
import {useState} from "react";
import {Controller, FormProvider, useForm} from "react-hook-form";
import {yupResolver} from "@hookform/resolvers/yup";

import ImagesInput from "../../../../components/journals/technics/images/images-input.tsx";
import apisWrapper from "../../../../libs/apis-wrapper.ts";
import {BackBtn, ResetBtn, SaveBtn} from "../../../ui/my-buttons.tsx";
import {FormInput} from "../../../ui/my-inputs.tsx";
import FormTextarea from "../../../ui/my-textareas.tsx";
import {useAuth} from "../../../../hooks/auth/use-auth.ts";

const schema = yup.object({
    name: yup.string().required("نام الزامی است."),
    description: yup.string().required("توضیحات الزامی است."),
    newImages: yup.array().of(
        yup
            .mixed<File>()
            .test(
                "is-valid-type",
                "نوع تصویر معتبر نمی باشد.",
                (value: File | null | undefined) => {
                    if (!value) return false;

                    return isValidFileType(value.name.toLowerCase(), "image");
                },
            )
            .test(
                "is-valid-size",
                "حداکثر سایز مجاز تصویر 5 مگابایت می باشد.",
                (value: File | null | undefined) => {
                    if (!value) return false;

                    return value.size <= MAX_FILE_SIZE;
                },
            ),
    ),
});

type FormData = yup.InferType<typeof schema>;

const MAX_FILE_SIZE = 1024 * 5000; //5MB

const validFileExtensions: { [key: string]: string[] } = {
    image: ["jpg", "gif", "png", "jpeg", "svg", "webp"],
};

function isValidFileType(
    fileName: string,
    fileType: keyof typeof validFileExtensions,
): boolean {
    if (!fileName) return false;

    const fileExtension = fileName.split(".").pop();

    return (
        !!fileExtension && validFileExtensions[fileType].includes(fileExtension)
    );
}

export default function CreateForm() {
    const navigate = useNavigate();
    const [previews, setPreviews] = useState<string[]>([]);
    const {user} = useAuth();

    const methods = useForm<FormData>({
        resolver: yupResolver(schema),
        defaultValues: {
            name: "",
            description: "",
            newImages: [],
        },
    });
    const onSubmit = async (data: FormData) => {
        const formData = new FormData();
        formData.append("name", data.name);
        formData.append("description", data.description);
        if (data) {
            const images = data.newImages as File[];

            images.forEach((file) => formData.append("newImages", file));
        }

        formData.append("userId", user?.id as string);

        await apisWrapper.TechnicWrapper.create(formData);
        navigate("/technics");
    };


    return (
        <>
            <div className="flex items-center justify-between p-5">
                <h1 className="text-white/80 font-iran-sans font-bold text-lg">افزودن تکنیک</h1>
                <BackBtn path="/technics"/>
            </div>
            <div className="p-5">

                <div className="flex flex-col gap-4 bg-zinc-700 rounded-sm">
                    <FormProvider {...methods}>
                        <form className="p-5" onSubmit={methods.handleSubmit(onSubmit)}>
                                <div className="md:w-1/3 font-iran-sans-fa-num">
                                    <FormInput name={"name"} label={"نام"}/>
                                </div>
                                <div className="pt-4 md:w-2/3 font-iran-sans-fa-num">
                                    <FormTextarea name={"description"} label={"توضیحات"}/>
                                </div>

                                <div className="pt-5">

                                    <Controller
                                        control={methods.control}
                                        name="newImages"
                                        render={({field}) => (
                                            <ImagesInput
                                                errors={methods.formState.errors.newImages}
                                                previews={previews}
                                                setPreviews={setPreviews}
                                                value={field.value as File[]}
                                                onChange={field.onChange}
                                            />
                                        )}
                                    />
                                </div>

                                <div className="flex gap-4 pt-4">
                                    <SaveBtn isLoading={methods.formState.isSubmitting}/>
                                    <ResetBtn
                                        resetfn={
                                            () => {
                                                methods.reset();
                                                setPreviews([]);
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
