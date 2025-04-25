import { useFormContext } from "react-hook-form";
import { tv } from "tailwind-variants";
import { Textarea } from "@headlessui/react";

type FormTextareaProps = {
    name: string;
    label?: string;
    ltr?: boolean;
} & React.TextareaHTMLAttributes<HTMLTextAreaElement>;

const textareaVariants = tv({
    base: "block w-full rounded-sm bg-zinc-800 px-3 py-2 focus:outline-none text-white/60 text-sm leading-8 text-justify scrollbar-thumb-rounded-full scrollbar-track-rounded-full scrollbar-thin scrollbar-thumb-zinc-600 scrollbar-track-zinc-500 overflow-y-auto",
    variants: {
        intent: {
            default: "focus:ring-1 focus:ring-zinc-500",
            error: "focus:ring-1 focus:ring-red-700 bg-red-700/30",
        },
    },
    defaultVariants: {
        intent: "default",
    },
});

export const FormTextarea: React.FC<FormTextareaProps> = ({ name, label, ltr = false, ...props }) => {
    const {
        register,
        formState: { errors },
    } = useFormContext();

    return (
        <div className="flex flex-col gap-1">
            {label && <label className="text-white/50 pb-1 text-xs font-medium" htmlFor={name}>{label}</label>}
            <Textarea
                autoComplete="off"
                dir={ltr ? "ltr" : "rtl"}
                id={name}
                className={textareaVariants({ intent: errors[name] ? "error" : "default" })}
                {...register(name)}
                {...props}
            />
            {errors[name] && <p className="text-xs text-red-700 font-iran-sans">{errors[name]?.message as string}</p>}
        </div>
    );
};

export default FormTextarea;
