import {
    Menu,
    MenuButton,
    MenuItem,
    MenuItems
} from '@headlessui/react'

import {ReactNode, useEffect, useState} from "react";
import {Link, useNavigate} from "react-router-dom";

import {useAuth} from "../../hooks/auth/use-auth.ts";
import {Check, ChevronDown, KeyRound, LogOut} from "lucide-react";
import {useFormContext} from "react-hook-form";
import {Symbols} from "../../libs/definitions.ts";
import {useAppStore} from "../../libs/stores/app-store.ts";


// const SimpleDropDown = ({menuBtnNode, menuItemsNode}: {
//     menuBtnNode: ReactNode,
//     menuItemsNode: ReactNode[],
// }) => {
//     return (
//         <div className="w-52">
//             <Menu>
//                 <MenuButton
//                     className="inline-flex w-full items-center gap-2 rounded-sm bg-zinc-700 py-1.5 px-3 text-sm/6 text-white focus:outline-none data-[hover]:bg-zinc-600 data-[open]:bg-zinc-700 data-[focus]:outline-1 data-[focus]:outline-white">
//                     <ChevronDown strokeWidth={0.75} className="size-4 stroke-zinc-100"/>
//                     {menuBtnNode}
//                 </MenuButton>
//
//                 <MenuItems
//                     transition
//                     anchor="bottom end"
//                     className="w-52 origin-top-right rounded-sm border border-white/5 bg-zinc-100/20 mt-1 p-1 text-sm/6 text-white transition duration-100 ease-out [--anchor-gap:var(--spacing-1)] focus:outline-none data-[closed]:scale-95 data-[closed]:opacity-0"
//                 >
//                     {menuItemsNode.map((item, index) =>
//
//                         <MenuItem key={index}>
//                             {item}
//                         </MenuItem>
//                     )}
//                 </MenuItems>
//             </Menu>
//         </div>
//     )
// }

const NavbarDropDown = ({menuBtnNode, menuItemsNode}: {
    menuBtnNode: ReactNode,
    menuItemsNode: ReactNode[],
}) => {
    return (

        <Menu>
            <MenuButton
                className="inline-flex items-center gap-2 border-x border-x-zinc-600 py-5 px-3 text-sm/6 text-white focus:outline-none data-[hover]:bg-zinc-600 data-[open]:bg-zinc-700 data-[focus]:outline-1 data-[focus]:outline-white">
                <ChevronDown strokeWidth={0.75} className="size-4 stroke-zinc-100"/>
                {menuBtnNode}
            </MenuButton>

            <MenuItems
                transition
                anchor="bottom end"
                className="w-52 origin-top-right rounded-sm border border-zinc-500 bg-zinc-700 mt-1 p-1 text-sm/6 text-white transition duration-100 ease-out [--anchor-gap:var(--spacing-1)] focus:outline-none data-[closed]:scale-95 data-[closed]:opacity-0"
            >
                {menuItemsNode.map((item, index) =>

                    <MenuItem key={index}>
                        {item}
                    </MenuItem>
                )}
            </MenuItems>
        </Menu>

    )
}

export const UserSettingsDropdown = () => {
    const {logout, user} = useAuth();
    const navigate = useNavigate();
    return (
        <NavbarDropDown
            menuBtnNode={
                <span className="font-jbm-regular">{user?.username}</span>
            }
            menuItemsNode={
                [
                    <Link className={
                        "group flex justify-end w-full items-center gap-2 rounded-sm py-1.5 px-3 data-[focus]:bg-zinc-900/60"
                    } to="/userSetting">
                        <span className="font-iran-sans">تغییر رمز عبور</span>
                        <KeyRound strokeWidth={0.75} className="size-4 stroke-zinc-100"/>
                    </Link>,
                    <button
                        onClick={() => {
                            navigate("/login");
                            logout();
                        }}
                        className="group flex justify-end w-full items-center gap-2 rounded-sm py-1.5 px-3 data-[focus]:bg-zinc-900/60">

                        <span className="font-iran-sans">خروج</span>
                        <LogOut strokeWidth={0.75} className="size-4 stroke-zinc-100"/>

                    </button>
                ]
            }/>
    )
}


type FormDropdownProps = {
    name: string;
    label?: string;
    options: { label: string; value: string | number }[];
} & React.ButtonHTMLAttributes<HTMLButtonElement>;

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

export const FormDropdown: React.FC<FormDropdownProps> = ({
                                                              name,
                                                              label,
                                                              options,
                                                              ...props
                                                          }) => {
    const {
        clearErrors,
        setValue,
        watch,
        formState: {errors},
    } = useFormContext();

    const selectedValue = watch(name); // Get selected value from react-hook-form
    const selectedOption = options.find((opt) => opt.value === selectedValue);
    const [displayText, setDisplayText] = useState(selectedOption?.label || "انتخاب کنید");

    // Update display text when reset() is called or when the value changes
    useEffect(() => {
        setDisplayText(selectedOption?.label || "انتخاب کنید");
    }, [selectedValue]); // Watches for form reset and value changes

    const handleSelect = (value: string | number, label: string) => {
        setValue(name, value);
        setDisplayText(label);
        clearErrors(name);
    };

    return (
        <FormFieldWrapper name={name} label={label} error={errors[name]}>
            <Menu as="div" className="relative">
                <MenuButton
                    {...props}
                    className={`flex w-full items-center justify-between rounded-sm px-3 py-2 text-white/60 text-sm focus:outline-none ${
                        errors[name] ? "ring-1 ring-red-700 bg-red-700/30" : "bg-zinc-800 focus:ring-1 focus:ring-zinc-500"
                    }`}
                >
                    {displayText}
                    <ChevronDown className="size-4 stroke-zinc-100"/>
                </MenuButton>

                <MenuItems
                    className="absolute z-10 w-full mt-1 bg-zinc-700 border border-zinc-600 rounded-sm shadow-lg">
                    {options.map(({label, value}) => (
                        <MenuItem key={value} as="div">
                            {({active}) => (
                                <button
                                    type="button"
                                    className={`w-full px-3 py-2 text-left ${
                                        active ? "bg-zinc-600" : "bg-zinc-700"
                                    } text-white/60`}
                                    onClick={() => handleSelect(value, label)}
                                >
                                    <div className="flex justify-start items-center">
                                        {label}
                                    </div>
                                </button>
                            )}
                        </MenuItem>
                    ))}
                </MenuItems>
            </Menu>
        </FormFieldWrapper>
    );
};


