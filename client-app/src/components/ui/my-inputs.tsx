import React from "react";
import { useFormContext} from "react-hook-form";
import { tv } from "tailwind-variants";
import { Input as HeadlessUIInput } from "@headlessui/react";
import { useMask} from '@react-input/mask';

type FormInputProps = {
    name: string;
    label?: string;
    type?: string;
    ltr?: boolean;
} & React.InputHTMLAttributes<HTMLInputElement>;

const inputVariants = tv({
    base: "block w-full rounded-sm bg-zinc-800 px-3 py-2 focus:outline-none text-white/60 text-sm",
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

const FormFieldWrapper: React.FC<{
    name: string;
    label?: string;
    error?: any;
    children: React.ReactNode;
}> = ({ name, label, error, children }) => (
    <div className="flex flex-col gap-1">
        {label && (
            <label htmlFor={name} className="text-white/50 pb-1 text-xs font-medium font-iran-sans">
                {label}
            </label>
        )}
        {children}
        {error && (
            <p className="text-xs text-red-700 font-iran-sans">
                {error.message as string}
            </p>
        )}
    </div>
);

export const FormInput: React.FC<FormInputProps> = ({
                                                        name,
                                                        label,
                                                        type = "text",
                                                        ltr = false,
                                                        ...props
                                                    }) => {
    const {
        register,
        formState: { errors },
    } = useFormContext();

    const fieldError = errors[name];

    return (
        <FormFieldWrapper name={name} label={label} error={fieldError}>
            <HeadlessUIInput
                autoComplete="off"
                dir={ltr ? "ltr" : "rtl"}
                id={name}
                type={type}
                className={inputVariants({ intent: fieldError ? "error" : "default" })}
                {...register(name)}
                {...props}
            />
        </FormFieldWrapper>
    );
};

export const FormDateTimeInput: React.FC<FormInputProps> = ({
                                                                name,
                                                                label,
                                                                ltr = false,
                                                                ...props
                                                            }) => {
    const {
        register,
        formState: { errors },
    } = useFormContext();

    const fieldError = errors[name];

    // Get the mask ref from useMask.
    // Since useMask returns a ref object, we type cast it appropriately.
    const maskRef = useMask({
        showMask: true,
        mask: "DD/MM/YYYY __:__:__",
        replacement: { D: /\d/, M: /\d/, Y: /\d/, _: /\d/}
    }) as React.MutableRefObject<HTMLInputElement | null>;

    // Extract react-hook-form's register output, including its ref.
    const { ref: rhfRef, ...registerProps } = register(name) as {
        ref: React.Ref<HTMLInputElement>;
    };

    // Create a combined ref that sets the node on both maskRef and react-hook-form's ref.
    const combinedRef = (node: HTMLInputElement | null) => {
        // Instead of calling maskRef(node), we just assign the node to maskRef.current.
        if (maskRef) {
            maskRef.current = node;
        }
        // Deal with react-hook-form's ref, which can be a function or a ref object.
        if (rhfRef) {
            if (typeof rhfRef === "function") {
                rhfRef(node);
            } else {
                (rhfRef as React.MutableRefObject<HTMLInputElement | null>).current = node;
            }
        }
    };

    return (
        <FormFieldWrapper name={name} label={label} error={fieldError}>
            <HeadlessUIInput
                ref={combinedRef}
                autoComplete="off"
                dir={ltr ? "ltr" : "rtl"}
                id={name}
                type="text"
                className={inputVariants({
                    intent: fieldError ? "error" : "default",
                })}
                {...registerProps}
                {...props}
            />
        </FormFieldWrapper>
    );
};

export const FormTimeInput: React.FC<FormInputProps> = ({
                                                                name,
                                                                label,
                                                                ltr = false,
                                                                ...props
                                                            }) => {
    const {

        register,
        formState: { errors },
    } = useFormContext();


    const fieldError = errors[name];

    // Get the mask ref from useMask.
    // Since useMask returns a ref object, we type cast it appropriately.
    const maskRef = useMask({
        showMask: true,
        mask: "__:__:__",
        replacement: { _: /\d/}
    }) as React.MutableRefObject<HTMLInputElement | null>;


    // Extract react-hook-form's register output, including its ref.
    const { ref: rhfRef, ...registerProps } = register(name) as {
        ref: React.Ref<HTMLInputElement>;
    };

    // Create a combined ref that sets the node on both maskRef and react-hook-form's ref.
    const combinedRef = (node: HTMLInputElement | null) => {
        // Instead of calling maskRef(node), we just assign the node to maskRef.current.
        if (maskRef) {
            maskRef.current = node;
        }
        // Deal with react-hook-form's ref, which can be a function or a ref object.
        if (rhfRef) {
            if (typeof rhfRef === "function") {
                rhfRef(node);
            } else {
                (rhfRef as React.MutableRefObject<HTMLInputElement | null>).current = node;
            }
        }
    };

    return (
        <FormFieldWrapper name={name} label={label} error={fieldError}>
            <HeadlessUIInput
                ref={combinedRef}
                autoComplete="off"
                dir={ltr ? "ltr" : "rtl"}
                id={name}
                type="text"
                className={inputVariants({
                    intent: fieldError ? "error" : "default",
                })}
                {...registerProps}
                {...props}
            />
        </FormFieldWrapper>
    );
};