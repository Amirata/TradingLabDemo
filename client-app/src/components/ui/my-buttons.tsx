import {tv} from 'tailwind-variants';
import {Button} from "@headlessui/react";
import {useNavigate} from "react-router-dom";

import {Save, LogIn, ArrowBigLeft, RotateCcw, Trash2} from 'lucide-react';
import {ReactNode} from "react";


export const button = tv({
    base: 'text-white/70 border border-white/20 text-xs min-w-30 py-1.5 px-4 rounded-xs active:opacity-50 transition-all duration-200',
    variants: {
        color: {
            primary: 'bg-blue-500/20 hover:bg-blue-700',
            secondary: 'bg-purple-500/20 hover:bg-purple-700',
            success: 'bg-green-500/20 hover:bg-green-700',
            warning: 'bg-red-500/20 hover:bg-red-700',
            default: 'bg-slate-500/20 hover:bg-slate-500',
        },
        disabled: {
            true: 'opacity-50 bg-zinc-500/40 pointer-events-none'
        }
    }
});

export const sqButton = tv({
    base: 'flex items-center justify-center text-white/80 border border-white/20 text-xs w-8 h-8 py-1.5 px-1.5 rounded-xs active:opacity-50 transition-all duration-200',
    variants: {
        color: {
            primary: 'bg-blue-500/20 hover:bg-blue-700',
            secondary: 'bg-purple-500/20 hover:bg-purple-700',
            success: 'bg-green-500/20 hover:bg-green-700',
            warning: 'bg-red-500/20 hover:bg-red-700',
            default: 'bg-slate-500/20 hover:bg-slate-500',
        },
        disabled: {
            true: 'opacity-50 bg-zinc-500/40 pointer-events-none'
        },
        isSelected:{
            true: "border-white/50"
        }
    }
});

const BtnLoading = () => {
    return (
        <div
            className="w-5 h-5 border-2 border-white border-t-transparent border-solid rounded-full animate-spin [animation-duration:400ms]">
        </div>
    )
}

export const BackBtn = ({path}: { path: string }) => {

    const navigate = useNavigate();
    return (
        <Button type="button" className={button({color: "default"})} onClick={() => navigate(path)}>
            <div className="flex items-center justify-center w-full font-iran-sans">
                <ArrowBigLeft strokeWidth={0.75} className="size-4 stroke-zinc-400"/>
                <span className="pr-2 font-iran-sans">
            بازگشت
            </span>
            </div>
        </Button>
    )
}

export const ResetBtn = ({resetfn}: { resetfn: () => void }) => {

    return (
        <Button type="button" className={button({color: "default"})} onClick={resetfn}>
            <div className="flex items-center justify-center w-full font-iran-sans">
                <RotateCcw strokeWidth={0.75} className="size-4 stroke-zinc-400"/>
                <span className="pr-2 font-iran-sans font-iran-sans">
            بازیابی
            </span>
            </div>
        </Button>
    )
}

export const TableAddBtn = ({message, path, disabled = false}: { message: ReactNode, path: string, disabled?:boolean }) => {
    const navigate = useNavigate();
    return (
        <Button disabled={disabled} type="button" className={button({color: "default",disabled: disabled})} onClick={() => navigate(path)}>
            {message}
        </Button>
    )
}

export const SaveBtn = ({
                            isLoading
                        }: {
    isLoading: boolean
}) => {


    return (
        <Button type="submit" className={button({color: "success", disabled: isLoading})}>
            <div className="flex items-center justify-center w-full">


                {!isLoading ? (
                    <>
                        <Save strokeWidth={0.75} className="size-4 stroke-green-400"/>
                        <span className="pr-2 font-iran-sans">
                    ذخیره
                </span>
                    </>
                ) : (
                    <>
                        <BtnLoading/>
                        <span className="pr-2 font-iran-sans">
                           ذخیره
                         </span>
                    </>
                )}
            </div>
        </Button>
    )
}

export const DeleteBtn = ({
                            isLoading,
                            onDelete
                        }: {
    isLoading: boolean,
    onDelete: () => void
}) => {


    return (
        <Button type="submit" className={button({color: "warning", disabled: isLoading})} onClick={onDelete}>
            <div className="flex items-center justify-center w-full">


                {!isLoading ? (
                    <>
                        <Trash2 strokeWidth={0.75} className="size-4 stroke-red-400"/>
                        <span className="pr-2 font-iran-sans">
                    حذف
                </span>
                    </>
                ) : (
                    <>
                        <BtnLoading/>
                        <span className="pr-2 font-iran-sans">
                           حذف
                         </span>
                    </>
                )}
            </div>
        </Button>
    )
}

export const LoginBtn = ({isLoading}: {
    isLoading: boolean
}) => {

    return (
        <Button type="submit" className={button({color: "primary", disabled: isLoading})}>
            <div className="flex items-center justify-center w-full">
                {!isLoading ? (
                    <>
                        <LogIn strokeWidth={0.75} className="size-4 stroke-blue-400"/>
                        <span className="pr-2 font-iran-sans">
                           ورود
                         </span>
                    </>
                ) : (
                    <>
                        <BtnLoading/>
                        <span className="pr-2 font-iran-sans">
                           ورود
                         </span>
                    </>
                )}

            </div>
        </Button>
    )
}


export const PaginationBtn = ({
                                  message,
                                  onClick,
                                  isSelected = false,
    disabled = false,
                              }: {
    message: ReactNode;
    isSelected?: boolean;
    disabled?: boolean;
    onClick: () => void;
}) => {

    return (
        <Button type="submit" className={sqButton({color: "default", isSelected: isSelected, disabled: disabled})} onClick={onClick}>
            {message}
        </Button>
    )
}

export const SquareBtn = ({
                             message,
                              onClick = undefined,
                         }: {
    message: ReactNode;
    onClick?: <T,>(obj:T) => void;

}) => {

    return (
        <Button type="submit" className={sqButton({color: "default"})} onClick={onClick}>
            {message}
        </Button>
    )
}