export const FormDropdownMultiSelect: React.FC<{
    name: string;
    label?: string;
    options: { label: string; value: string }[];
}> = ({name, label, options}) => {
    const {clearErrors, setValue, watch, formState: {errors}} = useFormContext();
    const selectedValues: string[] = watch(name) || [];
    const [selectedOptions, setSelectedOptions] = useState<{ label: string; value: string }[]>(
        options.filter(opt => selectedValues.includes(opt.value))
    );

    useEffect(() => {
        setSelectedOptions(options.filter(opt => selectedValues.includes(opt.value)));
    }, [selectedValues]);

    const handleSelect = (value: string) => {
        const newValues = selectedValues.includes(value)
            ? selectedValues.filter(v => v !== value)
            : [...selectedValues, value];
        setValue(name, newValues);
        clearErrors(name);
    };

    return (
        <FormFieldWrapper name={name} label={label} error={errors[name]}>
            <Menu as="div" className="relative">
                <MenuButton
                    className={`flex w-full items-center justify-between rounded-sm px-3 py-2 text-white/60 text-sm focus:outline-none ${
                        errors[name] ? "ring-1 ring-red-700 bg-red-700/30" : "bg-zinc-800 focus:ring-1 focus:ring-zinc-500"
                    }`}
                >
                    {selectedOptions.length > 0 ? selectedOptions.map(opt => opt.label).join(", ") : "انتخاب کنید"}
                    <ChevronDown className="size-4 stroke-zinc-100"/>
                </MenuButton>

                <MenuItems
                    className="absolute z-10 w-full mt-1 bg-zinc-700 border border-zinc-600 rounded-sm shadow-lg">
                    {options.map(({label, value}) => (
                        <MenuItem key={value} as="div">
                            {({active}) => (
                                <button
                                    type="button"
                                    className={`w-full px-3 py-2 text-left ${
                                        active ? "bg-zinc-600" : "bg-zinc-700"
                                    } text-white/60`}
                                    onClick={() => handleSelect(value)}
                                >
                                    <div className="flex justify-between items-center">
                                        <div>
                                            {label}
                                        </div>
                                        <div>
                                            {selectedValues.includes(value) ? <Check className="size-4"/> : ""}
                                        </div>
                                    </div>
                                </button>
                            )}
                        </MenuItem>
                    ))}
                </MenuItems>
            </Menu>
        </FormFieldWrapper>
    );
};


export const SymbolsDropdown = () => {
    const {symbolList, setSelectedSymbol} = useAppStore();
    const options: { label: string, value: number | null }[] = [{label: "All symbols", value: null}];

    options.push(...Object.entries(Symbols)
        // eslint-disable-next-line @typescript-eslint/no-unused-vars
        .filter(([_, value]) => symbolList.includes(value))
        .map(([key, value]) => ({
            label: key,
            value: value as number
        }))
    );


    const [selectedValue, setSelectedValue] = useState<number | null>(null); // Get selected value from react-hook-form


    const selectedOption = options.find((opt) => opt.value === selectedValue);
    const [displayText, setDisplayText] = useState(selectedOption?.label || "All symbols");

    // Update display text when reset() is called or when the value changes
    useEffect(() => {
        setDisplayText(selectedOption?.label || "All symbols");
    }, [selectedValue]); // Watches for form reset and value changes

    const handleSelect = (value: number | null, label: string) => {
        setSelectedValue(value);
        setDisplayText(label);
        setSelectedSymbol(value);
    };

    return (


        <Menu as="div" className="relative">
            <MenuButton
                disabled={symbolList.length == 0}
                className={`flex w-full items-center justify-between rounded-sm px-3 py-2 text-white/60 text-sm focus:outline-none ${
                    "bg-zinc-800 focus:ring-1 focus:ring-zinc-500 font-jbm-regular disabled:cursor-not-allowed"
                }`}
            >
                {displayText}
                <ChevronDown className="size-4 stroke-zinc-100"/>
            </MenuButton>

            <MenuItems
                className="absolute z-10 w-full mt-1 bg-zinc-700 border border-zinc-600 rounded-sm shadow-lg">
                {options.map(({label, value}) => (
                    <MenuItem key={value} as="div">
                        {({active}) => (
                            <button
                                type="button"
                                className={`w-full px-3 py-2 text-left ${
                                    active ? "bg-zinc-600" : "bg-zinc-700"
                                } text-white/60`}
                                onClick={() => handleSelect(value, label)}
                            >
                                <div className="flex justify-start items-center font-jbm-regular">
                                    {label}
                                </div>
                            </button>
                        )}
                    </MenuItem>
                ))}
            </MenuItems>
        </Menu>

    );
}