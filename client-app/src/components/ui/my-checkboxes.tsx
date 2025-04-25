import {useFormContext} from "react-hook-form";
import {Checkbox} from "@headlessui/react";
import {CheckIcon} from "@heroicons/react/20/solid";


const FormFieldWrapper: React.FC<{
    name: string;
    label?: string;
    error?: any;
    children: React.ReactNode;
}> = ({name, label, error, children}) => (
    <div className="flex flex-col gap-1">
        {label && (
            <label htmlFor={name} className="text-white/50 pb-1 text-xs font-medium font-iran-sans">
                {label}
            </label>
        )}
        {children}
        {error && <p className="text-xs text-red-700 font-iran-sans">{error.message as string}</p>}
    </div>
);

export const FormCheckboxGroup: React.FC<{
    name: string;
    label?: string;
    options: { label: string; value: string }[];
}> = ({name, label, options}) => {
    const {setValue, watch, formState: {errors}} = useFormContext();
    const selectedValues: string[] = watch(name) || [];

    const handleCheckboxChange = (value: string) => {
        const newValues = selectedValues.includes(value)
            ? selectedValues.filter(v => v !== value)
            : [...selectedValues, value];
        setValue(name, newValues);
    };

    return (
        <FormFieldWrapper name={name} label={label} error={errors[name]}>
            <div className="flex flex-wrap gap-2 bg-zinc-800 p-2 rounded-sm">
                {options.map(({label, value}) => (
                    <label key={value} className="flex items-center gap-2 text-white/60 text-sm">
                        <Checkbox
                            checked={selectedValues.includes(value)}
                            onChange={() => handleCheckboxChange(value)}
                            className="group size-4 rounded-md bg-white/10 p-1 ring-1 ring-white/15 ring-inset data-[checked]:bg-zinc-900"
                        >

                            <CheckIcon className="hidden size-2 fill-black group-data-[checked]:block stroke-white"/>


                        </Checkbox>
                        {label}
                    </label>
                ))}
            </div>
        </FormFieldWrapper>
    );
};