import { ToastContainer} from "react-toastify";
import { ExclamationCircleIcon,ExclamationTriangleIcon,InformationCircleIcon,CheckCircleIcon,XCircleIcon } from '@heroicons/react/24/solid'
import { toast } from "react-toastify";

import 'react-toastify/dist/ReactToastify.css';

export default function Toast(){

    const contextClass = {
        success: "bg-green-800",
        error: "bg-red-800",
        info: "bg-sky-800",
        warning: "bg-orange-800",
        default: "bg-zinc-800",
        dark: "bg-white-600 font-gray-300",
    };

    const icon ={
        success: <CheckCircleIcon className="text-white/30 size-8"/>,
        error: <ExclamationCircleIcon className="text-white/30 size-8"/>,
        info: <InformationCircleIcon className="text-white/30 size-8"/>,
        warning: <ExclamationTriangleIcon className="text-white/30 size-8"/>,
        default: false,
        dark: false,
    }

    return(
        <ToastContainer
            toastClassName={(obj ) => {
                return `${contextClass[obj?.type|| "default"]} relative w-80 my-2 text-white flex p-2 min-h-16 rounded-sm items-center justify-between overflow-hidden cursor-pointer font-iran-sans text-xs`;
            }}
            icon={(obj)=>{return icon[obj.type]}}
            closeButton={({ closeToast }) => <XCircleIcon onClick={closeToast} className="absolute top-1 right-1 size-4 text-white/50 hover:text-white transition-colors duration-500 ease-out"/>}
            position="bottom-left"
            autoClose={3000}
            hideProgressBar={true}
            pauseOnHover={false}
            rtl={true}
        />
    )
}


// toastHelper.tsx

export const toastError = (message: string) => {
    toast.error(<span dir="rtl">{message}</span>);
};