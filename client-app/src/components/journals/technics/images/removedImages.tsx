import React from "react";
//import { Image } from "@nextui-org/image";
import {UseFormSetValue} from "react-hook-form";
import {ArchiveRestore} from "lucide-react";

//import { FaTrashRestore } from "react-icons/fa";

interface RemovedImagesProps {
    onChange: (images: string[]) => void;
    value: string[];
    setValue: UseFormSetValue<{
        images?: (string | undefined)[] | undefined;
        removedImages?: (string | undefined)[] | undefined;
        newImages?: (File | undefined)[] | undefined;
        id: string;
        name: string;
        description: string;
    }>;
    imagesItems: string[];
}

const RemovedImages: React.FC<RemovedImagesProps> = ({
                                                         onChange,
                                                         value,
                                                         setValue,
                                                         imagesItems,
                                                     }) => {
    const baseUrl = import.meta.env.VITE_APP_IMG_URL as string;
    return (
        <div className="rounded-sm min-h-32 bg-zinc-800 p-5 text-white/80 border border-zinc-500">
            <h2 className="font-iran-sans">تصاویر حذف شونده</h2>
            <div className="flex justify-center gap-2 pt-5 flex-wrap">
                {value?.map((removedImage, index) => (
                    <button
                        key={index}
                        className="group relative flex-none w-32 h-32 cursor-pointer"
                        type="button"
                        onClick={() => {
                            onChange(value?.filter((img) => img !== removedImage));
                            setValue("images", [
                                ...imagesItems,
                                removedImage as string,
                            ]);
                        }}
                    >
                        <img
                            key={index}
                            className="w-full h-full object-cover"
                            src={`${baseUrl}${removedImage}`}
                        />
                        <span
                            className="z-10 opacity-0 flex group-hover:opacity-60 justify-center items-center bg-zinc-700 rounded-full w-12 h-12 absolute left-1/2 top-1/2 transform -translate-x-1/2 -translate-y-1/2 text-3xl transition ease-in-out duration-300">
              <ArchiveRestore/>
                            {/*<FaTrashRestore className="w-3.5 h-3.5 text-zinc-100" />*/}
            </span>
                    </button>
                ))}
            </div>
        </div>
    );
};

export default RemovedImages;
