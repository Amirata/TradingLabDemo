// import {FaCheckCircle, FaExclamationCircle, FaExclamationTriangle, FaInfoCircle} from "react-icons/fa";
// import Button from "../forms/Button.tsx";
// import {MdOutlineClose} from "react-icons/md";
// import {ReactNode} from "react";
// import MakeClass from "../../utilities/MakeClass.ts";
//
// interface Props {
//     danger?: boolean,
//     warning?: boolean,
//     info?: boolean,
//     success?: boolean,
//     headTitle: string,
//     message: string,
//     buttonMessage: ReactNode,
//     isLoading?: boolean,
//     loadingMessage?: string,
//     onCloseClick: () => void,
//     onButtonClick: () => void | Promise<void>
// }
//
// export default function AlertCard({
//                                       headTitle,
//                                       message,
//                                       onCloseClick,
//                                       buttonMessage,
//                                       onButtonClick,
//                                       danger,
//                                       warning,
//                                       success,
//                                       info,
//                                       isLoading,
//                                       loadingMessage
//                                   }: Props) {
//
//
//     const signClasses = MakeClass("absolute flex items-center justify-center rounded-full  w-16 h-16 -top-8 -right-8 shadow-md", {
//         "bg-rose-300/80": danger,
//         "bg-orange-300/80": warning,
//         "bg-green-300/80": success,
//         "bg-sky-300/80": info,
//     });
//     const signIconClasses = MakeClass("w-8 h-8 ", {
//         "text-rose-500": danger,
//         "text-orange-500": warning,
//         "text-green-500": success,
//         "text-sky-500": info,
//     });
//
//     const headerClasses = MakeClass("flex items-center h-12 rounded-t-lg pl-2 pr-4 bg-gradient-to-l ", {
//         "from-rose-400 to-rose-200": danger,
//         "from-orange-400 to-orange-200": warning,
//         "from-green-400 to-green-200": success,
//         "from-sky-400 to-sky-200": info,
//     });
//
//     let icon = <FaExclamationCircle className={signIconClasses}/>
//     if (warning) {
//         icon = <FaExclamationTriangle className={signIconClasses}/>
//     } else if (success) {
//         icon = <FaCheckCircle className={signIconClasses}/>
//     } else if (info) {
//         icon = <FaInfoCircle className={signIconClasses}/>
//     }
//
//     const closeClasses = MakeClass("btn-md-square rounded-full",{
//         "btn-rose-white":danger,
//         "btn-orange-white":warning,
//         "btn-emerald-white":success,
//         "btn-sky-white":info,
//     })
//
//     const btnClasses = MakeClass("btn-md rounded-sm",{
//         "btn-rose-white":danger,
//         "btn-orange-white":warning,
//         "btn-emerald-white":success,
//         "btn-sky-white":info,
//     })
//
//     return (
//
//         <>
//             <div className="relative h-1/3 w-5/12 shadow-md bg-white rounded-lg">
//
//                 <div
//                     className={signClasses}>
//                     {icon}
//                 </div>
//                 <div className="flex flex-col h-full">
//                     <div
//                         className={headerClasses}>
//                         <h2 className="ml-auto pr-5 text-white">{headTitle}</h2>
//                         <div className="ml-1">
//                             <Button message={<MdOutlineClose className="text-white"/>} type="button"
//                                     onClick={onCloseClick}
//                                     className={closeClasses}/>
//                         </div>
//                     </div>
//                     <div className="flex flex-col p-4 flex-grow ">
//                         <div className="overflow-hidden mb-auto text-xs">
//                             <p className="break-words text-justify">{message}</p>
//                         </div>
//                         <div className="flex justify-center">
//                             <Button  message={buttonMessage}
//                                     type="button" className={btnClasses} onClick={onButtonClick}
//                                     loadingMessage={loadingMessage}
//                                     isLoading={isLoading}/>
//                         </div>
//
//                     </div>
//                 </div>
//             </div>
//
//         </>
//     )
// }